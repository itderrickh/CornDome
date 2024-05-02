namespace CornDome.Models
{
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
        Kickstarter = 10,
        CommunityCards = 11
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
                Set.Kickstarter => "Kickstarter",
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
                Set.Kickstarter => "ks1",
                _ => "",
            };
        }
    }
}
