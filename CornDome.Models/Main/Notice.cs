namespace CornDome.Models.Main
{
    public class Notice
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }

        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
