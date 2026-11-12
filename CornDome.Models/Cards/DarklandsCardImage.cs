using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CornDome.Models.Cards
{
    [Table("darklands_cardimage")]
    public class DarklandsCardImage
    {
        public int Id { get; set; }
        public int CardImageTypeId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public CardImageType CardImageType { get; set; }
        public int CardId { get; set; }
        [JsonIgnore]
        public virtual DarklandsCard Card { get; set; }
    }
}
