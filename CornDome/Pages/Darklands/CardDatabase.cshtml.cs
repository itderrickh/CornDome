using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Darklands
{
    public class DarklandsCardDatabaseModel(IDarklandsCardRepository darklandsRepository) : BasePageModel
    {
        public IList<DarklandsCard> Cards { get; set; } = [];
        public IList<DarklandsCardType> CardTypes { get; set; } = [];
        public IList<DarklandsOverlord> Overlords { get; set; } = [];

        public async Task OnGetAsync()
        {
            Cards = await darklandsRepository.GetDarklandsCards();
            CardTypes = await darklandsRepository.GetDarklandsCardTypes();
            Overlords = await darklandsRepository.GetDarklandsOverlords();
        }
    }
}