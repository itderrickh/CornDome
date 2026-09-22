using CornDome.Models.Cards;
using Microsoft.AspNetCore.WebUtilities;
using System.IO.Compression;
using System.Text;

namespace CornDome.Models
{
    public class QueryDeck
    {
        public Card Hero { get; set; }
        public List<Card> Landscapes { get; set; } = [];
        public List<Card> Cards { get; set; } = [];

        public static QueryDeck GetDeckFromGzip(string query, IEnumerable<Card> cards)
        {
            query = query.Replace(" ", "+");
            byte[] compressedBytes;
            try
            {
                // New Base64URL format
                compressedBytes = WebEncoders.Base64UrlDecode(query);
            }
            catch (FormatException)
            {
                query = Uri.UnescapeDataString(query);

                compressedBytes = Convert.FromBase64String(query);
            }

            using var inputStream = new MemoryStream(compressedBytes);
            using var gzip = new GZipStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();

            gzip.CopyTo(outputStream);

            // Return original data again as Base64
            var finalString = Convert.ToBase64String(outputStream.ToArray());
            return GetFromQuery(finalString, cards);
        }

        public static QueryDeck GetFromQuery(string query, IEnumerable<Card> cards)
        {
            // Handle an old use case where legacy URL wasn't handled correctly
            query = query.PadRight(query.Length + (4 - query.Length % 4) % 4, '=');

            var converted = Convert.FromBase64String(query);
            var deckString = Encoding.UTF8.GetString(converted);

            return GetFromString(deckString, cards);
        }

        public static QueryDeck GetFromString(string deckString, IEnumerable<Card> cards)
        {
            var deckToReturn = new QueryDeck();
            var split = deckString.Split(';');
            var heroString = split[0];
            var landscapeString = split[1];
            var cardString = split[2];

            if (!string.IsNullOrEmpty(heroString) && !heroString.Equals("-1"))
                deckToReturn.Hero = cards.FirstOrDefault(x => x.Id == int.Parse(heroString));

            if (!string.IsNullOrEmpty(landscapeString))
            {
                foreach (var lsString in landscapeString.Split(','))
                {
                    var cardId = int.Parse(lsString.Split(":")[0]);
                    var count = int.Parse(lsString.Split(":")[1]);
                    var card = cards.FirstOrDefault(x => x.Id == cardId);
                    for (var i = 0; i < count; i++)
                    {
                        deckToReturn.Landscapes.Add(card);
                    }
                }
            }

            if (!string.IsNullOrEmpty(cardString))
            {
                foreach (var csString in cardString.Split(','))
                {
                    var cardId = int.Parse(csString.Split(":")[0]);
                    var count = int.Parse(csString.Split(":")[1]);
                    var card = cards.FirstOrDefault(x => x.Id == cardId);
                    for (var i = 0; i < count; i++)
                    {
                        deckToReturn.Cards.Add(card);
                    }
                }
            }

            return deckToReturn;
        }
    }
}
