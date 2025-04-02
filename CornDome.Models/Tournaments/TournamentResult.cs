namespace CornDome.Models.Tournaments
{
    public class TournamentResult
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int Placement { get; set; }
        public string Name { get; set; }
        public string Decklist { get; set; }
    }
}
