using CornDome.Models.Tournaments;
using CornDome.Models.Users;
using CornDome.Repository;
using CornDome.Repository.Tournaments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CornDome.Pages.Account
{
    [Authorize]
    public class TournamentDashboardModel(TournamentContext tournamentContext, IUserRepository userRepository) : PageModel
    {
        public List<TournamentRegistration> Registrations { get; set; }
        public int UserId { get; set; }
        public List<User> Players { get; set; }
        public async void OnGet()
        {
            await Load();   
        }

        private async Task Load()
        {
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Registrations = tournamentContext.Registrations
                .Include(x => x.Tournament)
                .ThenInclude(t => t.Rounds)
                .ThenInclude(r => r.Matches)
                .Where(x => x.UserId == UserId)
                .ToList();

            var allUsers = await userRepository.GetAll();
            Players = allUsers.ToList();
        }

        [BindProperty]
        public int PostMatchId { get; set; }
        [BindProperty]
        public int PostTournamentId { get; set; }
        [BindProperty]
        public MatchResult PostResult { get; set; }

        public async Task<IActionResult> OnPostMatchResult()
        {
            var match = tournamentContext.Matches.FirstOrDefault(x => x.Id == PostMatchId);

            if (match.Result == MatchResult.Incomplete)
            {
                match.Result = PostResult;
                tournamentContext.SaveChanges();
            }

            await Load();

            return Page();
        }
    }
}
