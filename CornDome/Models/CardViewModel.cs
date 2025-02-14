namespace CornDome.Models
{
    public class CardViewModel
    {
        public CardFullDetails Card { get; set; }
        public bool IsAddable { get; set; }
        public bool IsDeleteable { get; set; }
        public CardImageType CardImageType { get; set; }
    }
}
