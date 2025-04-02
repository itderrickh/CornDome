using CornDome.Models;
using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class IndexModel(IUserRepository userRepository, IFeedbackRepository feedbackRepository) : PageModel
    {
        public User LoggedInUser { get; set; }
        public List<FeedbackRequest> FeedbackRequests { get; set; }
        [BindProperty]
        public int DeleteFeedbackId { get; set; }

        public async void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                LoggedInUser = userRepository.GetUserByEmail(userEmail);
            }

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
