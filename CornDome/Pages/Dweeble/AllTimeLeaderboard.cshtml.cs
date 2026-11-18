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

        public int TotalPlayers { get; set; }

        public int TotalWins { get; set; }

        public async Task OnGetAsync()
        {
            var playerKey = GetPlayerKey();

            // Retrieve every successfully completed daily game.
            var games = await gameRepository.GetAllTimeDweebles();

            // Extract registered Identity user IDs.
            var userIds = games
                .Where(x => x.PlayerKey.StartsWith("user:"))
                .Select(x => int.Parse(x.PlayerKey.Substring(5)))
                .Distinct()
                .ToList();

            // Retrieve usernames from MainContext.
            var users = await userRepository.GetByIds(userIds.ToArray());

            // Calculate statistics for each player.
            var ranked = games
                .GroupBy(x => x.PlayerKey)
                .Where(group => group.Key.StartsWith("user:"))
                .Select(group =>
                {
                    var dates = group
                        .Select(x => x.DailyCard.Date)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                    var totalWins = group.Count();

                    var averageMistakes = group.Average(x => x.Mistakes);

                    var longestStreak = GetLongestStreak(dates);

                    var userId = int.Parse(group.Key.Substring(5));

                    string displayName = "";
                    var user = users.FirstOrDefault(x => x.Id == userId);
                    if (user != null)
                    {
                        displayName = user.UserName;
                    }

                    var isCoolGuy = user.UserRoles?.Any(x => x.Role.Name == "CoolGuy") ?? false;

                    return new
                    {
                        PlayerKey = group.Key,
                        DisplayName = displayName,
                        TotalWins = totalWins,
                        LongestStreak = longestStreak,
                        AverageMistakes = averageMistakes,
                        IsCoolGuy = isCoolGuy,
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
                    IsCoolGuy = x.IsCoolGuy
                })
                .ToList();

            TotalPlayers = ranked.Count;
            TotalWins = ranked.Sum(x => x.TotalWins);

            Players = ranked.Take(50).ToList();

            CurrentPlayer = ranked.FirstOrDefault(
                x => x.IsCurrentPlayer
            );
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
                {
                    current++;
                }
                else
                {
                    current = 1;
                }

                longest = Math.Max(longest, current);
            }

            return longest;
        }

        private string GetPlayerKey()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

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
            public bool IsCoolGuy { get; set; }
        }
    }
}
