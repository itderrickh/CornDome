using CornDome.Helpers;
using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class GoldfishModel(ICardRepository cardRepository) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<CardFullDetails> Cards { get; set; }
        public Deck QueryDeck { get; set; } = null;

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
