using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "rulingManager")]
    public class RulingsModel(ICardRepository cardRepository) : PageModel
    {
        [BindProperty]
        public int CardId { get; set; }
        [BindProperty]
        public Card EditCard { get; set; }

        public void OnGet()
        {
            var queryId = Request.Query["id"];
            CardId = int.Parse(queryId);

            EditCard = cardRepository.GetCard(CardId);
        }

        public async Task<IActionResult> OnPost()
        {
            var isSuccess = false;
            var card = cardRepository.GetCard(CardId);

            if (card != null && card.Id == CardId)
            {
                isSuccess = cardRepository.UpdateRevisionRulings(EditCard);
                isSuccess = true;
            }

            if (isSuccess)
                TempData["SuccessMessage"] = "Card ruling saved successfully.";
            else
                TempData["ErrorMessage"] = "There was an issue saving the card ruling.";

            EditCard = cardRepository.GetCard(CardId);
            return RedirectToPage(new { id = CardId });
        }

    }
}
