using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;

namespace CornDome.Pages
{
    public class CardDatabaseModel(ICardRepository cardRepository) : BasePageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;

        public void OnGet()
        {
            var cards = _cardRepository.GetAll();
            Cards = cards;
        }
    }
}
