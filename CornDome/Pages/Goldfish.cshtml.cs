using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages
{
    public class GoldfishModel(ICardRepository cardRepository, Config config, IDeckRepository deckRepository, IUserRepository userRepository) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;
        public string BaseUrl { get; set; } = config.BaseUrl;
        [BindProperty(Name = "id", SupportsGet = true)]
        public int? DeckId { get; set; }

        [BindProperty(Name = "gzdeck", SupportsGet = true)]
        public string GzDeck { get; set; }

        [BindProperty(Name = "deck", SupportsGet = true)]
        public string NonGZDeck { get; set; }

        public async Task OnGet()
        {
            Cards = _cardRepository.GetAll();

            await BuildDeckFromQuery();
        }

        private async Task BuildDeckFromQuery()
        {
            if (DeckId.HasValue)
            {
                var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));
                Deck deck = null;

                if (deckRepository.DoesUserHaveAccess(DeckId.Value, loggedInUser.Id))
                {
                    deck = deckRepository.GetDeck(DeckId.Value);

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
