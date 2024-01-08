namespace CornDome.Models.Articles
{
    public class Article
    {
        public required string Title { get; set; }
        public string Content { 
            get
            {
                return File.ReadAllText(Location);
            }
        }
        public required DateTime CreatedDate { get; set; }
        public required DateTime UpdatedDate { get; set; }
        public required string Location { get; set; }
        public required string ImagePath { get; set; }
    }
}
