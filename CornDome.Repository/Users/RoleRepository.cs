using CornDome.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository.Users
{
    public interface IRoleRepository
    {
        Task<bool> CreateRole(Role role);
        Task<bool> UpdateRole(Role role);
        Task<bool> DeleteRole(Role role);
        Task<Role> FindById(int id);
        Task<Role> FindByName(string roleName);
        Task<IEnumerable<Role>> GetAll();
        IQueryable<Role> GetAllRolesQueryable();
    }
    public class RoleRepository(MainContext context) : IRoleRepository
    {        
        public async Task<bool> CreateRole(Role role)
        {
            try
            {
                context.Add(role);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateRole(Role role)
        {
            try
            {
                var dbRole = context.Roles.FirstOrDefault(x => x.Id == role.Id);
                dbRole?.Name = role.Name;

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteRole(Role role)
        {
            try
            {
                var existingRole = await context.Set<Role>()
                    .FirstOrDefaultAsync(r => r.Id == role.Id);

                if (existingRole == null)
                    return false;

                context.Remove(existingRole);

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Role> FindById(int roleId)
        {
            return context.Roles.FirstOrDefault(r => r.Id == roleId);
        }

        public async Task<Role> FindByName(string roleName)
        {
            return context.Roles.FirstOrDefault(r => r.Name == roleName);
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            return context.Roles;
        }

        public IQueryable<Role> GetAllRolesQueryable()
        {
            return context.Roles.AsNoTracking();
        }
    }
}
