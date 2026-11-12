using CornDome.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CornDome.Models.Main
{
    public class DeckComment
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int DeckId { get; set; }
        [JsonIgnore]
        public Deck Deck { get; set; }
        [MaxLength(400)]
        public string CommentText { get; set; }
    }
}
