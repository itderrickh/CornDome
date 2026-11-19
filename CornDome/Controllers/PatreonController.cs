using CornDome.Models.Users;
using CornDome.Repository;
using CornDome.Repository.Patreon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.Json;

namespace CornDome.Controllers
{
    [Authorize]
    [Route("Patreon")]
    public class PatreonController(
        IPatreonRepository patreonRepository,
        IPatreonRoleService patreonRoleService,
        UserManager<User> userManager,
        MainContext context,
        IDataProtectionProvider dataProtectionProvider,
        Config config) : Controller
    {
        private readonly ITimeLimitedDataProtector _stateProtector =
            dataProtectionProvider
                .CreateProtector("CornDome.Patreon.OAuthState.v1")
                .ToTimeLimitedDataProtector();

        private record OAuthState(int UserId, string Nonce);

        // Redirect the logged-in CardDweeb user to Patreon.
        [HttpGet("Connect")]
        public async Task<IActionResult> Connect()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var statePayload = new OAuthState(
                user.Id,
                Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));

            var state = _stateProtector.Protect(
                JsonSerializer.Serialize(statePayload),
                TimeSpan.FromMinutes(10));

            return Redirect(patreonRepository.GetAuthorizationUrl(state));
        }

        // Patreon redirects back here after authorization.
        [HttpGet("Callback")]
        public async Task<IActionResult> Callback(string? code, string? state, string? error)
        {
            if (!string.IsNullOrEmpty(error))
                return Redirect("/Account?patreon=denied");

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
                return BadRequest("Missing authorization code or state.");

            OAuthState? statePayload;

            try
            {
                var unprotectedState = _stateProtector.Unprotect(state, out _);
                statePayload = JsonSerializer.Deserialize<OAuthState>(unprotectedState);
            }
            catch (CryptographicException)
            {
                return BadRequest("Invalid or expired Patreon authorization state.");
            }
            catch (JsonException)
            {
                return BadRequest("Invalid Patreon authorization state.");
            }

            if (statePayload == null)
                return BadRequest("Invalid Patreon authorization state.");

            var user = await userManager.GetUserAsync(User);

            if (user == null || user.Id != statePayload.UserId)
                return BadRequest("Your CardDweeb account does not match the account that started authorization.");

            // Exchange the authorization code for tokens.
            var token = await patreonRepository.ExchangeCodeAsync(code);

            // Retrieve the Patreon account and membership information.
            var identity = await patreonRepository.GetIdentityAsync(token.AccessToken);
            var patreonUserId = identity.Data.Id;

            if (string.IsNullOrWhiteSpace(patreonUserId))
                return BadRequest("Could not identify Patreon account.");

            if (string.IsNullOrWhiteSpace(config.PatreonConfig.CampaignId))
                throw new InvalidOperationException("Patreon CampaignId is not configured.");

            // Find currently entitled tiers from memberships belonging to our campaign.
            var entitledTierIds = GetEntitledTierIds(identity, config.PatreonConfig.CampaignId);

            // Ensure this Patreon account isn't linked to another CardDweeb user.
            var existingConnection = await context.Set<PatreonConnection>()
                .FirstOrDefaultAsync(x => x.PatreonUserId == patreonUserId);

            if (existingConnection != null && existingConnection.UserId != user.Id)
                return Conflict("This Patreon account is already connected to another CardDweeb account.");

            // Find or create this user's Patreon connection.
            var connection = await context.Set<PatreonConnection>()
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (connection == null)
            {
                connection = new PatreonConnection
                {
                    UserId = user.Id,
                    PatreonUserId = patreonUserId
                };

                context.Add(connection);
            }
            else
            {
                connection.PatreonUserId = patreonUserId;
            }

            if (entitledTierIds.Count == 1)
            {
                connection.CurrentTierId = entitledTierIds.First();
            }

            connection.AccessToken = token.AccessToken;
            connection.RefreshToken = token.RefreshToken;
            connection.TokenExpiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn);
            connection.LastSyncedAt = DateTime.UtcNow;
            connection.IsActive = true;

            await context.SaveChangesAsync();

            // Synchronize Patreon roles without affecting unrelated roles.
            await patreonRoleService.SyncRolesAsync(user, entitledTierIds);

            return Redirect("/Account?patreon=connected");
        }

        // Disconnect the currently logged-in user's Patreon account.
        [HttpPost("Disconnect")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disconnect()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var connection = await context.Set<PatreonConnection>()
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (connection != null)
            {
                context.Remove(connection);
                await context.SaveChangesAsync();
            }

            // Remove Patreon roles while preserving unrelated roles.
            await patreonRoleService.SyncRolesAsync(user, []);

            return Redirect("/Account?patreon=disconnected");
        }

        private static List<string> GetEntitledTierIds(PatreonIdentityResponse identity, string campaignId)
        {
            return identity.Included
                .Where(x => x.Type == "member")
                .Where(x => GetRelationshipId(x, "campaign") == campaignId)
                .SelectMany(x => GetRelationshipIds(x, "currently_entitled_tiers"))
                .Distinct()
                .ToList();
        }

        private static string? GetRelationshipId(PatreonResource resource, string relationshipName)
        {
            if (!resource.Relationships.TryGetValue(relationshipName, out var relationship))
                return null;

            var data = relationship.Data;

            if (data.ValueKind != JsonValueKind.Object)
                return null;

            return data.TryGetProperty("id", out var id) ? id.GetString() : null;
        }

        private static IEnumerable<string> GetRelationshipIds(PatreonResource resource, string relationshipName)
        {
            if (!resource.Relationships.TryGetValue(relationshipName, out var relationship))
                yield break;

            var data = relationship.Data;

            if (data.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in data.EnumerateArray())
                {
                    if (item.TryGetProperty("id", out var id) && id.GetString() is string tierId)
                        yield return tierId;
                }
            }
            else if (data.ValueKind == JsonValueKind.Object)
            {
                if (data.TryGetProperty("id", out var id) && id.GetString() is string tierId)
                    yield return tierId;
            }
        }
    }
}
