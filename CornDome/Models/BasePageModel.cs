using CornDome.Helpers;
using CornDome.Models.Users;
using CornDome.Repository.Users;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Models
{
    public abstract class BasePageModel : PageModel
    {
        private Config Config => HttpContext.RequestServices.GetRequiredService<Config>();
        private IUserRepository UserRepository => HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        public string BaseUrl => Config.BaseUrl;

        public async Task<User> GetUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var loggedInUser = await UserRepository.GetUserById(int.Parse(identifier));
                return loggedInUser;
            }

            return null;
        }
    }
}
