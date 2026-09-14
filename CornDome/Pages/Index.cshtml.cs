using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class IndexModel(Config config, ICardRepository cardRepository) : PageModel
    {
        public Card CardOfTheDay { get; set; }
        public string BaseUrl { get; set; } = config.BaseUrl;

        public void OnGet()
        {
            CardOfTheDay = cardRepository.GetCardOfTheDay(DateTime.Now);
        }
    }
}
