using CornDome.Models;
using CornDome.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Account
{
    public class LogoutModel(SignInManager<User> signInManager) : BasePageModel
    {
        private readonly SignInManager<User> _signInManager = signInManager;
        public async Task<IActionResult> OnGetAsync()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
