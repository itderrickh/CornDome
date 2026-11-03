using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Darklands
{
    public class CardModel(IDarklandsCardRepository darklandsRepository, AppFlagHelper appFlagHelper) : BasePageModel
    {
        public DarklandsCard QueryCard { get; set; } = null;

        [BindProperty(Name = "id", SupportsGet = true)]
        public int? CardId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.IsInRole("Admin") && !appFlagHelper.IsActive("EnableDarkLands"))
                return Forbid();

            if (CardId.HasValue)
            {
                QueryCard = await darklandsRepository.GetDarklandsCard(CardId.Value);
            }

            return Page();
        }
    }
}