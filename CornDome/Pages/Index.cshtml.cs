using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;

namespace CornDome.Pages
{
    public class IndexModel(ICardRepository cardRepository) : BasePageModel
    {
        public Card CardOfTheDay { get; set; }

        public void OnGet()
        {
            CardOfTheDay = cardRepository.GetCardOfTheDay(DateTime.Now);
        }
    }
}
