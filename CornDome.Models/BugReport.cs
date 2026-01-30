using CornDome.Models.Users;

namespace CornDome.Models
{
    public class BugReport
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Steps { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
