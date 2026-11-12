using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Cards
{
    [Table("darklands_card")]
    public class DarklandsCard
    {
        public int Id { get; set; }
        public int CardTypeId { get; set; }
        public virtual DarklandsCardType CardType { get; set; }
        public string Name { get; set; }
        public string Ability { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }
        public int? OverlordId { get; set; }
        public virtual DarklandsOverlord Overlord { get; set; }
        public virtual IList<DarklandsCardImage> Images { get; set; } = [];
    }
}
