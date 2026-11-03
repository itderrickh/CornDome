using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Darklands
{
    public class DarklandsCardDatabaseModel(IDarklandsCardRepository darklandsRepository, AppFlagHelper appFlagHelper) : BasePageModel
    {
        public IList<DarklandsCard> Cards { get; set; } = [];
        public IList<DarklandsCardType> CardTypes { get; set; } = [];
        public IList<DarklandsOverlord> Overlords { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.IsInRole("Admin") && !appFlagHelper.IsActive("EnableDarkLands"))
                return Forbid();

            Cards = await darklandsRepository.GetDarklandsCards();
            CardTypes = await darklandsRepository.GetDarklandsCardTypes();
            Overlords = await darklandsRepository.GetDarklandsOverlords();

            return Page();
        }
    }
}