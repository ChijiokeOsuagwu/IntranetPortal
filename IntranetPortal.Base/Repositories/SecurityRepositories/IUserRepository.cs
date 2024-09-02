using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.SecurityRepositories
{
    public interface IUserRepository
    {
        Task<IList<ApplicationUser>> GetUsersByLoginIdAsync(string login);
        Task<IList<ApplicationUser>> GetUsersByUserIdAsync(string userId);
        Task<bool> AddUserAccountAsync(ApplicationUser applicationUser);
        Task<bool> UpdateUserPasswordAsync(ApplicationUser applicationUser);
        Task<IList<ApplicationUser>> GetOtherUsersWithSameLoginIdAsync(string userId, string login);
        Task<bool> UpdateUserAccountAsync(ApplicationUser applicationUser);
        Task<bool> UpdateUserActivationAsync(string userId, string modifiedBy, bool deactivate);
        Task<bool> DeleteUserAccountByIdAsync(string userId);
    }
}
