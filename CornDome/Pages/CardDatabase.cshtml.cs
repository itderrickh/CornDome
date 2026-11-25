using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Main;
using CornDome.Repository.Cards;

namespace CornDome.Pages
{
    public class CardDatabaseModel(ICardRepository cardRepository, ICustomCardRepository customCardRepository) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public IEnumerable<CustomCard> CustomCards { get; set; }
        public IEnumerable<CardSet> Sets { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;

        public async Task OnGet()
        {
            var customCards = await customCardRepository.GetAllPublic();
            var cards = cardRepository.GetAll();
            var sets = cardRepository.GetAllSets();
            Cards = cards;
            Sets = sets;
            CustomCards = customCards;
        }
    }
}
