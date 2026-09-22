using CornDome.Models.Users;

namespace CornDome.Models
{
    public enum DeckVisibility
    {
        Visible,
        Limited,
        Invisible
    }

    public class Deck
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public string DeckString { get; set; }
        public int IconCardId { get; set; }
        public string Description { get; set; }
        public string DeckName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public DeckVisibility Visibility { get; set; }

        public virtual List<DeckComment> DeckComments { get; set; }
    }
}
