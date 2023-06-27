using IntranetPortal.Base.Models.SecurityModels;
//using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface ISecurityService
    {
        //=================== User Accounts Action Methods =============================================//
        #region User Accounts Action Methods
        Task<IList<EmployeeUser>>  GetEmployeeUsersByNameAsync(string fullName);
        Task<IList<EmployeeUser>> SearchEmployeeUsersByNameAsync(string fullName);
        Task<IList<EmployeeUser>> GetAllEmployeeUsersAsync();
        Task<IList<ApplicationUser>> GetUsersByLoginId(string loginId);
        Task<IList<ApplicationUser>> GetUsersByUserId(string userId);
        Task<bool> CreateUserAccountAsync(ApplicationUser user);
        Task<bool> ResetUserPasswordAsync(ApplicationUser user);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> LoginIdIsAvailable(string userId, string loginId);
        Task<bool> DeleteUserAsync(string userId, string deletedBy);
        #endregion

        //======================= User Permissions Interface ================================//
        #region User Permissions Interfaces
        Task<IList<UserPermission>> GetUserPermissionsByUserIdAsync(string userId);
        Task<IList<string>> GetUserPermissionListByUserIdAsync(string userId);
        Task<IList<UserPermission>> GetUserPermissionsByUserIdAndAppIdAsync(string userId, string applicationId);
        Task<IList<UserPermission>> GetAllUserPermissionsByUserIdAsync(string userId);
        Task<IList<UserPermission>> GetAllUserPermissionsByUserIdAndApplicationIdAsync(string userId, string applicationId);

        Task<bool> GrantPermissionAsync(string userId, string roleId, string actionBy);
        Task<bool> RevokePermissionAsync(string userId, string roleId, string actionBy);
        #endregion

        //======================= Role Interfaces ============================================//
        #region Role Interfaces
        Task<IList<ApplicationRole>> GetUserRolesUnGrantedByUserIDAsync(string userId, string applicationId = null);
        #endregion
        //======================= User Login History Action Interfaces
        #region UserLoginHistory Action Interfaces
        Task<bool> UpdateUserLoginHistoryAsync(UserLoginHistory userLoginHistory);
        Task<IList<UserLoginHistory>> GetUserLoginHistoryByUserNameAndDateAsync(string UserName, int? LoginYear = null, int? LoginMonth = null, int? LoginDay = null);
        Task<IList<UserLoginHistory>> GetUserLoginHistoryByDateOnlyAsync(int? LoginYear = null, int? LoginMonth = null, int? LoginDay = null);
        #endregion

        //====================== Security Cryptography Interfaces ==========================//
        #region Security Cryptography Interfaces
        string CreatePasswordHash(string plainTextPassword);
        bool ValidatePassword(string plainTextPassword, string hashedPassword);
        #endregion
    }
}
