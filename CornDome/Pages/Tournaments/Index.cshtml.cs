using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Tournaments
{
    public class IndexModel(ITournamentRepository tournamentRepository) : PageModel
    {
        [BindProperty]
        public Tournament TournamentToInsert { get; set; }

        public List<Tournament> AllTournaments { get; set; }
        public void OnGet()
        {
            AllTournaments = tournamentRepository.GetAllTournaments();
        }

        public IActionResult OnPostInsertTournament()
        {
            if (!User.IsInRole("Admin"))
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "You do not have permission to this action";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Added";
                return Page();
            }

            var result = tournamentRepository.InsertTournament(TournamentToInsert);

            if (result > 0)
            {
                TempData["Status"] = "success";
                TempData["Message"] = "Tournament Successfully Added";
            }
            else
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Added";
            }

            AllTournaments = tournamentRepository.GetAllTournaments();
            return Page();
        }
    }
}
