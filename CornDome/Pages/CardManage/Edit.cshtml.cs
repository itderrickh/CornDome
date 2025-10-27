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
            var card = cardRepository.GetCard(CardId);

            if (card != null)
            {
                // Do the card update 
            }

            EditCard = cardRepository.GetCard(CardId);
            return Page();
        }
    }
}
