using CornDome.Models.Tournaments;
using CornDome.Repository.Tournaments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity;
using System.Security.Claims;

namespace CornDome.Pages.Account
{
    [Authorize]
    public class TournamentDashboardModel(TournamentContext tournamentContext) : PageModel
    {
        public List<TournamentRegistration> Registrations { get; set; }
        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Registrations = tournamentContext.Tournaments
            //    .Include(x => x.Registrations)
            //    .SelectMany(x => x.Registrations)
            //    .Where(x => x.UserId == int.Parse(userId))
            //    .ToList();
            Registrations = tournamentContext.Registrations
                .Include(x => x.Tournament)
                .Where(x => x.UserId == int.Parse(userId))
                .ToList();
        }
    }
}
