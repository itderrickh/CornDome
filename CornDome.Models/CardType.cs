namespace CornDome.Models
{
    public enum CardType
    {
        Creature,
        Spell,
        Building,        
        Landscape,
        Hero,
        Teamwork
    }

    public static class CardTypeConverter
    {
        public static string ToString(CardType cardType)
        {
            return cardType switch
            {
                CardType.Creature => "Creature",
                CardType.Spell => "Spell",
                CardType.Building => "Building",
                CardType.Landscape => "Landscape",
                CardType.Hero => "Hero",
                CardType.Teamwork => "Teamwork",
                _ => "",
            };
        }
    }
}
