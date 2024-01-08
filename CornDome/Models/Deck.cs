namespace CornDome.Models
{
    public class Deck
    {
        public Card Hero { get; set; }
        public List<Card> Landscapes { get; set; } = [];
        public List<Card> Cards { get; set; } = [];

        public static Deck GetFromQuery(string query, IEnumerable<Card> cards)
        {
            var deckToReturn = new Deck();
            var converted = Convert.FromBase64String(query);
            var deckString = System.Text.Encoding.UTF8.GetString(converted);

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
