using CornDome.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private string UserAgent => Request.Headers.UserAgent.ToString();
    }

    [Authorize]
    public abstract class AuthorizedBaseController(UserManager<User> userManager) : BaseController
    {
        protected async Task<User> GetUser()
        {
            return await userManager.GetUserAsync(User);
        }
    }
}
