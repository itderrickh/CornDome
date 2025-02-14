using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class CardModel(ICardRepository cardRepository) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public CardFullDetails QueryCard { get; set; } = null;
        public int? RevisionId { get; set; } = null;

        public void OnGet()
        {
            var queryId = Request.Query["id"];
            QueryCard = _cardRepository.GetCard(int.Parse(queryId));

            var revisionNumber = Request.Query["revision"];
            var gotRevision = int.TryParse(revisionNumber, out int rev);
            if (gotRevision)
                RevisionId = rev;
        }
    }
}
