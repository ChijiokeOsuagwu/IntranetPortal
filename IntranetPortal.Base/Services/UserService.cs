using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.SecurityRepositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class UserService : IDisposable, IUserStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUtilityRepository _utilityRepository;

        public UserService(IUserRepository userRepository, IUtilityRepository utilityRepository)
        {
            _userRepository = userRepository;
            _utilityRepository = utilityRepository;
        }

        public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            bool UserIsAdded = false;
            if (user == null) { throw new ArgumentNullException(nameof(user), "The required parameter [user] is missing."); }
            try
            {
                UserIsAdded = await _userRepository.AddUserAccountAsync(user);
                if (UserIsAdded)
                {
                    ActivityHistory history = new ActivityHistory{ 
                    ActivityDetails = $"A new User Account belonging to {user.FullName} with Login ID [{user.UserName}] was created  on {user.CreatedTime} by {user.CreatedBy}.",
                    ActivityHistoryId = 0,
                    ActivityMachineName = "",
                    ActivitySource = "",
                    ActivitySourceIP="",
                    ActivityTime = DateTime.UtcNow,
                    ActivityTimeZone = "",
                    ActivityUserFullName = user.FullName,
                    };
                    await _utilityRepository.InsertActivityHistoryAsync(history);
                    return IdentityResult.Success;
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Sorry, an error was encountered while attempting to save the new user account details in the database."}); 
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            ApplicationUser user = new ApplicationUser();
            try
            {
                var entities = await _userRepository.GetUsersByUserIdAsync(userId);
                List<ApplicationUser> users = entities.ToList();
                if (users.Count > 0)
                {
                    user = users.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            ApplicationUser user = new ApplicationUser();
            try
            {
                var entities = await _userRepository.GetUsersByLoginIdAsync(normalizedUserName);
                List<ApplicationUser> users = entities.ToList();
                if(users.Count > 0)
                {
                    user = users.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return user;
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string normalizedUserName = string.Empty;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            normalizedUserName = user.UserName.ToUpper();
            return Task.FromResult<string>(normalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string passwordHash = string.Empty;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            passwordHash = user.PasswordHash;
            return Task.FromResult<string>(passwordHash);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string userId = string.Empty;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            userId = user.Id;
            return Task.FromResult<string>(userId);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string userName = string.Empty;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            userName = user.UserName;
            return Task.FromResult<string>(userName);
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            string password = string.Empty;
            if (user == null){throw new ArgumentNullException("user"); }
            password = user.PasswordHash;
            if (string.IsNullOrEmpty(password)) { return Task.FromResult(false); }
            return Task.FromResult(true);
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            string NormalizedUserName = string.Empty;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(user);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            string PasswordHash = string.Empty;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(user);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            string UserName = string.Empty;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.UserName = userName;
            return Task.FromResult(user);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
