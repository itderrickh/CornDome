using CornDome.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CornDome.Repository.Patreon
{
    public interface IPatreonSyncService
    {
        Task SyncAllAsync(CancellationToken cancellationToken);
    }

    public class PatreonSyncService(
        MainContext context,
        IPatreonRepository patreonRepository,
        IPatreonRoleService patreonRoleService,
        IOptions<PatreonOptions> options,
        ILogger<PatreonSyncService> logger) : IPatreonSyncService
    {
        private readonly PatreonOptions _options = options.Value;

        public async Task SyncAllAsync(CancellationToken cancellationToken)
        {
            var connections = await context.Set<PatreonConnection>()
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Starting Patreon sync for {Count} accounts.", connections.Count);

            foreach (var connection in connections)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await SyncAccountAsync(connection, cancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Failed to sync Patreon account for CardDweeb user {UserId}.", connection.UserId);
                }
            }

            logger.LogInformation("Finished Patreon synchronization.");
        }

        private async Task SyncAccountAsync(PatreonConnection connection, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(connection.AccessToken))
                throw new InvalidOperationException("Patreon access token is missing.");

            // Refresh the access token if it expires within five minutes.
            if (connection.TokenExpiresAt <= DateTime.UtcNow.AddMinutes(5))
            {
                if (string.IsNullOrWhiteSpace(connection.RefreshToken))
                    throw new InvalidOperationException("Patreon refresh token is missing.");

                var refreshedToken = await patreonRepository.RefreshTokenAsync(connection.RefreshToken);

                connection.AccessToken = refreshedToken.AccessToken;

                if (!string.IsNullOrWhiteSpace(refreshedToken.RefreshToken))
                    connection.RefreshToken = refreshedToken.RefreshToken;

                connection.TokenExpiresAt = DateTime.UtcNow.AddSeconds(refreshedToken.ExpiresIn);

                await context.SaveChangesAsync(cancellationToken);
            }

            // Retrieve current Patreon membership information.
            var identity = await patreonRepository.GetIdentityAsync(connection.AccessToken);

            if (identity.Data.Id != connection.PatreonUserId)
                throw new InvalidOperationException("Patreon returned a different account than the one linked.");

            if (string.IsNullOrWhiteSpace(_options.CampaignId))
                throw new InvalidOperationException("Patreon CampaignId is not configured.");

            // Only collect tiers from memberships belonging to our campaign.
            var entitledTierIds = GetEntitledTierIds(identity, _options.CampaignId);

            var user = await context.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == connection.UserId, cancellationToken);

            if (user == null)
                throw new InvalidOperationException("The linked CardDweeb user no longer exists.");

            await patreonRoleService.SyncRolesAsync(user, entitledTierIds);

            connection.LastSyncedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully synced Patreon roles for CardDweeb user {UserId}.", user.Id);
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
