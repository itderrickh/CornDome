using CornDome.Models.Cards;
using CornDome.Models.Main;

namespace CornDome.Models
{
    public class CardViewModel
    {
        public DisplayCardModel Card { get; set; }
        public bool IsAddable { get; set; }
        public bool IsDeleteable { get; set; }
        public CardImageTypeEnum CardImageType { get; set; }
        public string BaseUrl { get; set; }
    }

    public class DisplayCardModel
    {
        public string Id { get; set; }
        public bool IsCustomCard { get; set; }

        public string Name { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string Ability { get; set; } = string.Empty;
        public int? LandscapeId { get; set; }
        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }
        public ICollection<CardSet> CardSets { get; set; } = [];
        public string Image { get; set; }

        public DisplayCardModel(Card card)
        {
            Id = card.Id.ToString();
            IsCustomCard = false;
            Name = card.Name;
            TypeId = card.TypeId;
            Ability = card.Ability;
            LandscapeId = card.LandscapeId;
            Cost = card.Cost;
            Attack = card.Attack;
            Defense = card.Defense;
            CardSets = card.CardSets;

            var cardImage = card.GetOptimizedImage;
            Image = cardImage;
        }

        public DisplayCardModel(Card card, CardImageTypeEnum size)
        {
            Id = card.Id.ToString();
            IsCustomCard = false;
            Name = card.Name;
            TypeId = card.TypeId;
            Ability = card.Ability;
            LandscapeId = card.LandscapeId;
            Cost = card.Cost;
            Attack = card.Attack;
            Defense = card.Defense;
            CardSets = card.CardSets;

            var cardImage = card.GetOptimizedImage;

            if (size == CardImageTypeEnum.XSmall)
                cardImage = card.GetExtraSmallImage;
            else if (size == CardImageTypeEnum.Small)
                cardImage = card.GetSmallImage;
            else if (size == CardImageTypeEnum.Large)
                cardImage = card.GetLargeImage;
            Image = cardImage;
        }

        public DisplayCardModel(CustomCard customCard)
        {
            Id = $"c{customCard.Id}";
            IsCustomCard = true;
            Name = customCard.Name;
            Ability = customCard.Ability;
            Cost = customCard.Cost;
            Attack = customCard.Attack;
            Defense = customCard.Defense;
            LandscapeId = customCard.Landscape;
            TypeId = customCard.CardType;
            Image = customCard.ImageUrl;
        }
    }
}
