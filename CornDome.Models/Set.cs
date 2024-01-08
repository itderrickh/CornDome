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
        Kickstarter = 10
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
    }
}
