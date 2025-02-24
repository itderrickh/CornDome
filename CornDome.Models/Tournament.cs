namespace CornDome.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public DateTime TournamentDate { get; set; }
        public string TournamentName { get; set; }
        public string TournamentDescription { get; set; }
        public List<TournamentResult> TournamentResults { get; set; } = [];
    }
}
