using CornDome.Models.Cards;
using CornDome.Models.Main.DailyCard;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository.Main
{
    public interface IGameRepository
    {
        Task<DailyCard> GetTodayCardAsync();
        Task<DailyCardGame> GetDailyCardGameAsync(string playerKey);
        Task AddGame(DailyCardGame game, string playerKey);
        Task UpdateGameAttributeGuess(DailyCardGame game, Card card, string guess, string answer, string key, int maxMistakes);
        Task UpdateGameLetterGuess(DailyCardGame game, Card card, string guess, int maxMistakes);
        Task UpdateGameNameGuess(DailyCardGame game, Card card, string guess, int maxMistakes);
        Task<IList<DailyCardGame>> GetSuccessfulDweeblesToday();
        Task<List<DailyCardGame>> GetAllTimeDweebles();
        Task<List<DailyCardGame>> GetMonthlyDweebles();
        Task MigrateGuestGame(DailyCardGame game, string newId);
    }

    public class GameRepository(MainContext mainContext, CardDatabaseContext cardDatabaseContext) : IGameRepository
    {
        public static DateOnly GetToday()
        {
            return DateOnly.FromDateTime(DateTime.UtcNow);
        }

        public static DateOnly GetFirstOfMonth()
        {
            return DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
        }

        public async Task AddGame(DailyCardGame game, string playerKey)
        {
            var dailyCard = await GetTodayCardAsync();
            
            try
            {
                mainContext.DailyCardGames.Add(game);
                await mainContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                mainContext.Entry(game).State = EntityState.Detached;

                game = await mainContext.DailyCardGames
                    .FirstAsync(x =>
                        x.DailyCardId == dailyCard.Id &&
                        x.PlayerKey == playerKey
                    );
            }
        }

        public async Task<DailyCardGame> GetDailyCardGameAsync(string playerKey)
        {
            var dailyCard = await GetTodayCardAsync();
            var game = await mainContext.DailyCardGames
                .FirstOrDefaultAsync(x =>
                    x.DailyCardId == dailyCard.Id &&
                    x.PlayerKey == playerKey
                );

            return game;
        }

        public async Task<IList<DailyCardGame>> GetSuccessfulDweeblesToday()
        {
            var dailyCard = await GetTodayCardAsync();
            var games = await mainContext.DailyCardGames
                .AsNoTracking()
                .Where(x =>
                    x.DailyCardId == dailyCard.Id &&
                    x.IsComplete &&
                    x.Won &&
                    x.Completed != null)
                .ToListAsync();

            return games;
        }

        public async Task<List<DailyCardGame>> GetAllTimeDweebles()
        {
            var games = await mainContext.DailyCardGames
                .Include(x => x.DailyCard)
                .AsNoTracking()
                .Where(x =>
                    x.IsComplete &&
                    x.Won &&
                    x.Completed != null &&
                    x.PlayerKey != "user:1") // Remove the carddweeb from the overall leaderboard
                .ToListAsync();

            return games;
        }

        public async Task<List<DailyCardGame>> GetMonthlyDweebles()
        {
            var firstOfMonth = GetFirstOfMonth();
            var games = await mainContext.DailyCardGames
                .Include(x => x.DailyCard)
                .AsNoTracking()
                .Where(x =>
                    x.IsComplete &&
                    x.Won &&
                    x.Completed != null &&
                    x.DailyCard.Date > firstOfMonth &&
                    x.PlayerKey != "user:1")
                .ToListAsync();

            return games;
        }

        public async Task<DailyCard> GetTodayCardAsync()
        {
            var today = GetToday();

            var existing = await mainContext.DailyCards
                .FirstOrDefaultAsync(x => x.Date == today);

            if (existing != null)
                return existing;

            // Only select official cards.
            var eligibleCards = cardDatabaseContext.Cards
                .Where(x => !x.IsCustomCard && x.TypeId != (int)CardTypeEnum.Landscape);

            var count = await eligibleCards.CountAsync();

            if (count == 0)
                throw new InvalidOperationException(
                    "There are no eligible cards for the daily challenge."
                );

            var randomIndex = Random.Shared.Next(count);

            var cardId = await eligibleCards
                .OrderBy(x => x.Id)
                .Skip(randomIndex)
                .Select(x => x.Id)
                .FirstAsync();

            var dailyCard = new DailyCard
            {
                Date = today,
                CardId = cardId
            };

            mainContext.DailyCards.Add(dailyCard);

            try
            {
                await mainContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                mainContext.Entry(dailyCard).State = EntityState.Detached;
            }

            return await mainContext.DailyCards
                .FirstAsync(x => x.Date == today);
        }

        public async Task UpdateGameAttributeGuess(DailyCardGame game, Card card, string guess, string answer, string key, int maxMistakes)
        {
            if (string.Equals(guess, answer, StringComparison.OrdinalIgnoreCase))
            {
                game.AddAttribute(key);
            }
            else
            {
                game.Mistakes++;
                game.AddAttribute(key);
            }

            if (game.Mistakes >= maxMistakes)
            {
                game.FinishGame(false);
            }
            else
            {
                game.CheckWin(card.Name);
            }

            await mainContext.SaveChangesAsync();
        }

        public async Task UpdateGameLetterGuess(DailyCardGame game, Card card, string guess, int maxMistakes)
        {
            game.GuessedLetters += guess;

            if (!card.Name.Contains(guess, StringComparison.OrdinalIgnoreCase))
            {
                game.Mistakes++;
            }

            if (game.Mistakes >= maxMistakes)
            {
                game.FinishGame(false);
            }
            else if (game.IsNameComplete(card.Name))
            {
                game.AddAttribute("ability");

                game.CheckWin(card.Name);
            }

            await mainContext.SaveChangesAsync();
        }

        public async Task UpdateGameNameGuess(DailyCardGame game, Card card, string guess, int maxMistakes)
        {
            if (!card.Name.Contains(guess, StringComparison.OrdinalIgnoreCase))
            {
                game.Mistakes++;
            }
            else
            {
                // User ripped the guess right
                game.FinishGame(true);
            }

            if (game.Mistakes >= maxMistakes)
            {
                game.FinishGame(false);
            }
            else if (game.IsNameComplete(card.Name))
            {
                game.AddAttribute("ability");

                game.CheckWin(card.Name);
            }

            await mainContext.SaveChangesAsync();
        }

        public async Task MigrateGuestGame(DailyCardGame game, string newId)
        {
            try
            {
                game.PlayerKey = newId;
                await mainContext.SaveChangesAsync();
            }
            catch
            {

            }
        }
    }
}
