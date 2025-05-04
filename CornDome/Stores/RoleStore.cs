using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Identity;

namespace CornDome.Stores
{
    public class RoleStore(IRoleRepository roleRepository) : IRoleStore<Role>
    {
        private readonly IRoleRepository _roleRepo = roleRepository;

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            var result = await _roleRepo.CreateRole(new Role() { Id = role.Id, Name = role.Name });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Role creation failed." });
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            var result = await _roleRepo.UpdateRole(new Role() { Id = role.Id, Name = role.Name });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Role update failed." });
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            var result = await _roleRepo.DeleteRole(new Role() { Id = role.Id, Name = role.Name });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Role deletion failed." });
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var result = await _roleRepo.FindById(int.Parse(roleId));
            return result;
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var result = await _roleRepo.FindByName(normalizedRoleName);
            return result;
        }

        public void Dispose()
        {
        }

        public async Task<int> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return await Task.FromResult(role.Id);
        }

        public async Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return await Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return await Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.Name = normalizedName;
            return Task.CompletedTask;
        }

        async Task<string> IRoleStore<Role>.GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            var result = await GetRoleIdAsync(role, cancellationToken);
            return result.ToString();
        }
    }
}
