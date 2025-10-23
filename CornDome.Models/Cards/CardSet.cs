using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Cards
{
    [Table("set")]
    public class CardSet
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public enum Set
    {
        FinnVSJake = 0,
        BMOVSLadyRainicorn = 1,
        PrincessBubblegumVSLumpySpacePrincess = 2,
        IceKingVSMarceline = 3,
        LemonGrabVSGunter = 4,
        FionnaVSCake = 5,
        DoublesTournament = 6,
        HeroPack = 7,
        ForTheGlory = 8,
        Promo = 9,
        Kickstarter1 = 10,
        FlamePrincessVSFern = 11,
        PrismoVSTheLich = 12,
        PeppermintButlerVSMagicMan = 13,
        Kickstarter2 = 14,
        DarklandsExpansion = 15,
        CustomCards = 16,
        LandOfLegends = 17,
    }

    public static class SetConverter
    {
        public static string ToString(Set set)
        {
            return set switch
            {
                Set.FinnVSJake => "Finn Vs Jake",
                Set.BMOVSLadyRainicorn => "BMO Vs Lady Rainicorn",
                Set.PrincessBubblegumVSLumpySpacePrincess => "Princess Bubblegum Vs Lumpy Space Princess",
                Set.IceKingVSMarceline => "Ice King Vs Marceline",
                Set.LemonGrabVSGunter => "Lemon Grab Vs Gunter",
                Set.FionnaVSCake => "Fionna Vs Cake",
                Set.DoublesTournament => "Doubles Tournament",
                Set.HeroPack => "Hero Pack",
                Set.ForTheGlory => "For The Glory",
                Set.Promo => "Promo",
                Set.Kickstarter1 => "Kickstarter #1",
                Set.FlamePrincessVSFern => "Flame Princess Vs Fern",
                Set.PrismoVSTheLich => "Prismo Vs The Lich",
                Set.PeppermintButlerVSMagicMan => "Peppermint Butler Vs Magic Man",
                Set.Kickstarter2 => "Kickstarter #2",
                Set.DarklandsExpansion => "DarkLands Expansion",
                Set.CustomCards => "Custom Community Cards",
                Set.LandOfLegends => "Land of Legends",
                _ => "",
            };
        }

        public static string UntapSetCode(Set set)
        {
            return set switch
            {
                Set.FinnVSJake => "cp1",
                Set.BMOVSLadyRainicorn => "cp2",
                Set.PrincessBubblegumVSLumpySpacePrincess => "cp3",
                Set.IceKingVSMarceline => "cp4",
                Set.LemonGrabVSGunter => "cp5",
                Set.FionnaVSCake => "cp6",
                Set.DoublesTournament => "2v2",
                Set.HeroPack => "hp1",
                Set.ForTheGlory => "ftg",
                Set.Promo => "promo",
                Set.Kickstarter1 => "ks1",
                Set.FlamePrincessVSFern => "cp7",
                Set.PrismoVSTheLich => "cp8",
                Set.PeppermintButlerVSMagicMan => "cp9",
                Set.Kickstarter2 => "ks2",
                Set.DarklandsExpansion => "dl1",
                Set.CustomCards => "custom",
                Set.LandOfLegends => "lol",
                _ => "",
            };
        }
    }
}
