using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class IndexModel(ICardRepository cardRepository, IFeedbackRepository feedbackRepository) : PageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public List<FeedbackRequest> FeedbackRequests { get; set; }
        [BindProperty]
        public int DeleteFeedbackId { get; set; }

        public void OnGet()
        {
            FeedbackRequests = feedbackRepository.GetAllFeedback();
            Cards = cardRepository.GetAll();
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
