using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Darklands
{
    public class CardModel(IDarklandsCardRepository darklandsRepository) : BasePageModel
    {
        public DarklandsCard QueryCard { get; set; } = null;

        [BindProperty(Name = "id", SupportsGet = true)]
        public int? CardId { get; set; }

        public async Task OnGetAsync()
        {
            if (CardId.HasValue)
            {
                QueryCard = await darklandsRepository.GetDarklandsCard(CardId.Value);
            }
        }
    }
}