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

        [BindProperty(Name = "id", SupportsGet = true)]
        public int? CardId { get; set; }
        [BindProperty(Name = "revision", SupportsGet = true)]
        public int? RevisionId { get; set; }

        public void OnGet()
        {
            if (CardId.HasValue)
            {
                QueryCard = _cardRepository.GetCard(CardId.Value);
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
