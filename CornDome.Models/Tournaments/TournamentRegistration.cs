using CornDome.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Tournaments
{
    [Table("registration")]
    public class TournamentRegistration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Deck { get; set; }
        public int TournamentId { get; set; }

        public virtual User User { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}
