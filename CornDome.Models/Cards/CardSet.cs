using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CornDome.Models.Cards
{
    [Table("set")]
    public class CardSet
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual ICollection<CardRevision> Revisions { get; set; } = [];
    }
}
