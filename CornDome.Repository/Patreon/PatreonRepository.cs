
using CornDome.Models.Users;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CornDome.Repository.Patreon
{
    public class PatreonOptions
    {
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string RedirectUri { get; set; } = "";
        public string CampaignId { get; set; } = "";

        public Dictionary<string, string> TierRoles { get; set; } = [];
    }

    public class PatreonTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = "";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "";
    }

    public class PatreonResource
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("attributes")]
        public Dictionary<string, JsonElement> Attributes { get; set; } = [];

        [JsonPropertyName("relationships")]
        public Dictionary<string, PatreonRelationship> Relationships { get; set; } = [];
    }

    public class PatreonRelationship
    {
        [JsonPropertyName("data")]
        public JsonElement Data { get; set; }
    }

    public class PatreonIdentityResponse
    {
        [JsonPropertyName("data")]
        public PatreonResource Data { get; set; } = new();

        [JsonPropertyName("included")]
        public List<PatreonResource> Included { get; set; } = [];
    }

    public interface IPatreonRepository
    {
        string GetAuthorizationUrl(string state);
        Task<PatreonTokenResponse> ExchangeCodeAsync(string code);
        Task<PatreonIdentityResponse> GetIdentityAsync(string accessToken);
        Task<PatreonTokenResponse> RefreshTokenAsync(string refreshToken);
        Task<PatreonConnection> GetUserConnection(int userId);
    }

    public class PatreonRepository(MainContext mainContext, HttpClient httpClient, IOptions<PatreonOptions> options) : IPatreonRepository
    {
        private const string AuthorizationEndpoint =
            "https://www.patreon.com/oauth2/authorize";

        private const string TokenEndpoint =
            "https://www.patreon.com/api/oauth2/token";

        private const string IdentityEndpoint =
            "https://www.patreon.com/api/oauth2/v2/identity";

        private const string UserAgent =
            "CardDweeb Patreon Integration/1.0";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public string GetAuthorizationUrl(string state)
        {
            var query = new Dictionary<string, string?>
            {
                ["response_type"] = "code",
                ["client_id"] = options.Value.ClientId,
                ["redirect_uri"] = options.Value.RedirectUri,
                ["scope"] = "identity identity.memberships",
                ["state"] = state
            };

            return QueryHelpers.AddQueryString(
                AuthorizationEndpoint,
                query);
        }

        public async Task<PatreonTokenResponse> ExchangeCodeAsync(string code)
        {
            var parameters = new Dictionary<string, string>
            {
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["client_id"] = options.Value.ClientId,
                ["client_secret"] = options.Value.ClientSecret,
                ["redirect_uri"] = options.Value.RedirectUri
            };

            return await RequestTokenAsync(parameters);
        }

        public async Task<PatreonTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var parameters = new Dictionary<string, string>
            {
                ["refresh_token"] = refreshToken,
                ["grant_type"] = "refresh_token",
                ["client_id"] = options.Value.ClientId,
                ["client_secret"] = options.Value.ClientSecret
            };

            return await RequestTokenAsync(parameters);
        }

        private async Task<PatreonTokenResponse> RequestTokenAsync(Dictionary<string, string> parameters)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                TokenEndpoint);

            request.Headers.UserAgent.ParseAdd(UserAgent);
            request.Content = new FormUrlEncodedContent(parameters);

            using var response = await httpClient.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Patreon token request failed " +
                    $"({(int)response.StatusCode}): {content}",
                    null,
                    response.StatusCode);
            }

            return JsonSerializer.Deserialize<PatreonTokenResponse>(
                content,
                JsonOptions)
                ?? throw new InvalidOperationException(
                    "Patreon returned an empty token response.");
        }

        public async Task<PatreonIdentityResponse> GetIdentityAsync(string accessToken)
        {
            // Request membership information, campaign relationships, and entitled tiers.
            var query = new Dictionary<string, string?>
            {
                ["include"] =
                    "memberships,memberships.currently_entitled_tiers,memberships.campaign",

                ["fields[user]"] =
                    "full_name,email",

                ["fields[member]"] =
                    "patron_status,last_charge_status," +
                    "currently_entitled_amount_cents",

                ["fields[tier]"] =
                    "title,amount_cents",

                ["fields[campaign]"] =
                    "creation_name",
            };

            var url = QueryHelpers.AddQueryString(IdentityEndpoint, query);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.UserAgent.ParseAdd(UserAgent);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Patreon identity request failed ({(int)response.StatusCode}): {content}",
                    null,
                    response.StatusCode);
            }

            return JsonSerializer.Deserialize<PatreonIdentityResponse>(content, JsonOptions)
                ?? throw new InvalidOperationException("Patreon returned an empty identity response.");
        }

        public async Task<PatreonConnection> GetUserConnection(int userId)
        {
            return await mainContext.Set<PatreonConnection>().FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
        }
    }
}
