using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Identity;

namespace CornDome.Stores
{
    public class UserStore : IUserStore<User>
    {
        private readonly IUserRepository _userRepository;

        public UserStore(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return user.UserName;
        }

        public async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }
    }
}
