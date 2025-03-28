using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Admin
{
    public class LoginModel(IUserRepository userRepository) : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public string ReturnUrl { get; set; } = "/Admin/Index"; // Default to home

        private void ValidateUrl()
        {
            var returnUrl = Request.Query["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                ReturnUrl = returnUrl;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = userRepository.GetUserByUsername(Username);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            if (BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(principal);

                ValidateUrl();
                return LocalRedirect(ReturnUrl); // Redirect to a protected page
            }

            ErrorMessage = "Invalid username or password.";
            return Page();
        }
    }

}
