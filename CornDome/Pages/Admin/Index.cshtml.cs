using CornDome.Models;
using CornDome.Models.Logging;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    [Authorize]
    public class IndexModel(IUserRepository userRepository, ILoggingRepository loggingRepository, IFeedbackRepository feedbackRepository) : PageModel
    {
        public User LoggedInUser { get; set; }
        public IEnumerable<RouteLog> RouteLogs { get; set; }
        public List<FeedbackRequest> FeedbackRequests { get; set; }
        [BindProperty]
        public int DeleteFeedbackId { get; set; }

        public async void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                LoggedInUser = userRepository.GetUserByUsername(User.Identity.Name);
            }

            RouteLogs = await loggingRepository.GetAllRouteLogs();
            FeedbackRequests = feedbackRepository.GetAllFeedback();
        }

        public IActionResult OnPostDeleteFeedback()
        {
            var result = feedbackRepository.DeleteFeedback(DeleteFeedbackId);

            if (result == 1)
                TempData["SuccessMessage"] = "Delete feedback succeeded";
            else
                TempData["ErrorMessage"] = "Delete feedback failed";

            return RedirectToPage();
        }
    }
}
