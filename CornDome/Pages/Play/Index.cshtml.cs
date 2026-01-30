using CornDome.Repository;
using CornDome.Repository.Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Play
{
    [Authorize]
    public class IndexModel(Config config, IUserRepository userRepository, IDiscordRepository discordRepository) : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));

            var discordConnection = await discordRepository.GetDiscordConnection(loggedInUser.Id);

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
