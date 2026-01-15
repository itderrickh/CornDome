using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CornDome.Models.Cards
{
    [Table("revision")]
    public class CardRevision
    {
        public int Id { get; set; }
        public int RevisionNumber { get; set; }
        public int CardId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string? Ability { get; set; }
        public int? SetId { get; set; }
        public int? LandscapeId { get; set; }
        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }

        public ICollection<CardImage> CardImages { get; set; } = [];
        [JsonIgnore]
        public Card Card { get; set; }
        public CardSet CardSet { get; set; }
        public CardType CardType { get; set; }
        public Landscape Landscape { get; set; }
        public string Rulings { get; set; } = string.Empty;

        public string GetExtraSmallImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.XSmall)?.ImageUrl ?? GetSmallImage;
            }
        }

        public string GetSmallImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Small)?.ImageUrl ?? GetRegularImage;
            }
        }

        public string GetRegularImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Regular)?.ImageUrl ?? GetLargeImage;
            }
        }

        public string GetLargeImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Large)?.ImageUrl ?? "";
            }
        }

        public string GetLargestImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Large)?.ImageUrl
                    ?? CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Regular)?.ImageUrl
                    ?? CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Small)?.ImageUrl
                    ?? CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.XSmall)?.ImageUrl ?? "";
            }
        }

        public string GetSmallestImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.XSmall)?.ImageUrl
                    ?? CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Small)?.ImageUrl
                    ?? CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Regular)?.ImageUrl
                    ?? CardImages.FirstOrDefault(x => x.CardImageTypeId == (int)CardImageTypeEnum.Large)?.ImageUrl ?? "";
            }
        }
    }
}
