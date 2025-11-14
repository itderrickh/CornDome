using System.ComponentModel.DataAnnotations.Schema;

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

    [Table("tournament")]
    public class Tournament
    {
        public int Id { get; set; }
        public DateTime TournamentDate { get; set; }
        public DateTime ActualStart { get; set; }
        public TimeSpan RoundDuration { get; set; }
        public string TournamentName { get; set; }
        public string TournamentDescription { get; set; }
        public TournamentStatus Status { get; set; }
        public bool IsOpenList { get; set; }

        public virtual List<Round> Rounds { get; set; }
        public virtual List<TournamentRegistration> Registrations { get; set; }
    }
}
