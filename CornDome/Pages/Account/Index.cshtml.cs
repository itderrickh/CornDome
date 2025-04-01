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
        public void OnGet()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var loggedInUser = userRepository.GetUserByEmail(userEmail);

            LocalUser.Email = loggedInUser?.Email;
            LocalUser.Username = User.Identity.Name;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var loggedInUser = userRepository.GetUserByEmail(userEmail);

            loggedInUser.Username = LocalUser.Username;
            var updated = userRepository.UpdateUser(loggedInUser);

            if (updated)
            {
                TempData["Message"] = $"Username has been updated";
            }

            LocalUser.Email = userEmail;
            LocalUser.Username = LocalUser.Username;

            return Page();
        }
    }
}
