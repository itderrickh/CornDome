using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Main
{
    [Table("CardFeedback")]
    public class FeedbackRequest
    {
        public string Feedback { get; set; }
        public int CardId { get; set; }
        public int RevisionId { get; set; }
        public int Id { get; set; }
    }
}
