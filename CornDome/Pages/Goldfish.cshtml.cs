using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages
{
    public class GoldfishModel(ICardRepository cardRepository, IDeckRepository deckRepository) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; }
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

            await BuildDeckFromQuery();
        }

        private async Task BuildDeckFromQuery()
        {
            if (DeckId.HasValue)
            {
                var loggedInUser = await GetUser();
                if (deckRepository.DoesUserHaveAccess(DeckId.Value, loggedInUser.Id))
                {
                    Deck deck = deckRepository.GetDeck(DeckId.Value);
                    if (deck != null)
                    {
                        QueryDeck = QueryDeck.GetFromString(deck.DeckString, Cards);
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(GzDeck))
            {
                QueryDeck = QueryDeck.GetDeckFromGzip(GzDeck, Cards);
            }
            else if (!string.IsNullOrWhiteSpace(NonGZDeck))
            {
                QueryDeck = QueryDeck.GetFromQuery(NonGZDeck, Cards);
            }
        }
    }
}
