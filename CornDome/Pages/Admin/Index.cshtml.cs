using CornDome.Models;
using CornDome.Models.Logging;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    [Authorize]
    public class IndexModel(IUserRepository userRepository, ILoggingRepository loggingRepository) : PageModel
    {
        public User LoggedInUser { get; set; }
        public IEnumerable<RouteLog> RouteLogs { get; set; }
        public async void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                LoggedInUser = userRepository.GetUserByUsername(User.Identity.Name);
            }

            RouteLogs = await loggingRepository.GetAllRouteLogs();
        }
    }
}
