using CornDome.Models;
using Dapper;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public interface IUserRepository
    {
        User? GetUserByEmail(string email);
        UserPermission? GetUserPermission(string email);
        bool UpdateUser(User user);
        bool CreateUser(User user);
        User? GetUserById(int id);
        User? GetUserByUsername(string username);
    }

    public class UserRepository(UserRepositoryConfig config) : IUserRepository
    {
        public User? GetUserByEmail(string email)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var user = con.QueryFirstOrDefault<User>("SELECT Id, Email, UserName FROM User WHERE Email = @Email", new { Email = email });

            return user;
        }

        public User? GetUserById(int id)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var user = con.QueryFirstOrDefault<User>("SELECT Id, Email, Username FROM User WHERE Id = @Id", new { Id = id });

            return user;
        }

        public User? GetUserByUsername(string username)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var user = con.QueryFirstOrDefault<User>("SELECT Id, Email, Username FROM User WHERE Username = @Username", new { Username = username });

            return user;
        }

        public UserPermission? GetUserPermission(string email)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            // TODO: Clean this up later to use join
            var user = GetUserByEmail(email);
            var permission = con.QueryFirstOrDefault<UserPermission>("SELECT Id, UserId, PermissionLevel FROM UserPermission WHERE UserId = @UserId", new { UserId = user.Id });

            return permission;
        }

        public bool UpdateUser(User user)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var value = con.Execute("UPDATE User SET Username = @Username WHERE Email = @Email", new { Email = user.Email, Username = user.Username });
            return value > 0;
        }

        public bool CreateUser(User user)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var value = con.Execute("INSERT INTO User (Email, Username) VALUES (@Email, @Username);", new { user.Email, user.Username });

            return value > 0;
        }
    }
}
