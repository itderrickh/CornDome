using CornDome.Models.Tournaments;
using CornDome.Models.Users;
using Dapper;

namespace CornDome.Repository
{
    public interface ITournamentRepository
    {
        int InsertTournament(Tournament tournament);
        bool UpdateTournament(int id, Tournament tournament);
        bool DeleteTournament(int id);
        List<Tournament> GetAllTournaments();
        Tournament GetById(int id);
        bool RegisterForTournament(Tournament tournament, TournamentRegistration registration);
        List<TournamentRegistration> GetAllRegisteredUsers(int tournamentId);
        TournamentRegistration? GetRegistration(int userId, int tournamentId);
        bool DeleteRegistration(int userId, int tournamentId);
        bool UpdateRegistration(TournamentRegistration updatedRegistration);
    }

    public class TournamentRepository(IDbConnectionFactory dbConnectionFactory) : ITournamentRepository
    {
        private const string INSERT_TOURNAMENT_QUERY = @"
            INSERT INTO
	            Tournament (TournamentDate, Name, Description, Status)
            VALUES 
	            (@TournamentDate, @TournamentName, @TournamentDescription, @Status);

            SELECT LAST_INSERT_ROWID();
        ";

        private const string INSERT_RESULT_QUERY = @"
            INSERT INTO
	            TournamentResult (TournamentId, Placement, Name, Decklist)
            VALUES 
	            (@TournamentId, @Placement, @Name, @Decklist);
        ";

        private const string UPDATE_TOURNAMENT_QUERY = @"
            UPDATE Tournament
            SET
                TournamentDate = @TournamentDate,
                Name = @TournamentName,
                Description = @TournamentDescription,
                Status = @Status
            WHERE Id = @TournamentId;
        ";

        private const string REGISTER_TOURNAMENT_QUERY = @"
            INSERT INTO
                TournamentRegistration (UserId, TournamentId, Deck)
            VALUES
                (@UserId, @TournamentId, @Deck);
        ";

        public int InsertTournament(Tournament tournament)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournamentId = con.QueryFirstOrDefault<int>(INSERT_TOURNAMENT_QUERY, new { TournamentDate = tournament.TournamentDate.ToShortDateString(), tournament.TournamentName, tournament.TournamentDescription, Status = TournamentStatus.NotActive });

            return tournamentId;
        }

        public List<Tournament> GetAllTournaments()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournaments = con.Query<Tournament>("SELECT Id, TournamentDate, Name AS TournamentName, Description AS TournamentDescription, Status from Tournament");

            return tournaments.ToList();
        }

        public Tournament GetById(int id)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournament = con.QueryFirstOrDefault<Tournament>("SELECT Id, TournamentDate, Name AS TournamentName, Description AS TournamentDescription, Status from Tournament WHERE Id = @Id", new { Id = id });

            return tournament;
        }

        public bool UpdateTournament(int id, Tournament tournament)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var changed = con.Execute(UPDATE_TOURNAMENT_QUERY, new { TournamentId = id, TournamentDate = tournament.TournamentDate.ToShortDateString(), tournament.TournamentName, tournament.TournamentDescription, Status = tournament.Status });

            return changed > 0;
        }

        public bool RegisterForTournament(Tournament tournament, TournamentRegistration registration)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var result = con.Execute(REGISTER_TOURNAMENT_QUERY, registration);
            return result > 0;
        }

        private const string GET_REGISTRATIONS_QUERY = @"
            SELECT UserId, TournamentId, Deck FROM TournamentRegistration
            WHERE TournamentId = @TournamentId;
        ";
        public List<TournamentRegistration> GetAllRegisteredUsers(int tournamentId)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var registrations = con.Query<TournamentRegistration>(GET_REGISTRATIONS_QUERY, new { TournamentId = tournamentId });
            var users = con.Query<User>("SELECT Id, Username FROM User WHERE Id IN @Ids", new { Ids = registrations.Select(x => x.UserId) });

            foreach (var registration in registrations)
            {
                registration.User = users.FirstOrDefault(x => x.Id == registration.UserId);
            }
            return registrations.ToList();
        }

        public bool DeleteTournament(int tournamentId)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var deleted = con.Execute("DELETE FROM Tournament WHERE Id = @TournamentId", new { TournamentId = tournamentId });
            return deleted > 0;
        }

        private const string GET_REGISTRATIONS_BY_USERNAME_QUERY = @"
            SELECT UserId, TournamentId, Deck FROM TournamentRegistration
            WHERE UserId = @UserId AND TournamentId = @TournamentId;
        ";
        public TournamentRegistration? GetRegistration(int userId, int tournamentId)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var registration = con.Query<TournamentRegistration>(GET_REGISTRATIONS_BY_USERNAME_QUERY, new { UserId = userId, TournamentId = tournamentId });

            return registration.FirstOrDefault();
        }

        private const string UPDATE_REGISTRATION_QUERY = @"
            UPDATE TournamentRegistration
            SET
                Deck = @Deck
            WHERE UserId = @UserId AND TournamentId = @TournamentId;
        ";
        public bool UpdateRegistration(TournamentRegistration updatedRegistration)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var updated = con.Execute(UPDATE_REGISTRATION_QUERY, new { updatedRegistration.Deck, updatedRegistration.UserId, updatedRegistration.TournamentId });

            return updated > 0;
        }

        private const string DELETE_REGISTRATION_QUERY = @"
            DELETE FROM TournamentRegistration
            WHERE UserId = @UserId AND TournamentId = @TournamentId;
        ";
        public bool DeleteRegistration(int userId, int tournamentId)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var updated = con.Execute(DELETE_REGISTRATION_QUERY, new { UserId = userId, TournamentId = tournamentId });

            return updated > 0;
        }
    }
}
