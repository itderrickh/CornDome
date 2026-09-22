using CornDome.Models;
using CornDome.Models.Cards;

namespace CornDome.Pages.Decks
{
    public class DeckGridViewModel
    {
        public List<Deck> Decks { get; set; } = [];
        public List<Card> Cards { get; set; } = [];

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public string BaseUrl { get; set; } = "";
        public int UserId { get; set; }
    }
}
