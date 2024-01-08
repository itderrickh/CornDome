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
            switch (cardType)
            {
                case Landscape.BluePlains:
                    return "Blue Plains";
                case Landscape.CornFields:
                    return "Cornfield";
                case Landscape.UselessSwamp:
                    return "Useless Swamp";
                case Landscape.SandyLands:
                    return "SandyLands";
                case Landscape.NiceLands:
                    return "NiceLands";
                case Landscape.IcyLands:
                    return "IcyLands";
                case Landscape.Rainbow:
                    return "Rainbow";
                default:
                    return "";
            }
        }
    }
}
