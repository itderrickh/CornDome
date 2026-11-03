using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class FeedbackModel(IFeedbackRepository feedbackRepository) : BasePageModel
    {
        public IEnumerable<FeedbackRequest> FeedbackRequests { get; set; }
        [BindProperty]
        public int DeleteFeedbackId { get; set; }

        public async Task OnGet()
        {
            FeedbackRequests = await feedbackRepository.GetAllFeedback();
        }

        public async Task<IActionResult> OnPostDeleteFeedback()
        {
            var result = await feedbackRepository.DeleteFeedback(DeleteFeedbackId);

            if (result)
                TempData["SuccessMessage"] = "Delete feedback succeeded";
            else
                TempData["ErrorMessage"] = "Delete feedback failed";

            return RedirectToPage();
        }
    }
}
