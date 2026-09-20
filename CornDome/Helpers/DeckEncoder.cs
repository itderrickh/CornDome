using System.IO.Compression;

namespace CornDome.Helpers
{
    public class DeckEncoder
    {
        public static string UrlEncoder(string deck)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(deck);

            using var inputStream = new MemoryStream(inputBytes);
            using var outputStream = new MemoryStream();

            using (var gzip = new GZipStream(outputStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(gzip);
            }

            return Convert.ToBase64String(outputStream.ToArray());
        }

        public static string UrlToDeck(string url)
        {
            byte[] compressedBytes = Convert.FromBase64String(url);

            using var inputStream = new MemoryStream(compressedBytes);
            using var gzip = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();

            gzip.CopyTo(outputStream);

            var deckString = System.Text.Encoding.UTF8.GetString(outputStream.ToArray());
            return deckString;
        }
    }
}
