using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.SecurityRepositories
{
    public interface IUserPermissionRepository
    {
        Task<bool> AddUserPermissionAsync(string userId, string roleId, string modifiedBy);
        Task<bool> DeleteUserPermissionAsync(string userId, string roleId, string modifiedBy);
        Task<bool> DeleteUserPermissionAsync(int userPermissionId);
        Task<bool> DeleteUserPermissionByUserIdAsync(string userId);
        Task<IList<UserPermission>> GetUserPermissionsByUserIdAsync(string userId);
        Task<IList<UserPermission>> GetUserPermissionsByUserIdAndRoleIdAsync(string userId, string roleId);
        Task<IList<UserPermission>> GetUserPermissionsByUserIdAndApplicationIdAsync(string userId, string applicationId);
        Task<IList<UserPermission>> GetFullUserPermissionsByUserIdAsync(string userId);
        Task<IList<UserPermission>> GetFullUserPermissionsByUserIdAndApplicationIdAsync(string userId, string applicationId);
    }
}
