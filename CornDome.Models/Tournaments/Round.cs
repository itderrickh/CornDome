using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Tournaments
{
    [Table("round")]
    public class Round
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int RoundNumber { get; set; }

        public virtual Tournament Tournament { get; set; }
        public virtual List<Match> Matches { get; set; }
    }
}
