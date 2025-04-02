using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Cards
{
    [Table("cardType")]
    public class CardType
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    public enum CardTypeEnum
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
        public static string ToString(CardTypeEnum cardType)
        {
            return cardType switch
            {
                CardTypeEnum.Creature => "Creature",
                CardTypeEnum.Spell => "Spell",
                CardTypeEnum.Building => "Building",
                CardTypeEnum.Landscape => "Landscape",
                CardTypeEnum.Hero => "Hero",
                CardTypeEnum.Teamwork => "Teamwork",
                _ => "",
            };
        }
    }
}
