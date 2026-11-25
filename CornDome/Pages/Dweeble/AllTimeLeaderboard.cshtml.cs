
using CornDome.Helpers;
using CornDome.Models;
using CornDome.Repository.Main;
using CornDome.Repository.Users;
using System.Security.Claims;

namespace CornDome.Pages.Dweeble
{
    public class AllTimeLeaderboardModel(IGameRepository gameRepository, IUserRepository userRepository) : BasePageModel
    {
        public List<AllTimeEntry> Players { get; set; } = [];
        public AllTimeEntry? CurrentPlayer { get; set; }
        public List<MonthlyLeaderboard> MonthlyLeaderboards { get; set; } = [];

        public int TotalPlayers { get; set; }
        public int TotalWins { get; set; }

        public async Task OnGetAsync()
        {
            var playerKey = GetPlayerKey();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);

            // Retrieve all successfully completed daily games.
            var games = await gameRepository.GetAllTimeDweebles();

            // Only registered Identity users.
            var userIds = games
                .Where(x => x.PlayerKey.StartsWith("user:"))
                .Select(x => int.Parse(x.PlayerKey.Substring(5)))
                .Distinct()
                .ToList();

            var users = await userRepository.GetByIds(userIds.ToArray());

            // Main leaderboard: last 30 days, including today.
            var recentGames = games
                .Where(x => x.DailyCard.Date >= firstDayOfMonth &&
                            x.DailyCard.Date <= today)
                .ToList();


            var ranked = BuildRankings(
                recentGames,
                users,
                playerKey,
                x => x.PlayerKey,
                x => x.DailyCard.Date,
                x => x.Mistakes,
                x => x.Id,
                x => x.UserName,
                x => IconHelper.GetUserIcon(x)
            );

            TotalPlayers = ranked.Count;
            TotalWins = ranked.Sum(x => x.TotalWins);

            Players = ranked.Take(50).ToList();

            CurrentPlayer = ranked.FirstOrDefault(x => x.IsCurrentPlayer);

            // Monthly leaderboards: every previous calendar month.
            var previousMonthGames = games
                .Where(x => x.DailyCard.Date < firstDayOfMonth)
                .ToList();

            MonthlyLeaderboards = previousMonthGames
                .GroupBy(x => new
                {
                    x.DailyCard.Date.Year,
                    x.DailyCard.Date.Month
                })
                .OrderByDescending(x => x.Key.Year)
                .ThenByDescending(x => x.Key.Month)
                .Select(month =>
                {
                    var monthRankings = BuildRankings(
                        month.ToList(),
                        users,
                        playerKey,
                        x => x.PlayerKey,
                        x => x.DailyCard.Date,
                        x => x.Mistakes,
                        x => x.Id,
                        x => x.UserName,
                        x => IconHelper.GetUserIcon(x)
                    );

                    return new MonthlyLeaderboard
                    {
                        Month = new DateOnly(
                            month.Key.Year,
                            month.Key.Month,
                            1
                        ),
                        Players = monthRankings.Take(3).ToList()
                    };
                })
                .Where(x => x.Players.Count > 0)
                .ToList();
        }


        private static List<AllTimeEntry> BuildRankings<TGame, TUser>(
            IEnumerable<TGame> source,
            IEnumerable<TUser> users,
            string playerKey,
            Func<TGame, string> getPlayerKey,
            Func<TGame, DateOnly> getDate,
            Func<TGame, int> getMistakes,
            Func<TUser, int> getUserId,
            Func<TUser, string> getUserName,
            Func<TUser, UserIcon> getUserIcon)
        {
            return source
                .GroupBy(getPlayerKey)
                .Where(group => group.Key.StartsWith("user:"))
                .Select(group =>
                {
                    var dates = group
                        .Select(getDate)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    var userId = int.Parse(group.Key.Substring(5));
                    var user = users.FirstOrDefault(x => getUserId(x) == userId);

                    return new
                    {
                        PlayerKey = group.Key,
                        DisplayName = user != null ? getUserName(user) : "",
                        TotalWins = group.Count(),
                        LongestStreak = GetLongestStreak(dates),
                        AverageMistakes = group.Average(getMistakes),
                        UserIcon = user != null ? getUserIcon(user) : default
                    };
                })
                .OrderByDescending(x => x.TotalWins)
                .ThenByDescending(x => x.LongestStreak)
                .ThenBy(x => x.AverageMistakes)
                .ThenBy(x => x.DisplayName)
                .Select((x, index) => new AllTimeEntry
                {
                    Rank = index + 1,
                    DisplayName = x.DisplayName,
                    TotalWins = x.TotalWins,
                    LongestStreak = x.LongestStreak,
                    AverageMistakes = x.AverageMistakes,
                    IsCurrentPlayer = x.PlayerKey == playerKey,
                    UserIcon = x.UserIcon
                })
                .ToList();
        }


        private static int GetLongestStreak(List<DateOnly> dates)
        {
            if (dates.Count == 0)
                return 0;

            var longest = 1;
            var current = 1;

            for (var i = 1; i < dates.Count; i++)
            {
                if (dates[i] == dates[i - 1].AddDays(1))
                    current++;
                else
                    current = 1;

                longest = Math.Max(longest, current);
            }

            return longest;
        }

        private string GetPlayerKey()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.Identity?.IsAuthenticated == true &&
                !string.IsNullOrWhiteSpace(userId))
            {
                return $"user:{userId}";
            }

            const string cookieName = "CardDweebDailyGuest";

            if (Request.Cookies.TryGetValue(cookieName, out var guestId) &&
                Guid.TryParse(guestId, out var parsed))
            {
                return $"guest:{parsed}";
            }

            var newGuestId = Guid.NewGuid().ToString();

            Response.Cookies.Append(
                cookieName,
                newGuestId,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    IsEssential = true,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                }
            );

            return $"guest:{newGuestId}";
        }

        public class AllTimeEntry
        {
            public int Rank { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public int TotalWins { get; set; }
            public int LongestStreak { get; set; }
            public double AverageMistakes { get; set; }
            public bool IsCurrentPlayer { get; set; }
            public UserIcon UserIcon { get; set; }
        }

        public class MonthlyLeaderboard
        {
            public DateOnly Month { get; set; }
            public List<AllTimeEntry> Players { get; set; } = [];
        }
    }
}
