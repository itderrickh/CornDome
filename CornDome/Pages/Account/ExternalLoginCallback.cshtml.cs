using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Account
{
    public class ExternalLoginCallbackModel(IUserRepository userRepository, SignInManager<User> signInManager) : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Get external login info
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            // Check if the user already exists in your local database
            var user = await userRepository.GetUserByEmail(email);

            if (user == null)
            {
                // Max length on name
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                var trimmedName = name.Length > 20 ? name[..20] : name;

                // Create a new ApplicationUser with external login details
                var createdUser = new User
                {
                    Username = trimmedName,
                    Email = email
                };

                // Create the user in the database
                var result = await userRepository.CreateUser(createdUser);
                if (!result)
                {
                    // Handle errors (add to ModelState, return to login page, etc.)
                    ModelState.AddModelError(string.Empty, "User creation failed");
                    return Page();
                }

                // Pull details from DB
                user = await userRepository.GetUserByEmail(email);
            }

            if (user == null)
            {
                // Handle errors (add to ModelState, return to login page, etc.)
                ModelState.AddModelError(string.Empty, "User creation failed");
                return Page();
            }

            // If the user is found, sign them in
            await signInManager.SignInAsync(user, isPersistent: false);

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
