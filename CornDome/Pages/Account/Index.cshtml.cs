using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Account
{
    public class LocalUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }

    [Authorize]
    public class IndexModel(IUserRepository userRepository) : PageModel
    {
        [BindProperty]
        public LocalUser LocalUser { get; set; } = new LocalUser();
        public async void OnGet()
        {
            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));

            LocalUser.Email = loggedInUser?.Email;
            LocalUser.Username = User.Identity.Name;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));

            loggedInUser.UserName = LocalUser.Username;
            var updated = await userRepository.UpdateUser(loggedInUser);

            if (updated)
            {
                TempData["Message"] = $"Username has been updated. Please re-login to display these changes.";
            }

            LocalUser.Email = loggedInUser.Email;
            LocalUser.Username = LocalUser.Username;

            return Page();
        }
    }
}
