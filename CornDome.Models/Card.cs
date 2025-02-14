using System.Text.Json.Serialization;

namespace CornDome.Models
{
    public class Card
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("landscape")]
        public Landscape? Landscape { get; set; }
        [JsonPropertyName("type")]
        public required CardType CardType { get; set; }
        [JsonPropertyName("attack")]
        public int? Attack {  get; set; }
        [JsonPropertyName("defense")]
        public int? Defense { get; set; }
        [JsonPropertyName("cost")]
        public int? Cost { get; set; }
        [JsonPropertyName("ability")]
        public string Ability { get; set; }
        [JsonPropertyName("imageurl")]
        public required string ImageUrl { get; set; }
        [JsonPropertyName("set")]
        public Set? Set { get; set; }
        public int? Revision { get; set; }
    }
}
