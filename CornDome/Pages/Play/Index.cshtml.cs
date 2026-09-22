using CornDome.Models;
using CornDome.Repository.Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Play
{
    [Authorize]
    public class IndexModel(Config config, IDiscordRepository discordRepository) : BasePageModel
    {
        public async Task<IActionResult> OnGet()
        {
            var loggedInUser = await GetUser();

            var discordConnection = discordRepository.GetDiscordConnection(loggedInUser.Id);

            if (discordConnection == null)
            {
                var clientId = config.DiscordClient.ClientId;

                var redirectUrl = Url.Page("/Play/Callback", pageHandler: null, values: null, protocol: Request.Scheme);

                var url =
                    $"https://discord.com/api/oauth2/authorize" +
                    $"?client_id={clientId}" +
                    $"&redirect_uri={redirectUrl}" +
                    $"&response_type=code" +
                    $"&scope=guilds+email+identify";

                return Redirect(url);
            }
            else
            {
                return RedirectToPage("/Play/Board");
            }
        }
    }
}
