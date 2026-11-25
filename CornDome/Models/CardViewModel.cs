using CornDome.Models.Cards;

namespace CornDome.Models
{
    public class CardViewModel
    {
        public DisplayCardModel Card { get; set; }
        public bool IsAddable { get; set; }
        public bool IsDeleteable { get; set; }
        public CardImageTypeEnum CardImageType { get; set; }
        public bool Link { get; set; } = false;
    }  
}
