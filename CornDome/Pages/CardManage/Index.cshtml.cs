using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class IndexModel(ICardRepository cardRepository) : PageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public void OnGet()
        {
            Cards = cardRepository.GetAll();
        }
    }
}
