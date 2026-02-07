using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class CardDatabaseModel(ICardRepository cardRepository, Config config) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public Deck QueryDeck { get; set; } = null;
        public string BaseUrl { get; set; } = config.BaseUrl;

        public void OnGet()
        {
            var cards = _cardRepository.GetAll();
            Cards = cards;
        }
    }
}
