using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class EditModel(ICardRepository cardRepository) : PageModel
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

        public IActionResult OnPost()
        {
            var isSuccess = false;
            var card = cardRepository.GetCard(EditCard.Id);

            if (card != null && card.Id == EditCard.Id)
            {
                isSuccess = cardRepository.UpdateCardAndRevisions(EditCard);
            }

            if (isSuccess)
                TempData["SuccessMessage"] = "Card saved successfully.";
            else
                TempData["ErrorMessage"] = "There was an issue saving the card.";

            EditCard = cardRepository.GetCard(CardId);
            return Page();
        }
    }
}
