using CornDome.Models;
using CornDome.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CornDome.Repository.Discord
{
    public interface IDiscordRepository
    {
        Task<bool> IsUserInGuildAsync(DiscordConnection connection);
        Task<bool> AddDiscordConnection(DiscordConnection connection);
        Task<string> GetValidAccessTokenAsync(DiscordConnection connection);
        Task<DiscordConnection> GetDiscordConnection(int userId);
        Task<bool> RemoveDiscordConnection(DiscordConnection connection);

        Task<IEnumerable<PlayAvailability>> GetPlayAvailabilities(int userId);
        Task<bool> UpdatePlayAvailabilities(int userId, List<PlayAvailability> playAvailabilities);

        Task<PlayPreferences> GetPlayPreferences(int userId);
        Task<bool> UpdatePlayPreferences(PlayPreferences playPreferences);

        Task<IEnumerable<PlayPreferences>> GetAllActivePlayPreferences();
    }

    public class DiscordRepository(ITokenProtector tokenProtector, IApiClient apiClient, MainContext context) : IDiscordRepository
    {
        public async Task<bool> AddDiscordConnection(DiscordConnection connection)
        {
            try
            {
                context.Add(connection);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetValidAccessTokenAsync(DiscordConnection conn)
        {
            if (conn.TokenExpiresAt > DateTime.UtcNow.AddMinutes(1))
                return tokenProtector.Unprotect(conn.EncryptedAccessToken);

            var client = await apiClient.GetHttpClient();
            var discordConfig = apiClient.GetDiscordConfiguration();

            var res = await client.PostAsync(
                "https://discord.com/api/oauth2/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = discordConfig.ClientId,
                    ["client_secret"] = discordConfig.ClientSecret,
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = tokenProtector.Unprotect(conn.EncryptedRefreshToken)
                })
            );

            var json = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement;

            conn.EncryptedAccessToken =
                tokenProtector.Protect(json.GetProperty("access_token").GetString());
            conn.EncryptedRefreshToken =
                tokenProtector.Protect(json.GetProperty("refresh_token").GetString());
            conn.TokenExpiresAt =
                DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32());

            await context.SaveChangesAsync();

            return tokenProtector.Unprotect(conn.EncryptedAccessToken);
        }

        public async Task<bool> RefreshAccessToken(DiscordConnection discordConnection)
        {
            try
            {
                await GetValidAccessTokenAsync(discordConnection);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> IsUserInGuildAsync(DiscordConnection connection)
        {
            var token = await GetValidAccessTokenAsync(connection);
            var discordConfig = apiClient.GetDiscordConfiguration();

            var client = await apiClient.GetHttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.GetAsync(
                "https://discord.com/api/users/@me/guilds");

            if (!response.IsSuccessStatusCode)
                return false;

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            foreach (var guild in doc.RootElement.EnumerateArray())
            {
                if (guild.GetProperty("id").GetString() == discordConfig.GuildId)
                    return true;
            }

            return false;
        }

        public async Task<DiscordConnection> GetDiscordConnection(int userId)
        {
            return context.DiscordConnections.FirstOrDefault(x => x.UserId == userId);
        }

        public async Task<bool> RemoveDiscordConnection(DiscordConnection connection)
        {
            try
            {
                context.Remove(connection);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false; 
            }
        }

        public async Task<IEnumerable<PlayAvailability>> GetPlayAvailabilities(int userId)
        {
            return context.PlayAvailabilities
                .Where(x => x.UserId == userId);
        }

        public async Task<bool> UpdatePlayAvailabilities(int userId, List<PlayAvailability> playAvailabilities)
        {
            try
            {
                var existing = context.PlayAvailabilities
                    .Where(x => x.UserId == userId);

                context.PlayAvailabilities.RemoveRange(existing);

                foreach (var day in playAvailabilities)
                {
                    context.PlayAvailabilities.Add(day);
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PlayPreferences> GetPlayPreferences(int userId)
        {
            return context.PlayPreferences
                .FirstOrDefault(x => x.UserId == userId);
        }

        public async Task<bool> UpdatePlayPreferences(PlayPreferences playPreferences)
        {
            try
            {
                var existing = context.PlayPreferences.FirstOrDefault(x => playPreferences.UserId == x.UserId);

                if (existing != null)
                {
                    existing.Platform = playPreferences.Platform;
                    existing.Pronouns = playPreferences.Pronouns;
                    existing.AddressMeAs = playPreferences.AddressMeAs;
                    existing.GameFormat = playPreferences.GameFormat;
                    existing.TimeZone = playPreferences.TimeZone;
                    existing.WantsToPlay = playPreferences.WantsToPlay;
                }
                else
                {
                    context.Add(playPreferences);
                }
                
                await context.SaveChangesAsync();
                return true;
            } 
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<PlayPreferences>> GetAllActivePlayPreferences()
        {
            return context.PlayPreferences
                .Where(x => x.WantsToPlay);
        }
    }
}
