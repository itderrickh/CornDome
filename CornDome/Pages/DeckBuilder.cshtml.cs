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

        public void OnGet()
        {
            Cards = _cardRepository.GetAll();

            if (Request.QueryString.HasValue)
                BuildDeckFromQuery();
        }

        private void BuildDeckFromQuery()
        {
            QueryDeck = Deck.GetFromQuery(Request.Query["deck"], Cards);
        }
    }
}
