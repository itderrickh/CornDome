using CornDome.Models;
using Dapper;
using System.Data;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public interface IUserRoleRepository
    {
        bool AddToRole(User user, Role role);
        bool RemoveFromRole(User user, Role role);
        bool IsInRole(User user, Role role);
        IEnumerable<Role> GetRolesForUser(User user);
        IEnumerable<User> GetUsersInRole(Role role);
    }

    public class UserRoleRepository(IDbConnectionFactory dbConnectionFactory) : IUserRoleRepository
    {
        public bool AddToRole(User user, Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "INSERT INTO UserRole (UserId, RoleId) VALUES (@UserId, @RoleId)";
            var result = con.Execute(sql, new { UserId = user.Id, RoleId = role.Id });
            return result > 0;
        }

        public IEnumerable<Role> GetRolesForUser(User user)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = @"
                SELECT Id, Name FROM Role r
                LEFT JOIN UserRole ur ON ur.RoleId = r.Id
                WHERE ur.UserId = @UserId";
            var result = con.Query<Role>(sql, new { UserId = user.Id });
            return result;
        }

        public IEnumerable<User> GetUsersInRole(Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = @"
                SELECT Id, Email, Username FROM User u
                LEFT JOIN UserRole ur ON ur.UserId = u.Id
                WHERE ur.RoleId = @RoleId";
            var result = con.Query<User>(sql, new { RoleId = role.Id });
            return result;
        }

        public bool IsInRole(User user, Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "SELECT UserId, RoleId FROM UserRole WHERE UserId = @UserId AND RoleId = @RoleId)";
            var result = con.Query<UserRole>(sql, new { UserId = user.Id, RoleId = role.Id });
            return result != null && result.Any();
        }

        public bool RemoveFromRole(User user, Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "DELETE FROM UserRole WHERE UserId = @UserId AND RoleId = @RoleId)";
            var result = con.Execute(sql, new { UserId = user.Id, RoleId = role.Id });
            return result > 0;
        }
    }
}
