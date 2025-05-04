using CornDome.Models.Tournaments;
using CornDome.Models.Users;
using CornDome.Repository;
using CornDome.Repository.Tournaments;
using CornDome.TournamentSystem;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Tournaments
{
    public class ViewModel(TournamentContext tournamentContext, IUserRepository userRepository) : PageModel
    {
        public Tournament Tournament { get; set; }
        public List<TournamentRegistration> RegisteredUsers { get; set; } = [];
        private List<User> Users { get; set; } = [];
        public TournamentManager TournamentManager { get; set; }

        public int TournamentId { get; set; }
        public async void OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);

            var users = await userRepository.GetAll();
            Users = users.ToList();
            Tournament = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == TournamentId);
            RegisteredUsers = tournamentContext.Registrations.Where(x => x.TournamentId == Tournament.Id).ToList();

            foreach (var user in RegisteredUsers)
            {
                user.User = Users.FirstOrDefault(x => x.Id == user.UserId);
            }

            TournamentManager = new TournamentManager(TournamentId, tournamentContext, userRepository);
        }
    }
}
