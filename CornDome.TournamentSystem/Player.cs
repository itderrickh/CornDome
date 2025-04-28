using CornDome.Models.Users;

namespace CornDome.TournamentSystem
{
    public class Player
    {
        public User User { get; set; }
        public PlayerStats Stats { get; set; }
    }

    public class PlayerStats
    {
        public int Played { get; set; } = 0;
        public double Points { get; set; } = 0;
        public List<Player> Opponents { get; set; } = [];
        public double OMW { get; set; } = 0; // Opponent Match Win %

        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Ties { get; set; } = 0;
        public bool HadBye { get; set; } = false;
    }
}
