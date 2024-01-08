using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class CardModel(ICardRepository cardRepository) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public Card QueryCard { get; set; } = null;

        public void OnGet()
        {
            var queryId = Request.Query["id"];
            Cards = _cardRepository.GetAll();
            QueryCard = Cards.FirstOrDefault(x => x.Id == int.Parse(queryId));
        }
    }
}
