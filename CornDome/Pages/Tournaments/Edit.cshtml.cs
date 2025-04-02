using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Tournaments
{
    [Authorize(Policy = "admin")]
    public class EditModel(ITournamentRepository tournamentRepository) : PageModel
    {
        [BindProperty]
        public Tournament Tournament { get; set; }

        public int TournamentId { get; set; }
        public void OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);

            Tournament = tournamentRepository.GetById(TournamentId);
        }

        public IActionResult OnPostEditTournament()
        {
            if (!ModelState.IsValid)
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Updated";
                return Page();
            }

            var result = tournamentRepository.UpdateTournament(Tournament.Id, Tournament);

            if (!result)
            { 
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Updated";
                return Page();
            }

            return RedirectToPage("/Tournaments/Index");
        }
    }
}
