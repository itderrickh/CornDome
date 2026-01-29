using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Identity;

namespace CornDome.Stores
{
    public class UserStore(IUserRepository userRepository) : IUserStore<User>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var result = await _userRepository.CreateUser(new User() { Email = user.Email, UserName = user.UserName });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User creation failed." });
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var result = await _userRepository.UpdateUser(new User() { UserName = user.UserName, Email = user.Email });
            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User update failed." });
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetUserById(int.Parse(userId));
            return result;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetUserByUsername(normalizedUserName);
            return result;
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetUserByEmail(normalizedEmail);
            return result;
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

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }
    }
}
