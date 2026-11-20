using CornDome.Models.Users;
using CornDome.Repository.Users;
using Microsoft.AspNetCore.Identity;

namespace CornDome.Stores
{
    public class UserRoleStore(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository) : IUserRoleStore<User>
    {
        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await roleRepository.FindByName(roleName);
            await userRoleRepository.AddToRole(user, role);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var result = await userRepository.CreateUser(new User() { Email = user.Email, UserName = user.UserName });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User creation failed." });
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var result = await userRepository.UpdateUser(new User() { UserName = user.UserName, Email = user.Email });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User update failed." });
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetUserById(int.Parse(userId));
            return result;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetUserByUsername(normalizedUserName);
            return result;
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetUserByEmail(normalizedEmail);
            return result;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            var roles = await userRoleRepository.GetRolesForUser(user);
            return roles.Select(x => x.Name).ToList();
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = await roleRepository.FindByName(roleName);
            var usersInRole = await userRoleRepository.GetUsersInRole(role);
            return usersInRole.ToList();
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await roleRepository.FindByName(roleName);
            var isInRole = await userRoleRepository.IsInRole(user, role);
            return isInRole;
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await roleRepository.FindByName(roleName);
            await userRoleRepository.RemoveFromRole(user, role);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
            return Task.CompletedTask;
        }
    }
}
