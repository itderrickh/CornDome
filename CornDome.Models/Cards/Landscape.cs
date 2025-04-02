using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Cards
{
    [Table("landscape")]
    public class Landscape
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public enum LandscapeEnum
    {
        BluePlains,
        CornFields,
        UselessSwamp,
        SandyLands,
        NiceLands,
        IcyLands,
        Rainbow,
        LavaFlats,
    }

    public static class LandscapeConverter
    {
        public static string ToString(LandscapeEnum cardType)
        {
            return cardType switch
            {
                LandscapeEnum.BluePlains => "Blue Plains",
                LandscapeEnum.CornFields => "Cornfield",
                LandscapeEnum.UselessSwamp => "Useless Swamp",
                LandscapeEnum.SandyLands => "SandyLands",
                LandscapeEnum.NiceLands => "NiceLands",
                LandscapeEnum.IcyLands => "IcyLands",
                LandscapeEnum.Rainbow => "Rainbow",
                LandscapeEnum.LavaFlats => "LavaFlats",
                _ => "",
            };
        }
    }
}
