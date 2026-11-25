using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages
{
    public class GoldfishModel(ICardRepository cardRepository, IDeckRepository deckRepository, ICustomCardRepository customCardRepository) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public IEnumerable<CustomCard> CustomCards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;
        [BindProperty(Name = "id", SupportsGet = true)]
        public int? DeckId { get; set; }

        [BindProperty(Name = "gzdeck", SupportsGet = true)]
        public string GzDeck { get; set; }

        [BindProperty(Name = "deck", SupportsGet = true)]
        public string NonGZDeck { get; set; }

        public async Task OnGet()
        {
            Cards = cardRepository.GetAll();
            CustomCards = await customCardRepository.GetAllPublic();

            await BuildDeckFromQuery();
        }

        private async Task BuildDeckFromQuery()
        {
            if (DeckId.HasValue)
            {
                var loggedInUser = await GetUser();
                if (loggedInUser != null && deckRepository.DoesUserHaveAccess(DeckId.Value, loggedInUser.Id))
                {
                    Deck deck = deckRepository.GetDeck(DeckId.Value);
                    if (deck != null)
                    {
                        QueryDeck = QueryDeck.GetFromString(deck.DeckString, Cards, CustomCards);
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(GzDeck))
            {
                QueryDeck = QueryDeck.GetDeckFromGzip(GzDeck, Cards, CustomCards);
            }
            else if (!string.IsNullOrWhiteSpace(NonGZDeck))
            {
                QueryDeck = QueryDeck.GetFromQuery(NonGZDeck, Cards, CustomCards);
            }
        }
    }
}
