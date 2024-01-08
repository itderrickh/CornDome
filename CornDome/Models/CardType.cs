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
            switch (cardType)
            {
                case CardType.Creature:
                    return "Creature";
                case CardType.Spell:
                    return "Spell";
                case CardType.Building:
                    return "Building";
                case CardType.Landscape:
                    return "Landscape";
                case CardType.Hero:
                    return "Hero";
                case CardType.Teamwork:
                    return "Teamwork";
                default:
                    return "";
            }
        }
    }
}
