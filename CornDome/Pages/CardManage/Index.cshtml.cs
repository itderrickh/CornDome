using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class IndexModel(ICardRepository cardRepository, IFeedbackRepository feedbackRepository, IUserRepository userRepository) : PageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public IEnumerable<FeedbackRequest> FeedbackRequests { get; set; }
        [BindProperty]
        public int DeleteFeedbackId { get; set; }
        [BindProperty]
        public int DeleteCardId { get; set; }

        public async void OnGet()
        {
            FeedbackRequests = await feedbackRepository.GetAllFeedback();
            Cards = cardRepository.GetAll();
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

        public async Task<IActionResult> OnPostDeleteCard()
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = cardRepository.DeleteCard(DeleteCardId);

            if (result)
                TempData["SuccessMessage"] = "Delete card succeeded";
            else
                TempData["ErrorMessage"] = "Delete card failed";

            return RedirectToPage();
        }
    }
}
