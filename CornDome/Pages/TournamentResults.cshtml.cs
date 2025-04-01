using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages
{
    public class TournamentResultsModel(ITournamentRepository tournamentRepository) : PageModel
    {
        public List<Tournament> Tournaments { get; set; } = [];
        public void OnGet()
        {
            Tournaments = tournamentRepository.GetAllTournaments()
                .OrderByDescending(x => x.TournamentDate)
                .Where(x => x.TournamentDate > DateTime.Now.AddDays(-90))
                .ToList();
        }
    }
}
