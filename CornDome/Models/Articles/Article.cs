using System.Text.Json.Serialization;

namespace CornDome.Models.Articles
{
    public class Article
    {
        [JsonPropertyName("title")]
        public required string Title { get; set; }
        public string Content { 
            get
            {
                return File.ReadAllText(Location);
            }
        }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        [JsonPropertyName("location")]
        public required string Location { get; set; }
        [JsonPropertyName("imagePath")]
        public required string ImagePath { get; set; }
    }
}
