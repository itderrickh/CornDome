using CornDome.Helpers;
using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace CornDome.Pages.Play;

public class CallbackModel : PageModel
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenProtector _tokenProtector;

    public CallbackModel(
        IConfiguration config,
        IUserRepository userRepository,
        ITokenProtector tokenProtector,
        IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _userRepository = userRepository;
        _httpClientFactory = httpClientFactory;
        _tokenProtector = tokenProtector;
    }

    public async Task<string> GetValidAccessTokenAsync(DiscordConnection conn)
    {
        if (conn.TokenExpiresAt > DateTime.UtcNow.AddMinutes(1))
            return _tokenProtector.Unprotect(conn.EncryptedAccessToken);

        var client = _httpClientFactory.CreateClient();

        var res = await client.PostAsync(
            "https://discord.com/api/oauth2/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _config["Discord:ClientId"],
                ["client_secret"] = _config["Discord:ClientSecret"],
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = _tokenProtector.Unprotect(conn.EncryptedRefreshToken)
            })
        );

        var json = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement;

        conn.EncryptedAccessToken =
            _tokenProtector.Protect(json.GetProperty("access_token").GetString());
        conn.EncryptedRefreshToken =
            _tokenProtector.Protect(json.GetProperty("refresh_token").GetString());
        conn.TokenExpiresAt =
            DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32());

        await _userRepository.UpdateDiscordTokens(conn);
        
        return _tokenProtector.Unprotect(conn.EncryptedAccessToken);
    }

    public async Task<IActionResult> OnGetAsync(string code)
    {
        if (string.IsNullOrEmpty(code))
            return RedirectToPage("/Error");

        var client = _httpClientFactory.CreateClient();

        // Exchange code
        var tokenRes = await client.PostAsync(
            "https://discord.com/api/oauth2/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _config["Discord:ClientId"],
                ["client_secret"] = _config["Discord:ClientSecret"],
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = _config["Discord:RedirectUri"]
            })
        );

        var tokenJson = JsonDocument.Parse(await tokenRes.Content.ReadAsStringAsync()).RootElement;

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

        var existing = await _userRepository.GetDiscordConnection(int.Parse(appUserId));

        if (existing != null && existing.UserId != appUserId)
            return Forbid(); // Discord already linked to another account

        var entity = existing ?? new DiscordConnection
        {
            UserId = appUserId,
            DiscordUserId = discordUserId
        };

        entity.Username = userJson.GetProperty("username").GetString();
        entity.Discriminator = userJson.GetProperty("discriminator").GetString();
        entity.AvatarHash = userJson.GetProperty("avatar").GetString();
        entity.EncryptedAccessToken = _tokenProtector.Protect(accessToken);
        entity.EncryptedRefreshToken = _tokenProtector.Protect(refreshToken);
        entity.TokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

        if (existing == null)
        {
            var result = await _userRepository.InsertDiscordConnection(entity);
            if (!result)
                return RedirectToPage("/Error");
        }


        return RedirectToPage("/Account/Connections");
    }
}