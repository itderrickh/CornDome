using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;

namespace CornDome.Pages
{
    public class CardDatabaseModel(ICardRepository cardRepository) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;

        public void OnGet()
        {
            var cards = cardRepository.GetAll();
            Cards = cards;
        }
    }
}
