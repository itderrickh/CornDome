using CornDome.Models.Users;
using System.Text.Json.Serialization;

namespace CornDome.Models.Main
{
    public class DeckLike
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int DeckId { get; set; }
        [JsonIgnore]
        public Deck Deck { get; set; }
        public bool Liked { get; set; }
    }
}
