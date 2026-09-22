using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages
{
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public class CardModel(ICardRepository cardRepository, IFeedbackRepository feedbackRepository) : BasePageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public Card QueryCard { get; set; } = null;
        public int? RevisionId { get; set; } = null;

        public void OnGet()
        {
            var queryId = Request.Query["id"];

            if (int.TryParse(queryId, out int cardId))
            {
                QueryCard = _cardRepository.GetCard(cardId);
                var revisionNumber = Request.Query["revision"];
                var gotRevision = int.TryParse(revisionNumber, out int rev);
                if (gotRevision)
                    RevisionId = rev;
            }
        }

        public IActionResult OnPostFeedback([FromBody] FeedbackRequest feedbackRequest)
        {
            if (feedbackRequest == null || string.IsNullOrEmpty(feedbackRequest.Feedback))
            {
                return new JsonResult(new { success = false });
            }

            var result = feedbackRepository.AddFeedbackAsync(feedbackRequest);
            return new JsonResult(new { success = result });
        }
    }
}
