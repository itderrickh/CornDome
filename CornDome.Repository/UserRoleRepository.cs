using CornDome.Models.Users;

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

    public class UserRoleRepository(MainContext context) : IUserRoleRepository
    {
        public async Task<bool> AddToRole(User user, Role role)
        {
            try
            {
                UserRole userRole = new UserRole();
                userRole.RoleId = role.Id;
                userRole.UserId = user.Id;

                context.Add(userRole);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Role>> GetRolesForUser(User user)
        {
            var userRoles = context.UserRoles
                .Where(x => x.UserId == user.Id)
                .Select(y => y.Role)
                .Distinct()
                .ToList();
            return userRoles;
        }

        public async Task<IEnumerable<User>> GetUsersInRole(Role role)
        {
            var users = context.UserRoles
                .Where(x => x.RoleId == role.Id)
                .Select(y => y.User)
                .Distinct()
                .ToList();
            return users;
        }

        public async Task<bool> IsInRole(User user, Role role)
        {
            var userRole = context.UserRoles.FirstOrDefault(x => x.RoleId == role.Id && x.UserId == user.Id);

            return userRole != null;
        }

        public async Task<bool> RemoveFromRole(User user, Role role)
        {
            try
            {
                var userRoleToDelete = context.UserRoles.FirstOrDefault(x => x.RoleId == role.Id && x.UserId == user.Id);

                if (userRoleToDelete != null)
                {
                    context.Remove(userRoleToDelete);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public async Task<IEnumerable<UserRole>> GetAll()
        {
            return context.UserRoles;
        }
    }
}
