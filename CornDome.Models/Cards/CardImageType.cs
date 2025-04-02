using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Cards
{
    public enum CardImageTypeEnum
    {
        XSmall = 0,
        Small = 1,
        Regular = 2,
        Large = 3
    }

    [Table("cardImageType")]
    public class CardImageType
    {
        public int Id { get; set; }
        public string Descriptor { get; set; } = string.Empty;
    }
}
