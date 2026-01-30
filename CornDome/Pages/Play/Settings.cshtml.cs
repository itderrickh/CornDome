using CornDome.Models;
using CornDome.Models.Users;
using CornDome.Repository;
using CornDome.Repository.Discord;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Play
{
    public class AvailabilityDayVm
    {
        public DayOfWeek Day { get; set; }
        public bool IsAvailable { get; set; }
        public TimeOnly? Start { get; set; }
        public TimeOnly? End { get; set; }
    }

    public class PlayPreferencesForm
    {
        public string TimeZone { get; set; }
        public string GameFormat { get; set; }
        public string Pronouns { get; set; }
        public string AddressMeAs { get; set; }
        public string Platform { get; set; }
        public bool WantsToPlay { get; set; }
        public List<AvailabilityDayVm> Days { get; set; } = new();
    }

    public class SettingsModel(IDiscordRepository discordRepository, IUserRepository userRepository) : PageModel
    {
        public DiscordConnection DiscordConnection { get; set; }
        public bool IsUserInServer { get; set; }
        [BindProperty]
        public PlayPreferencesForm Form { get; set; }
        public List<TimeZoneInfo> TimeZones { get; set; } = [.. TimeZoneInfo.GetSystemTimeZones()];


        public async Task<IActionResult> OnGet()
        {
            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));

            DiscordConnection = await discordRepository.GetDiscordConnection(loggedInUser.Id);
            IsUserInServer = await discordRepository.IsUserInGuildAsync(DiscordConnection);

            var preferences = await discordRepository.GetPlayPreferences(loggedInUser.Id);
            var availabilities = await discordRepository.GetPlayAvailabilities(loggedInUser.Id);

            Form = new PlayPreferencesForm
            {
                TimeZone = preferences?.TimeZone ?? "",
                AddressMeAs = preferences?.AddressMeAs ?? "",
                GameFormat = preferences?.GameFormat ?? "",
                Platform = preferences?.Platform ?? "",
                Pronouns = preferences?.Pronouns ?? "",
                WantsToPlay = preferences?.WantsToPlay ?? false
            };

            foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
            {
                var match = availabilities.FirstOrDefault(x => x.Day == day);

                Form.Days.Add(new AvailabilityDayVm
                {
                    Day = day,
                    IsAvailable = match?.IsAvailable ?? false,
                    Start = match?.StartTime ?? TimeOnly.Parse("5:00 PM"),
                    End = match?.EndTime ?? TimeOnly.Parse("8:00 PM")
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));
            var userId = loggedInUser.Id;

            foreach (var day in Form.Days)
            {
                if (day.IsAvailable)
                {
                    if (!day.Start.HasValue || !day.End.HasValue)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            $"{day.Day}: start and end times are required.");
                    }
                    else if (day.Start >= day.End)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            $"{day.Day}: start time must be before end time.");
                    }
                }
            }

            if (!ModelState.IsValid)
                return Page();

            PlayPreferences preferences = new()
            {
                GameFormat = Form.GameFormat,
                AddressMeAs = Form.AddressMeAs,
                Platform = Form.Platform,
                Pronouns = Form.Pronouns,
                TimeZone = Form.TimeZone,
                WantsToPlay = Form.WantsToPlay,
                UserId = userId
            };

            await discordRepository.UpdatePlayPreferences(preferences);

            var newPlayAvail = new List<PlayAvailability>();

            // Insert updated rows
            foreach (var day in Form.Days)
            {
                if (day.Start != null && day.End != null && day.IsAvailable)
                {
                    newPlayAvail.Add(new PlayAvailability
                    {
                        UserId = userId,
                        Day = day.Day,
                        IsAvailable = day.IsAvailable,
                        StartTime = day.IsAvailable ? day.Start : null,
                        EndTime = day.IsAvailable ? day.End : null
                    });
                }

            }

            await discordRepository.UpdatePlayAvailabilities(userId, newPlayAvail);

            return RedirectToPage("/Play/Board"); // or success page
        }
    }
}
