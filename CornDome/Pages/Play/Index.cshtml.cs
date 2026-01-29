using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Play
{
    public class IndexModel : PageModel
    {
        private readonly Config _config;

        public IndexModel(Config config)
        {
            _config = config;
        }

        public IActionResult OnGet()
        {
            var clientId = _config.DiscordClient.ClientId;
            var redirectUri = "/Play/Board";

            var url =
                $"https://discord.com/api/oauth2/authorize" +
                $"?client_id={clientId}" +
                $"&redirect_uri={redirectUri}" +
                $"&response_type=code" +
                $"&scope=identify email";

            return Redirect(url);
        }
    }
}
