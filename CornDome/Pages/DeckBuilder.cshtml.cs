using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class DeckBuilderModel(ICardRepository cardRepository, Config config) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public Deck QueryDeck { get; set; } = null;
        public string BaseUrl { get; set; } = config.BaseUrl;
        public bool QueryBuildFailed { get; set; } = false;

        public void OnGet()
        {
            Cards = _cardRepository.GetAll();

            try
            {
                if (Request.QueryString.HasValue)
                {
                    BuildDeckFromQuery();
                }
            }
            catch (Exception ex)
            {
                QueryBuildFailed = true;
            }
        }

        private void BuildDeckFromQuery()
        {
            var nonGZDeck = Request.Query["deck"];
            var gzDeck = Request.Query["gzdeck"];

            if (string.IsNullOrEmpty(nonGZDeck))
            {
                QueryDeck = Deck.GetDeckFromGzip(gzDeck, Cards);
            }
            else
            {
                QueryDeck = Deck.GetFromQuery(nonGZDeck, Cards);
            }
        }
    }
}
