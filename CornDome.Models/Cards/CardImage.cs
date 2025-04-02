using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Cards
{
    [Table("cardImage")]
    public class CardImage
    {
        public int Id { get; set; }
        public int RevisionId { get; set; }
        public int CardImageTypeId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public CardImageType CardImageType { get; set; }
        public CardRevision Revision { get; set; }
    }
}
