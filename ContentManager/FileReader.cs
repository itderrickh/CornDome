namespace CornDome.ContentManager
{
    public class FileReader
    {
        public static string SafeFileRead(string safeLocation, string location)
        {
            if (File.Exists(location) && Path.GetFullPath(location).StartsWith(safeLocation, StringComparison.OrdinalIgnoreCase))
            {
                return File.ReadAllText(location);
            }

            return null;
        }
    }
}
