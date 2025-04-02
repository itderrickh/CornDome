﻿using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Identity;

namespace CornDome.Stores
{
    public class UserRoleStore(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository) : IUserRoleStore<User>
    {
        private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = _roleRepository.FindByName(roleName);
            _userRoleRepository.AddToRole(user, role);
            return Task.CompletedTask;
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
            var result = _userRepository.CreateUser(new User() { Email = user.Email, Username = user.UserName });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User creation failed." });
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var result = _userRepository.UpdateUser(new User() { Username = user.UserName, Email = user.Email });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User update failed." });
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var result = _userRepository.GetUserById(int.Parse(userId));
            return result;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = _userRepository.GetUserByUsername(normalizedUserName);
            return result;
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var result = _userRepository.GetUserByEmail(normalizedEmail);
            return result;
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return user.NormalizedUserName;
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            var roles = _userRoleRepository.GetRolesForUser(user);
            return roles.Select(x => x.Name).ToList();
        }

        public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return user.Id.ToString();
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return user.UserName;
        }

        public async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = _roleRepository.FindByName(roleName);
            return _userRoleRepository.GetUsersInRole(role).ToList();
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = _roleRepository.FindByName(roleName);
            var isInRole = _userRoleRepository.IsInRole(user, role);
            return isInRole;
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = _roleRepository.FindByName(roleName);
            _userRoleRepository.RemoveFromRole(user, role);
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
            return Task.CompletedTask;
        }
    }
}
