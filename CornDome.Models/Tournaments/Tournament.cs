namespace CornDome.Models.Tournaments
{
    public enum TournamentStatus
    {
        NotActive = 0,
        OpenForSignups = 1,
        ClosedForSignups = 2,
        Ongoing = 3,
        Completed = 4
    }

    public class Tournament
    {
        public int Id { get; set; }
        public DateTime TournamentDate { get; set; }
        public string TournamentName { get; set; }
        public string TournamentDescription { get; set; }
        public TournamentStatus Status { get; set; }
        public List<TournamentResult> TournamentResults { get; set; } = [];
    }
}
