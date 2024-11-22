using CornDome.Helpers;
using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class GoldfishModel(ICardRepository cardRepository) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public Deck QueryDeck { get; set; } = null;

        public List<Card> Hand { get; set; } = new List<Card>();
        public Stack<Card> RemainingDeck { get; set; }

        public void OnGet()
        {
            Cards = _cardRepository.GetAll();

            if (Request.QueryString.HasValue)
                BuildDeckFromQuery();
        }

        private void BuildDeckFromQuery()
        {
            QueryDeck = Deck.GetFromQuery(Request.Query["deck"], Cards);

            RemainingDeck = new Stack<Card>(QueryDeck.Cards.Shuffle());

            for (int i = 0; i < 5; i++)
            {
                Hand.Add(RemainingDeck.Pop());
            }
        }
    }
}
