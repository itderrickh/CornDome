using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class GoldfishModel(ICardRepository cardRepository, Config config) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;
        public string BaseUrl { get; set; } = config.BaseUrl;

        public void OnGet()
        {
            Cards = _cardRepository.GetAll();

            if (Request.QueryString.HasValue)
                BuildDeckFromQuery();
        }

        private void BuildDeckFromQuery()
        {
            var nonGZDeck = Request.Query["deck"];
            var gzDeck = Request.Query["gzdeck"];

            if (string.IsNullOrEmpty(nonGZDeck))
            {
                QueryDeck = QueryDeck.GetDeckFromGzip(gzDeck, Cards);
            }
            else
            {
                QueryDeck = QueryDeck.GetFromQuery(nonGZDeck, Cards);
            }
        }
    }
}
