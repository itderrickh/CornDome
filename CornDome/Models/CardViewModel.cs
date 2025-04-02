using CornDome.Models.Cards;

namespace CornDome.Models
{
    public class CardViewModel
    {
        public Card Card { get; set; }
        public bool IsAddable { get; set; }
        public bool IsDeleteable { get; set; }
        public CardImageTypeEnum CardImageType { get; set; }
    }
}
