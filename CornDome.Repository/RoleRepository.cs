using CornDome.Models;
using Dapper;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public interface IRoleRepository
    {
        bool CreateRole(Role role);
        bool UpdateRole(Role role);
        bool DeleteRole(Role role);
        Role? FindById(int id);
        Role? FindByName(string roleName);
    }
    public class RoleRepository(SqliteRepositoryConfig config) : IRoleRepository
    {
        private SqliteRepositoryConfig _config { get; set; } = config;
        
        public bool CreateRole(Role role)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            string sql = "INSERT INTO Roles (Id, Name) VALUES (@Id, @Name)";
            var result = con.Execute(sql, role);
            return result > 0;
        }

        public bool UpdateRole(Role role)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            string sql = "UPDATE Roles SET Name = @Name WHERE Id = @Id";
            var result = con.Execute(sql, role);
            return result > 0;
        }

        public bool DeleteRole(Role role)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            string sql = "DELETE FROM Roles WHERE Id = @Id";
            var result = con.Execute(sql, role);
            return result > 0;
        }

        public Role? FindById(int roleId)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            string sql = "SELECT * FROM Roles WHERE Id = @RoleId";
            return con.QueryFirstOrDefault<Role>(sql, new { RoleId = roleId });
        }

        public Role? FindByName(string roleName)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            string sql = "SELECT * FROM Roles WHERE Name = @RoleName";
            return con.QueryFirstOrDefault<Role>(sql, new { RoleName = roleName });
        }
    }
}
