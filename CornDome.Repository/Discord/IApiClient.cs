namespace CornDome.Repository.Discord
{
    public interface IApiClient
    {
        Task<HttpClient> GetHttpClient();
        DiscordConfiguration GetDiscordConfiguration();
    }
}
