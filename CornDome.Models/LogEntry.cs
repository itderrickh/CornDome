namespace CornDome.Models
{
    public class LogEntry
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string QueryString { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long DurationMs { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string RemoteIp { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
    }
}
