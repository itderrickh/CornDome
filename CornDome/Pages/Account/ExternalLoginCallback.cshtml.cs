using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Account
{
    public class ExternalLoginCallbackModel(IUserRepository userRepository, SignInManager<User> signInManager) : PageModel
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly SignInManager<User> _signInManager = signInManager;

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Get external login info
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            // Check if the user already exists in your local database
            var user = _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                // The user doesn't exist in your local database, so create a new user
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                // Create a new ApplicationUser with external login details
                user = new User
                {
                    Username = name,
                    Email = email
                };

                // Create the user in the database
                var result = userRepository.CreateUser(user);
                if (!result)
                {
                    // Handle errors (add to ModelState, return to login page, etc.)
                    ModelState.AddModelError(string.Empty, "User creation failed");
                    return Page();
                }
            }

            // If the user is found, sign them in
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect the user to their return URL or default page
            return RedirectToLocal(returnUrl);
        }

        // Helper method for redirecting to the local page
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
