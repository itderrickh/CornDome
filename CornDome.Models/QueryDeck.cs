using CornDome.Models.Cards;
using CornDome.Models.Main;
using Microsoft.AspNetCore.WebUtilities;
using System.IO.Compression;
using System.Text;

namespace CornDome.Models
{
    public class QueryDeck
    {
        public DisplayCardModel Hero { get; set; }
        public List<DisplayCardModel> Landscapes { get; set; } = [];
        public List<DisplayCardModel> Cards { get; set; } = [];

        public static QueryDeck GetDeckFromGzip(string query, IEnumerable<Card> cards, IEnumerable<CustomCard> customCards)
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
            return GetFromQuery(finalString, cards, customCards);
        }

        public static QueryDeck GetFromQuery(string query, IEnumerable<Card> cards, IEnumerable<CustomCard> customCards)
        {
            // Handle an old use case where legacy URL wasn't handled correctly
            query = query.PadRight(query.Length + (4 - query.Length % 4) % 4, '=');

            var converted = Convert.FromBase64String(query);
            var deckString = Encoding.UTF8.GetString(converted);

            return GetFromString(deckString, cards, customCards);
        }

        public static QueryDeck GetFromString(string deckString, IEnumerable<Card> cards, IEnumerable<CustomCard> customCards)
        {
            var deckToReturn = new QueryDeck();
            var split = deckString.Split(';');
            var heroString = split[0];
            var landscapeString = split[1];
            var cardString = split[2];

            if (!string.IsNullOrEmpty(heroString) && !heroString.Equals("-1"))
            {
                if (heroString.StartsWith("c"))
                {
                    var cardId = int.Parse(heroString.Substring(1));
                    var card = customCards.FirstOrDefault(x => x.CardId == cardId);
                    if (card != null)
                    {
                        deckToReturn.Hero = new DisplayCardModel(card);
                    }
                }
                else
                {
                    var cardId = int.Parse(heroString);
                    var card = cards.FirstOrDefault(x => x.Id == cardId);
                    if (card != null)
                    {
                        deckToReturn.Hero = new DisplayCardModel(card);
                    }
                }
                
            }
                
            if (!string.IsNullOrEmpty(landscapeString))
            {
                foreach (var lsString in landscapeString.Split(','))
                {
                    var part1 = lsString.Split(":")[0];

                    if (part1.StartsWith("c"))
                    {
                        var cardId = int.Parse(part1.Substring(1));
                        var card = customCards.FirstOrDefault(x => x.CardId == cardId);
                        var count = int.Parse(lsString.Split(":")[1]);
                        if (card != null)
                        {
                            for (var i = 0; i < count; i++)
                            {
                                deckToReturn.Landscapes.Add(new DisplayCardModel(card));
                            }
                        }
                    }
                    else
                    {
                        var cardId = int.Parse(part1);
                        var card = cards.FirstOrDefault(x => x.Id == cardId);
                        var count = int.Parse(lsString.Split(":")[1]);
                        if (card != null)
                        {
                            for (var i = 0; i < count; i++)
                            {
                                deckToReturn.Landscapes.Add(new DisplayCardModel(card));
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(cardString))
            {
                foreach (var csString in cardString.Split(','))
                {
                    var part1 = csString.Split(":")[0];

                    if (part1.StartsWith("c"))
                    {
                        var cardId = int.Parse(part1.Substring(1));
                        var card = customCards.FirstOrDefault(x => x.CardId == cardId);
                        var count = int.Parse(csString.Split(":")[1]);
                        if (card != null)
                        {
                            for (var i = 0; i < count; i++)
                            {
                                deckToReturn.Cards.Add(new DisplayCardModel(card));
                            }
                        }
                    } 
                    else
                    {
                        var cardId = int.Parse(part1);
                        var count = int.Parse(csString.Split(":")[1]);
                        var card = cards.FirstOrDefault(x => x.Id == cardId);
                        if (card != null)
                        {
                            for (var i = 0; i < count; i++)
                            {
                                deckToReturn.Cards.Add(new DisplayCardModel(card));
                            }
                        }
                    }
                }
            }

            return deckToReturn;
        }
    }
}
