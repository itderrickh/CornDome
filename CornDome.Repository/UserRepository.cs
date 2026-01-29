using CornDome.Models.Users;
using Dapper;

namespace CornDome.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmail(string email);
        Task<bool> UpdateUser(User user);
        Task<bool> CreateUser(User user);
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByUsername(string username);
        Task<IEnumerable<User>> GetAll();
        Task<DiscordConnection> GetDiscordConnection(int userId);
        Task<bool> InsertDiscordConnection(DiscordConnection discordConnection);
        Task<bool> UpdateDiscordTokens(DiscordConnection discordConnection);
        Task<bool> RemoveDiscordConnection(int userId);
    }

    public class UserRepository(IDbConnectionFactory dbConnectionFactory) : IUserRepository
    {
        public async Task<User?> GetUserByEmail(string email)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var user = await con.QueryFirstOrDefaultAsync<User>("SELECT Id, Email, UserName FROM User WHERE Email = @Email", new { Email = email });

            return user;
        }

        public async Task<User?> GetUserById(int id)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var user = await con.QueryFirstOrDefaultAsync<User>("SELECT Id, Email, Username FROM User WHERE Id = @Id", new { Id = id });

            return user;
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var user = await con.QueryFirstOrDefaultAsync<User>("SELECT Id, Email, Username FROM User WHERE Username = @Username", new { Username = username });

            return user;
        }

        public async Task<bool> UpdateUser(User user)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var value = await con.ExecuteAsync("UPDATE User SET Username = @Username WHERE Email = @Email", new { Email = user.Email, Username = user.Username });
            return value > 0;
        }

        public async Task<bool> CreateUser(User user)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var value = await con.ExecuteAsync("INSERT INTO User (Email, Username) VALUES (@Email, @Username);", new { user.Email, user.Username });

            return value > 0;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var users = await con.QueryAsync<User>("SELECT Id, Email, Username FROM User");

            return users;
        }

        public Task<DiscordConnection> GetDiscordConnection(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertDiscordConnection(DiscordConnection discordConnection)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateDiscordTokens(DiscordConnection discordConnection)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveDiscordConnection(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
