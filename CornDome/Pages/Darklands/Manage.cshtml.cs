using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Darklands
{
    [Authorize(Policy = "cardManager")]
    public class ManageModel(IDarklandsCardRepository darklandsRepository) : BasePageModel
    {
        public IEnumerable<DarklandsCard> DarklandsCards { get; set; } = [];
        [BindProperty]
        public int DeleteDarklandsCardId { get; set; }

        public async Task OnGet()
        {
            DarklandsCards = await darklandsRepository.GetDarklandsCards();
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
