using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class IndexModel(
        ICardRepository cardRepository,
        IFeedbackRepository feedbackRepository,
        IDarklandsCardRepository darklandsRepository) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; } = [];
        public IEnumerable<FeedbackRequest> FeedbackRequests { get; set; } = [];
        public IEnumerable<DarklandsCard> DarklandsCards { get; set; } = [];

        [BindProperty]
        public int DeleteFeedbackId { get; set; }
        [BindProperty]
        public int DeleteCardId { get; set; }

        [BindProperty]
        public int DeleteDarklandsCardId { get; set; }

        public async Task OnGetAsync()
        {
            FeedbackRequests = await feedbackRepository.GetAllFeedback();
            Cards = cardRepository.GetAll();
            DarklandsCards = await darklandsRepository.GetDarklandsCards();
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
                return Forbid();

            var result = cardRepository.DeleteCard(DeleteCardId);

            if (result)
                TempData["SuccessMessage"] = "Delete card succeeded";
            else
                TempData["ErrorMessage"] = "Delete card failed";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteDarklandsCard()
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            var result = await darklandsRepository.DeleteDarklandsCard(DeleteDarklandsCardId);

            if (result)
                TempData["SuccessMessage"] = "Delete Darklands card succeeded";
            else
                TempData["ErrorMessage"] = "Delete Darklands card failed";

            return RedirectToPage();
        }
    }
}