using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Tournaments
{
    public enum MatchResult
    {
        Incomplete = 0,
        Player1Wins = 1,
        Player2Wins = 2,
        Tie = 3,
        Bye = 4
    }

    [Table("match")]
    public class Match
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int RoundId { get; set; }
        public int Player1Id { get; set; }
        public int? Player2Id { get; set; }
        public MatchResult Result { get; set; }

        public virtual Round Round { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
