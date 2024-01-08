namespace CornDome
{
    public class Branding
    {
        public string Title { get; set; }
    }

    public class AppData
    {
        public string ImagePath { get; set; }
        public string DataPath { get; set; }
    }

    public class ContentStore
    {
        public string Articles { get; set; }
        public string Images { get; set; }
    }

    public class Config
    {
        public Branding Branding { get; set; }
        public AppData AppData { get; set; }
        public ContentStore ContentStore { get; set; }
    }
}
