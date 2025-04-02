using CornDome.Models.Users;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Account
{
    public class LoginModel(SignInManager<User> signInManager) : PageModel
    {
        private readonly SignInManager<User> _signInManager = signInManager;

        public IActionResult OnGet()
        {
            var redirectUrl = Url.Page("/Account/ExternalLoginCallback", pageHandler: null, values: null, protocol: Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
    }
}
