using CornDome.Models.Users;
using Dapper;

namespace CornDome.Repository
{
    public interface IRoleRepository
    {
        Task<bool> CreateRole(Role role);
        Task<bool> UpdateRole(Role role);
        Task<bool> DeleteRole(Role role);
        Task<Role?> FindById(int id);
        Task<Role?> FindByName(string roleName);
        Task<IEnumerable<Role>> GetAll();
    }
    public class RoleRepository(IDbConnectionFactory dbConnectionFactory) : IRoleRepository
    {        
        public async Task<bool> CreateRole(Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "INSERT INTO Role (Id, Name) VALUES (@Id, @Name)";
            var result = await con.ExecuteAsync(sql, role);
            return result > 0;
        }

        public async Task<bool> UpdateRole(Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "UPDATE Role SET Name = @Name WHERE Id = @Id";
            var result = await con.ExecuteAsync(sql, role);
            return result > 0;
        }

        public async Task<bool> DeleteRole(Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "DELETE FROM Role WHERE Id = @Id";
            var result = await con.ExecuteAsync(sql, role);
            return result > 0;
        }

        public async Task<Role?> FindById(int roleId)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "SELECT * FROM Role WHERE Id = @RoleId";
            return await con.QueryFirstOrDefaultAsync<Role>(sql, new { RoleId = roleId });
        }

        public async Task<Role?> FindByName(string roleName)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "SELECT * FROM Role WHERE Name = @RoleName";
            return await con.QueryFirstOrDefaultAsync<Role>(sql, new { RoleName = roleName });
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            string sql = "SELECT * FROM Role";
            return await con.QueryAsync<Role>(sql);
        }
    }
}
