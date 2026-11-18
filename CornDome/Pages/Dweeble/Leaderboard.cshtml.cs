using CornDome.Models;
using CornDome.Repository.Main;
using CornDome.Repository.Users;
using System.Security.Claims;

namespace CornDome.Pages.Dweeble;

public class LeaderboardModel(IGameRepository gameRepository, IUserRepository userRepository) : BasePageModel
{
    public List<LeaderboardEntry> Players { get; set; } = [];

    public LeaderboardEntry? CurrentPlayer { get; set; }

    public int TotalPlayers { get; set; }

    public DateOnly ChallengeDate { get; set; }

    public async Task OnGetAsync()
    {
        var dailyCard = await gameRepository.GetTodayCardAsync();

        ChallengeDate = dailyCard.Date;

        var playerKey = GetPlayerKey();

        // Retrieve all successful games for today's challenge.
        var games = await gameRepository.GetSuccessfulDweeblesToday();

        // Extract registered Identity user IDs.
        var userIds = games
            .Where(x => x.PlayerKey.StartsWith("user:"))
            .Select(x => int.Parse(x.PlayerKey.Substring(5)))
            .Distinct()
            .ToList();

        // Retrieve usernames from MainContext.
        var users = await userRepository.GetByIds(userIds.ToArray());

        // Rank all successful players.
        var ranked = games
            .Where(x => x.PlayerKey.StartsWith("user:"))
            .Select(x =>
            {
                var userId = int.Parse(x.PlayerKey.Substring(5));

                string displayName = "";
                var user = users.FirstOrDefault(x => x.Id == userId);
                if (user != null)
                {
                    displayName = user.UserName;
                }

                var isCoolGuy = user.UserRoles?.Any(x => x.Role.Name == "CoolGuy") ?? false;

                return new
                {
                    x.PlayerKey,
                    DisplayName = displayName,
                    x.Mistakes,
                    IsCoolGuy = isCoolGuy,
                    Seconds = Math.Max(
                        0,
                        (int)(x.Completed!.Value - x.Started).TotalSeconds
                    ),
                    Completed = x.Completed.Value
                };
            })
            .OrderBy(x => x.Mistakes)
            .ThenBy(x => x.Seconds)
            .ThenBy(x => x.Completed)
            .Select((x, index) => new LeaderboardEntry
            {
                Rank = index + 1,
                DisplayName = x.DisplayName,
                Mistakes = x.Mistakes,
                Seconds = x.Seconds,
                IsCoolGuy = x.IsCoolGuy,
                IsCurrentPlayer = x.PlayerKey == playerKey
            })
            .ToList();

        TotalPlayers = ranked.Count;

        Players = ranked.Take(50).ToList();

        CurrentPlayer = ranked.FirstOrDefault(
            x => x.IsCurrentPlayer
        );
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

    public class LeaderboardEntry
    {
        public int Rank { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public int Mistakes { get; set; }

        public int Seconds { get; set; }

        public bool IsCurrentPlayer { get; set; }
        public bool IsCoolGuy { get; set; }
    }
}