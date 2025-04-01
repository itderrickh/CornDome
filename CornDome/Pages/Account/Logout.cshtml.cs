using CornDome.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Account
{
    public class LogoutModel(SignInManager<User> signInManager) : PageModel
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
