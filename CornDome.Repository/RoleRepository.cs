using CornDome.Models.Users;
using Dapper;

namespace CornDome.Repository
{
    public interface IRoleRepository
    {
        bool CreateRole(Role role);
        bool UpdateRole(Role role);
        bool DeleteRole(Role role);
        Role? FindById(int id);
        Role? FindByName(string roleName);
        public IEnumerable<Role> GetAll();
    }
    public class RoleRepository(IDbConnectionFactory dbConnectionFactory) : IRoleRepository
    {        
        public bool CreateRole(Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "INSERT INTO Role (Id, Name) VALUES (@Id, @Name)";
            var result = con.Execute(sql, role);
            return result > 0;
        }

        public bool UpdateRole(Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "UPDATE Role SET Name = @Name WHERE Id = @Id";
            var result = con.Execute(sql, role);
            return result > 0;
        }

        public bool DeleteRole(Role role)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "DELETE FROM Role WHERE Id = @Id";
            var result = con.Execute(sql, role);
            return result > 0;
        }

        public Role? FindById(int roleId)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "SELECT * FROM Role WHERE Id = @RoleId";
            return con.QueryFirstOrDefault<Role>(sql, new { RoleId = roleId });
        }

        public Role? FindByName(string roleName)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "SELECT * FROM Role WHERE Name = @RoleName";
            return con.QueryFirstOrDefault<Role>(sql, new { RoleName = roleName });
        }

        public IEnumerable<Role> GetAll()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            string sql = "SELECT * FROM Role";
            return con.Query<Role>(sql);
        }
    }
}
