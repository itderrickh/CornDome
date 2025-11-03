using CornDome.Models.Users;
using Dapper;

namespace CornDome.Repository
{
    public interface IUserRoleRepository
    {
        Task<bool> AddToRole(User user, Role role);
        Task<bool> RemoveFromRole(User user, Role role);
        Task<bool> IsInRole(User user, Role role);
        Task<IEnumerable<Role>> GetRolesForUser(User user);
        Task<IEnumerable<User>> GetUsersInRole(Role role);
        Task<IEnumerable<UserRole>> GetAll();
    }

    public class UserRoleRepository(IDbConnectionFactory dbConnectionFactory) : IUserRoleRepository
    {
        public async Task<bool> AddToRole(User user, Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "INSERT INTO UserRole (UserId, RoleId) VALUES (@UserId, @RoleId)";
            var result = await con.ExecuteAsync(sql, new { UserId = user.Id, RoleId = role.Id });
            return result > 0;
        }

        public async Task<IEnumerable<Role>> GetRolesForUser(User user)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = @"
                SELECT Id, Name FROM Role r
                LEFT JOIN UserRole ur ON ur.RoleId = r.Id
                WHERE ur.UserId = @UserId";
            var result = await con.QueryAsync<Role>(sql, new { UserId = user.Id });
            return result;
        }

        public async Task<IEnumerable<User>> GetUsersInRole(Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = @"
                SELECT Id, Email, Username FROM User u
                LEFT JOIN UserRole ur ON ur.UserId = u.Id
                WHERE ur.RoleId = @RoleId";
            var result = await con.QueryAsync<User>(sql, new { RoleId = role.Id });
            return result;
        }

        public async Task<bool> IsInRole(User user, Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "SELECT UserId, RoleId FROM UserRole WHERE UserId = @UserId AND RoleId = @RoleId)";
            var result = await con.QueryAsync<UserRole>(sql, new { UserId = user.Id, RoleId = role.Id });
            return result != null && result.Any();
        }

        public async Task<bool> RemoveFromRole(User user, Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "DELETE FROM UserRole WHERE UserId = @UserId AND RoleId = @RoleId)";
            var result = await con.ExecuteAsync(sql, new { UserId = user.Id, RoleId = role.Id });
            return result > 0;
        }

        public async Task<IEnumerable<UserRole>> GetAll()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "SELECT UserId, RoleId FROM UserRole";
            var result = await con.QueryAsync<UserRole>(sql);
            return result;
        }
    }
}
