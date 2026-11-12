using CornDome.Models.Main;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository.Main
{
    public interface IDeckRepository
    {
        bool IsPublic(int deckId);
        bool DoesUserHaveAccess(int deckId, int userId);
        Deck GetDeck(int deckId);

        bool AddDeck(Deck deck);
        int GetTotalUserDecks(int userId);
        List<Deck> GetUserDecks(int userId, int pageNumber, int pageSize, string sort = "newest");
        (int count, List<Deck> decks) GetPublicUsersDecks(int userId, int pageNumber, int pageSize, string sort = "newest");
        (int count, List<Deck> decks) GetAllVisibleDecks(string authorFilter, int? cardIdFilter, int pageNumber, int pageSize, string sort = "newest");
        bool ChangeDeckValue(int deckId, string deckString);
        bool ChangeDeckSettings(int deckId, DeckVisibility visibility, string description, int iconCardId, string deckName);
        bool DeleteDeck(int deckId);
        int AddView(int deckId);

        bool ToggleLike(int userId, int deckId);
        bool AddComment(int userId, int deckId, string commentText);
        List<DeckComment> GetComments(int deckId);
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
                    .Include(x => x.DeckComments)
                        .ThenInclude(x => x.User)
                    .Include(x => x.DeckLikes)
                    .Where(x => x.Id == deckId)
                    .FirstOrDefault();

            return deck;
        }

        public int AddView(int deckId)
        {
            var deck = mainContext.Decks.FirstOrDefault(d => d.Id == deckId);

            if (deck == null)
            {
                return 00;
            }

            try
            {
                deck.Views++;
                mainContext.SaveChanges();
                return deck.Views;
            }
            catch
            {
                return 0;
            }
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

        public List<Deck> GetUserDecks(int userId, int pageNumber, int pageSize, string sort)
        {
            return ApplyDeckSort(
                    mainContext.Decks
                        .Include(card => card.User)
                        .Include(x => x.DeckLikes)
                        .Include(x => x.DeckComments)
                        .Where(u => u.UserId == userId),
                    sort)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private static IOrderedQueryable<Deck> ApplyDeckSort(IQueryable<Deck> query, string sort)
        {
            return sort?.ToLowerInvariant() switch
            {
                "oldest" => query.OrderBy(deck => deck.Created),

                "views" => query.OrderByDescending(deck => deck.Views)
                                    .ThenByDescending(deck => deck.Modified),

                "likes" => query.OrderByDescending(deck => deck.DeckLikes.Count())
                                    .ThenByDescending(deck => deck.Modified),

                "comments" => query.OrderByDescending(deck => deck.DeckComments.Count())
                                    .ThenByDescending(deck => deck.Modified),

                _ => query.OrderByDescending(deck => deck.Modified)
            };
        }

        public (int count, List<Deck> decks) GetAllVisibleDecks(string authorFilter, int? cardIdFilter, int pageNumber, int pageSize, string sort)
        {
            var query = mainContext.Decks
                .Include(deck => deck.User)
                .Include(x => x.DeckComments)
                .Include(x => x.DeckLikes)
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

            var totalDecks = query.Count();
            var decks = ApplyDeckSort(query, sort)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (totalDecks, decks);
        }

        public (int count, List<Deck> decks) GetPublicUsersDecks(int userId, int pageNumber, int pageSize, string sort)
        {
            var query = mainContext.Decks
                .Include(card => card.User)
                .Include(x => x.DeckComments)
                .Include(x => x.DeckLikes)
                .Where(u => u.UserId == userId &&
                            u.Visibility == DeckVisibility.Visible);

            var totalDecks = query.Count();

            var decks = ApplyDeckSort(query, sort)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (totalDecks, decks);
        }

        public bool ChangeDeckSettings(int deckId, DeckVisibility visibility, string description, int iconCardId, string deckName)
        {
            var deck = mainContext.Decks.FirstOrDefault(d => d.Id == deckId);

            if (deck == null)
            {
                return false;
            }

            try
            {
                deck.DeckName = deckName;
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

        public bool ToggleLike(int userId, int deckId)
        {
            try
            {
                var like = mainContext.DeckLikes.FirstOrDefault(x => x.UserId == userId && x.DeckId == deckId);

                if (like == null)
                {
                    var newLike = new DeckLike()
                    {
                        UserId = userId,
                        DeckId = deckId,
                        Liked = true
                    };
                    mainContext.Add(newLike);
                    mainContext.SaveChanges();
                    return true;
                }
                else
                {
                    like.Liked = !like.Liked;
                    var result = mainContext.SaveChanges();
                    return like.Liked;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool AddComment(int userId, int deckId, string commentText)
        {
            try
            {
                var comment = new DeckComment
                {
                    UserId = userId,
                    DeckId = deckId,
                    CommentText = commentText.Trim(),
                    Timestamp = DateTime.Now
                };

                mainContext.DeckComments.Add(comment);

                return mainContext.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public List<DeckComment> GetComments(int deckId)
        {
            return mainContext.DeckComments
                .Include(x => x.User)
                .Where(x => x.DeckId == deckId)
                .OrderByDescending(x => x.Timestamp)
                .ToList();
        }
    }
}
