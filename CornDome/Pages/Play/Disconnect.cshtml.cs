using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Play
{
    public class DisconnectModel(IUserRepository userRepository) : PageModel
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var conn = await _userRepository.GetDiscordConnection(int.Parse(userId));

            if (conn == null)
                return RedirectToPage("/Account/Connections");

            await _userRepository.RemoveDiscordConnection(int.Parse(userId));

            return RedirectToPage("/Account/Connections");
        }
    }
}
