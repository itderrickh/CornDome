using CornDome.Models;
using Dapper;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
    }

    public class UserRepository(UserRepositoryConfig config) : IUserRepository
    {
        public User GetUserByUsername(string username)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var user = con.QueryFirstOrDefault<User>("SELECT Id, Username, PasswordHash FROM User WHERE Username = @Username", new { Username = username });

            return user;
        }
    }
}
