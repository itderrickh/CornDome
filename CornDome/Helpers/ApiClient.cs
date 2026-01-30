using CornDome.Repository.Discord;

namespace CornDome.Helpers
{
    public class ApiClient(HttpClient httpClient, Config config) : IApiClient
    {
        public DiscordConfiguration GetDiscordConfiguration()
        {
            return config.DiscordClient;
        }

        public async Task<HttpClient> GetHttpClient()
        {
            return httpClient;
        }
    }
}
