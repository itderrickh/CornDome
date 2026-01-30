using CornDome.Models.Users;

namespace CornDome.Models
{
    public class PlayAvailability
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DayOfWeek Day { get; set; }

        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public bool IsAvailable { get; set; }
    }
}
