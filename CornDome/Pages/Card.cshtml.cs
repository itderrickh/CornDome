using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    public class CardModel(ICardRepository cardRepository, IFeedbackRepository feedbackRepository, Config config) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public Card QueryCard { get; set; } = null;
        public int? RevisionId { get; set; } = null;
        public string BaseUrl { get; set; } = config.BaseUrl;

        public void OnGet()
        {
            var queryId = Request.Query["id"];
            QueryCard = _cardRepository.GetCard(int.Parse(queryId));

            var revisionNumber = Request.Query["revision"];
            var gotRevision = int.TryParse(revisionNumber, out int rev);
            if (gotRevision)
                RevisionId = rev;
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
