using CornDome.Models;
using CornDome.Models.Users;
using CornDome.Repository;
using CornDome.Repository.Discord;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Play
{
    public class BoardItem
    {
        public List<PlayPreferences> PlayPreferences { get; set; }
        public Dictionary<int, List<PlayAvailability>> Availability { get; set; } = new Dictionary<int, List<PlayAvailability>>();
        public Dictionary<int, DiscordConnection> UserTable { get; set; } = new Dictionary<int, DiscordConnection>();
    }

    public class BoardModel(IDiscordRepository discordRepository, IUserRepository userRepository) : PageModel
    {
        public DiscordConnection DiscordConnection { get; set; }
        public bool IsUserInServer { get; set; }
        public List<TimeZoneInfo> TimeZones { get; set; } = [..TimeZoneInfo.GetSystemTimeZones()];
        public BoardItem Board { get; set; } = new BoardItem();
        public bool AmIOnTheBoard { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));

            DiscordConnection = await discordRepository.GetDiscordConnection(loggedInUser.Id);
            IsUserInServer = await discordRepository.IsUserInGuildAsync(DiscordConnection);

            var activePlayPreferences = await discordRepository.GetAllActivePlayPreferences();

            Board.PlayPreferences = [..activePlayPreferences];
            foreach (var app in activePlayPreferences)
            {
                var avails = await discordRepository.GetPlayAvailabilities(app.UserId);
                Board.Availability[app.UserId] = [..avails];

                var dc = await discordRepository.GetDiscordConnection(app.UserId);
                Board.UserTable[app.UserId] = dc;
            }

            AmIOnTheBoard = activePlayPreferences.FirstOrDefault(x => x.UserId == loggedInUser.Id) != null;

            return Page();
        }
    }
}
