using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class InsertTournamentModel(ITournamentRepository tournamentRepository) : PageModel
    {
        [BindProperty]
        public Tournament Tournament { get; set; }
        [BindProperty]
        public List<TournamentResult> Results { get; set; } = [];

        public void OnGet()
        {
            for (int i = 1; i <= 8; i++)
            {
                Results.Add(new TournamentResult { Placement = i });
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            tournamentRepository.InsertTournament(Tournament, Results);

            return RedirectToPage("/TournamentResults"); // Redirect after successful save
        }
    }
}
