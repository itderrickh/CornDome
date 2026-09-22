using CornDome.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace CornDome.Models
{
    public class DeckComment
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int DeckId { get; set; }
        public Deck Deck { get; set; }
        public bool IsUpvoted { get; set; }
        [MaxLength(400)]
        public string CommentText { get; set; }
    }
}
