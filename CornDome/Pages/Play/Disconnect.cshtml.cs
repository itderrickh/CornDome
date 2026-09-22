using CornDome.Models;
using CornDome.Repository.Discord;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CornDome.Pages.Play
{
    public class DisconnectModel(IDiscordRepository discordRepository) : BasePageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var conn = discordRepository.GetDiscordConnection(int.Parse(userId));

            if (conn == null)
                return RedirectToPage("/Account/Connections");

            await discordRepository.RemoveDiscordConnection(conn);

            return RedirectToPage("/Account/Connections");
        }
    }
}
