namespace CornDome.Models
{
    public enum Landscape
    {
        BluePlains,
        CornFields,
        UselessSwamp,
        SandyLands,
        NiceLands,
        IcyLands,
        Rainbow
    }

    public static class LandscapeConverter
    {
        public static string ToString(Landscape cardType)
        {
            return cardType switch
            {
                Landscape.BluePlains => "Blue Plains",
                Landscape.CornFields => "Cornfield",
                Landscape.UselessSwamp => "Useless Swamp",
                Landscape.SandyLands => "SandyLands",
                Landscape.NiceLands => "NiceLands",
                Landscape.IcyLands => "IcyLands",
                Landscape.Rainbow => "Rainbow",
                _ => "",
            };
        }
    }
}
