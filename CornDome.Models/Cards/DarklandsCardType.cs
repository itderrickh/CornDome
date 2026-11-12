using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Cards
{
    [Table("darklands_cardtype")]
    public class DarklandsCardType
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public enum DarklandsCardTypeEnum
    {
        Creature,
        Overlord,
        Curse,
        Barrier,
        DifficultySetting,
        Landscape
    }

    public static class DarklandsCardTypeConverter
    {
        public static string ToString(DarklandsCardTypeEnum cardType)
        {
            return cardType switch
            {
                DarklandsCardTypeEnum.Creature => "Creature",
                DarklandsCardTypeEnum.Overlord => "Overlord",
                DarklandsCardTypeEnum.Curse => "Curse",
                DarklandsCardTypeEnum.Barrier => "Barrier",
                DarklandsCardTypeEnum.DifficultySetting => "Difficulty Setting",
                DarklandsCardTypeEnum.Landscape => "Landscape",
                _ => "",
            };
        }
    }
}
