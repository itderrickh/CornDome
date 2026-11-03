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
    public class IndexModel(ICardRepository cardRepository) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        [BindProperty]
        public int DeleteCardId { get; set; }

        [BindProperty]
        public int DeleteDarklandsCardId { get; set; }

        public async Task OnGetAsync()
        {
            Cards = cardRepository.GetAll();
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
    }
}