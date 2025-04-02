namespace CornDome.Models
{
    public class TournamentRegistration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Deck { get; set; }
        public int TournamentId { get; set; }
    }
}
