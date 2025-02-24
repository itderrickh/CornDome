namespace CornDome.Models
{
    public enum CardImageType
    {
        XSmall = 0,
        Small = 1,
        Regular = 2,
        Large = 3
    }

    public class CardImage
    {
        public int Id { get; set; }
        public int RevisionId { get; set; }
        public CardImageType CardImageType { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CardRevision
    {
        public int Id { get; set; }
        public int RevisionNumber { get; set; }
        public int CardId { get; set; }
        public string Name { get; set; }
        public CardType CardType { get; set; }
        public string Ability { get; set; }
        public Landscape? Landscape { get; set; }
        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }
        public Set? CardSet { get; set; }
        public List<CardImage> CardImages { get; set; } = [];

        public string GetExtraSmallImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageType == CardImageType.XSmall)?.ImageUrl ?? GetSmallImage;
            }
        }

        public string GetSmallImage
        { 
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageType == CardImageType.Small)?.ImageUrl ?? GetRegularImage;
            }
        }

        public string GetRegularImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageType == CardImageType.Regular)?.ImageUrl ?? GetLargeImage;
            }
        }

        public string GetLargeImage
        {
            get
            {
                return CardImages.FirstOrDefault(x => x.CardImageType == CardImageType.Large)?.ImageUrl ?? "";
            }
        }
    }

    public class CardFullDetails
    {
        public int Id { get; set; }
        public List<CardRevision> Revisions { get; set; }

        public CardRevision LatestRevision
        {
            get
            {
                return Revisions
                    .OrderByDescending(x => x.RevisionNumber)
                    .FirstOrDefault();
            }
        }
    }
}
