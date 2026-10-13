using CornDome.Models.Cards;
using CornDome.Models.Users;

namespace CornDome.Models.Main
{
    public enum DeckVisibility
    {
        Visible,
        Limited,
        Invisible
    }

    public class Deck
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public string DeckString { get; set; }
        public int IconCardId { get; set; }
        public string Description { get; set; }
        public string DeckName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public DeckVisibility Visibility { get; set; }

        public virtual List<DeckComment> DeckComments { get; set; }
    }

    public class SerializableDeck
    {
        public SerializableDeck(Deck deck)
        {
            Id = deck.Id;
            UserId = deck.UserId;
            Username = deck.User.UserName;
            IconCardId = deck.IconCardId;
            Description = deck.Description;
            DeckName = deck.DeckName;
            Created = deck.Created;
            Modified = deck.Modified;
            Visibility = deck.Visibility;

            var parts = deck.DeckString.Split(";");
            if (!string.IsNullOrEmpty(parts[0]))
            {
                Hero = new SerializedCard()
                {
                    Count = 1,
                    CardId = int.Parse(parts[0])
                };
            }

            if (!string.IsNullOrEmpty(parts[1]))
            {
                var landscapes = parts[1].Split(",");
                foreach (var landscape in landscapes)
                {
                    var pieces = landscape.Split(":");
                    Landscapes.Add(new SerializedCard()
                    {
                        CardId = int.Parse(pieces[0]),
                        Count = int.Parse(pieces[1])
                    });
                }
            }

            if (!string.IsNullOrEmpty(parts[2]))
            {
                var cards = parts[2].Split(",");
                foreach (var card in cards)
                {
                    var pieces = card.Split(":");
                    Cards.Add(new SerializedCard()
                    {
                        CardId = int.Parse(pieces[0]),
                        Count = int.Parse(pieces[1])
                    });
                }
            }
        }
        public int Id { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; }
        private string DeckString { get; set; }
        public int IconCardId { get; set; }
        public string Description { get; set; }
        public string DeckName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public DeckVisibility Visibility { get; set; }
        public List<SerializedCard> Landscapes { get; set; } = [];
        public List<SerializedCard> Cards { get; set; } = [];
        public SerializedCard Hero { get; set; }
    }

    public class SerializedCard
    {
        public int CardId { get; set; }
        public int Count { get; set; }
    }
}
