using CornDome.Models;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository
{
    public interface IDeckRepository
    {
        bool IsPublic(int deckId);
        bool DoesUserHaveAccess(int deckId, int userId);
        Deck GetDeck(int deckId);

        bool AddDeck(Deck deck);
        int GetTotalUserDecks(int userId);
        List<Deck> GetUserDecks(int userId, int pageNumber, int pageSize);
        (int count, List<Deck> decks) GetPublicUsersDecks(int userId, int pageNumber, int pageSize);
        (int count, List<Deck> decks) GetAllVisibleDecks(string authorFilter, int? cardIdFilter, int pageNumber, int pageSize);
        bool ChangeDeckValue(int deckId, string deckString);
        bool ChangeDeckSettings(int deckId, DeckVisibility visibility, string description, int iconCardId);
        bool DeleteDeck(int deckId);
    }

    public class DeckRepository(MainContext mainContext) : IDeckRepository
    {
        public bool IsPublic(int deckId)
        {
            var deck = mainContext.Decks
                    .Where(x => x.Id == deckId)
                    .Where(y => y.Visibility == DeckVisibility.Visible || y.Visibility == DeckVisibility.Limited)
                    .FirstOrDefault();
            return deck != null;
        }
        public bool DoesUserHaveAccess(int deckId, int userId)
        {
            var deck = mainContext.Decks
                    .Where(x => x.Id == deckId)
                    .Where(y => y.Visibility == DeckVisibility.Visible || y.Visibility == DeckVisibility.Limited || (y.Visibility == DeckVisibility.Invisible && y.UserId == userId))
                    .FirstOrDefault();
            return deck != null;
        }

        public Deck GetDeck(int deckId)
        {
            var deck = mainContext.Decks
                    .Where(x => x.Id == deckId)
                    .FirstOrDefault();

            return deck;
        }

        public bool AddDeck(Deck deck)
        {
            mainContext.Decks.Add(deck);
            return mainContext.SaveChanges() > 0;
        }

        public int GetTotalUserDecks(int userId)
        {
            var totalDecks = mainContext.Decks
                .Include(card => card.User)
                .Where(u => u.UserId == userId)
                .Count();
            return totalDecks;
        }

        public List<Deck> GetUserDecks(int userId, int pageNumber, int pageSize)
        {
            return mainContext.Decks
                .Include(card => card.User)
                .Where(u => u.UserId == userId)
                .OrderByDescending(deck => deck.Modified)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public (int count, List<Deck> decks) GetAllVisibleDecks(string authorFilter, int? cardIdFilter, int pageNumber, int pageSize)
        {
            var query = mainContext.Decks
                .Include(deck => deck.User)
                .Where(deck => deck.Visibility == DeckVisibility.Visible)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(authorFilter))
            {
                query = query.Where(deck =>
                    deck.User.UserName != null &&
                    deck.User.UserName.Contains(authorFilter));
            }

            if (cardIdFilter.HasValue)
            {
                query = query.Where(deck => deck.DeckString.Contains(cardIdFilter.Value.ToString() + ":") || deck.DeckString.StartsWith(cardIdFilter.Value.ToString() + ";"));
            }

            query = query.OrderByDescending(deck => deck.Modified);
            var totalDecks = query.Count();
            var decks = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (totalDecks, decks);
        }

        public (int count, List<Deck> decks) GetPublicUsersDecks(int userId, int pageNumber, int pageSize)
        {
            var totalDecks = mainContext.Decks
                .Include(card => card.User)
                .Where(u => u.UserId == userId && u.Visibility == DeckVisibility.Visible)
                .Count();

            var decks = mainContext.Decks
                .Include(card => card.User)
                .Where(u => u.UserId == userId && u.Visibility == DeckVisibility.Visible)
                .OrderByDescending(deck => deck.Modified)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (totalDecks, decks);
        }

        public bool ChangeDeckSettings(int deckId, DeckVisibility visibility, string description, int iconCardId)
        {
            var deck = mainContext.Decks.FirstOrDefault(d => d.Id == deckId);

            if (deck == null)
            {
                return false;
            }

            try
            {
                deck.Description = description;
                deck.IconCardId = iconCardId;
                deck.Visibility = visibility;
                deck.Modified = DateTime.Now;
                var result = mainContext.SaveChanges();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool ChangeDeckValue(int deckId, string deckString)
        {
            var deck = mainContext.Decks.FirstOrDefault(d => d.Id == deckId);

            if (deck == null)
            {
                return false;
            }

            try
            {
                deck.DeckString = deckString;
                deck.Modified = DateTime.Now;
                var result = mainContext.SaveChanges();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteDeck(int deckId)
        {
            try
            {
                var deck = mainContext.Decks.FirstOrDefault(d => d.Id == deckId);

                if (deck == null)
                    return false;

                mainContext.Remove(deck);
                var result = mainContext.SaveChanges();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
