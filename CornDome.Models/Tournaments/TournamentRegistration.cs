using CornDome.Models.Users;

namespace CornDome.Models.Tournaments
{
    public class TournamentRegistration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Deck { get; set; }
        public int TournamentId { get; set; }

        public virtual User User { get; set; }
    }
}
