using CornDome.Models.Users;
using CornDome.Repository.Discord;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace CornDome.Pages.Play;

public class CallbackModel(
    Config config,
    IDiscordRepository discordRepository,
    ITokenProtector tokenProtector,
    IHttpClientFactory httpClientFactory) : PageModel
{

    public async Task<IActionResult> OnGetAsync(string code)
    {
        if (string.IsNullOrEmpty(code))
            return RedirectToPage("/Error");

        var client = httpClientFactory.CreateClient();
        var discordConfig = config.DiscordClient;
        var redirectUrl = Url.Page("/Play/Callback", pageHandler: null, values: null, protocol: Request.Scheme);

        // Exchange code
        var tokenRes = await client.PostAsync(
            "https://discord.com/api/oauth2/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = discordConfig.ClientId,
                ["client_secret"] = discordConfig.ClientSecret,
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = redirectUrl
            })
        );

        var tokenContent = await tokenRes.Content.ReadAsStringAsync();
        var tokenJson = JsonDocument.Parse(tokenContent).RootElement;

        var accessToken = tokenJson.GetProperty("access_token").GetString();
        var refreshToken = tokenJson.GetProperty("refresh_token").GetString();
        var expiresIn = tokenJson.GetProperty("expires_in").GetInt32();

        // Get user
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var userJson = JsonDocument.Parse(
            await client.GetStringAsync("https://discord.com/api/users/@me")
        ).RootElement;

        var discordUserId = userJson.GetProperty("id").GetString();
        var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existing = await discordRepository.GetDiscordConnection(int.Parse(appUserId));

        if (existing != null && existing.UserId != int.Parse(appUserId))
            return Forbid(); // Discord already linked to another account

        var entity = existing ?? new DiscordConnection
        {
            UserId = int.Parse(appUserId),
            DiscordUserId = discordUserId
        };

        entity.Username = userJson.GetProperty("username").GetString();
        entity.Discriminator = userJson.GetProperty("discriminator").GetString();
        entity.AvatarHash = userJson.GetProperty("avatar").GetString();
        entity.EncryptedAccessToken = tokenProtector.Protect(accessToken);
        entity.EncryptedRefreshToken = tokenProtector.Protect(refreshToken);
        entity.TokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

        if (existing == null)
        {
            var result = await discordRepository.AddDiscordConnection(entity);
            if (!result)
                return RedirectToPage("/Error");
        }


        return RedirectToPage("/Play/Board");
    }
}