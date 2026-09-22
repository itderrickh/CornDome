using CornDome.Helpers;
using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Models
{
    public class BasePageModel : PageModel
    {
        private Config Config => HttpContext.RequestServices.GetRequiredService<Config>();
        private IUserRepository UserRepository => HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        public string BaseUrl => Config.BaseUrl;

        protected async Task<User> GetUser()
        {
            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await UserRepository.GetUserById(int.Parse(identifier));
            return loggedInUser;
        }
    }
}
