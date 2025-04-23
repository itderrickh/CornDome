using CornDome.Models.Users;
using Dapper;

namespace CornDome.Repository
{
    public interface IUserRepository
    {
        User? GetUserByEmail(string email);
        bool UpdateUser(User user);
        bool CreateUser(User user);
        User? GetUserById(int id);
        User? GetUserByUsername(string username);
        IEnumerable<User> GetAll();
    }

    public class UserRepository(IDbConnectionFactory dbConnectionFactory) : IUserRepository
    {
        public User? GetUserByEmail(string email)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var user = con.QueryFirstOrDefault<User>("SELECT Id, Email, UserName FROM User WHERE Email = @Email", new { Email = email });

            return user;
        }

        public User? GetUserById(int id)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var user = con.QueryFirstOrDefault<User>("SELECT Id, Email, Username FROM User WHERE Id = @Id", new { Id = id });

            return user;
        }

        public User? GetUserByUsername(string username)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var user = con.QueryFirstOrDefault<User>("SELECT Id, Email, Username FROM User WHERE Username = @Username", new { Username = username });

            return user;
        }

        public bool UpdateUser(User user)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var value = con.Execute("UPDATE User SET Username = @Username WHERE Email = @Email", new { Email = user.Email, Username = user.Username });
            return value > 0;
        }

        public bool CreateUser(User user)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var value = con.Execute("INSERT INTO User (Email, Username) VALUES (@Email, @Username);", new { user.Email, user.Username });

            return value > 0;
        }

        public IEnumerable<User> GetAll()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var users = con.Query<User>("SELECT Id, Email, Username FROM User");

            return users;
        }
    }
}
