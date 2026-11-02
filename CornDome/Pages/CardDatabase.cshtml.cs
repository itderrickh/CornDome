using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository.Cards;

namespace CornDome.Pages
{
    public class CardDatabaseModel(ICardRepository cardRepository) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public IEnumerable<CardSet> Sets { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;

        public void OnGet()
        {
            var cards = cardRepository.GetAll();
            var sets = cardRepository.GetAllSets();
            Cards = cards;
            Sets = sets;
        }
    }
}
