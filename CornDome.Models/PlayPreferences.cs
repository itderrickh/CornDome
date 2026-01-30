using CornDome.Models.Users;

namespace CornDome.Models
{
    public class PlayPreferences
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string TimeZone { get; set; }
        public string GameFormat { get; set; }
        public string Pronouns { get; set; }
        public string AddressMeAs { get; set; }
        public string Platform { get; set; }
        public bool WantsToPlay { get; set; }
    }
}
