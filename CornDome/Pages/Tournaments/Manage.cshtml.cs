using CornDome.Models.Tournaments;
using CornDome.Models.Users;
using CornDome.Repository;
using CornDome.Repository.Tournaments;
using CornDome.TournamentSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Pages.Tournaments
{
    [Authorize(Policy = "admin")]
    public class ManageModel(TournamentContext tournamentContext, IUserRepository userRepository) : PageModel
    {
        public Tournament Tournament { get; set; }

        [BindProperty]
        public int TournamentId { get; set; }

        public List<TournamentRegistration> Registrations { get; set; } = [];
        public List<User> Users { get; set; } = [];

        [BindProperty]
        public int UserToAddId { get; set; } = -1;

        public TournamentManager TournamentManager { get; set; }

        [BindProperty]
        public MatchResult PostResult { get; set; }
        [BindProperty]
        public int PostMatchId { get; set; }

        public void OnGet()
        {
            Load();
        }

        private async void Load()
        {
            if (TournamentId <= 0)
            {
                var queryId = Request.Query["id"];
                TournamentId = int.Parse(queryId);
            }

            Tournament = tournamentContext.Tournaments
                .Include(x => x.Rounds)
                .ThenInclude(y => y.Matches)
                .SingleOrDefault(x => x.Id == TournamentId);

            Registrations = [.. tournamentContext.Registrations.Where(x => x.TournamentId == Tournament.Id)];
            var allUsers = await userRepository.GetAll();
            Users = allUsers.ToList();

            foreach (var user in Registrations)
            {
                user.User = Users.FirstOrDefault(x => x.Id == user.UserId);
            }

            TournamentManager = new TournamentManager(TournamentId, tournamentContext, userRepository);
        }

        public IActionResult OnPostMatchResult()
        {
            Load();

            TournamentManager.ReportResult(PostMatchId, PostResult);

            Load();

            return Page();
        }

        public IActionResult OnPostEndTournament()
        {
            Load();
            Tournament.Status = TournamentStatus.Completed;
            tournamentContext.SaveChanges();
            return Page();
        }

        public IActionResult OnPostAddRound()
        {
            Load();

            TournamentManager.PairNextRound();

            return Page();
        }

        public async Task<IActionResult> OnPostAddUser()
        {
            if (UserToAddId != -1 && TournamentId > -1)
            {
                var user = await userRepository.GetUserById(UserToAddId);
                var reg = new TournamentRegistration()
                {
                    Deck = "Test",
                    TournamentId = TournamentId,
                    UserId = user.Id
                };

                tournamentContext.Registrations.Add(reg);
                tournamentContext.SaveChanges();
            }

            Load();
            return Page();
        }
    }
}
