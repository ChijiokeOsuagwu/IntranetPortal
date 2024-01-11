using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Repositories.BaseRepositories;
using IntranetPortal.Base.Repositories.SecurityRepositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
//using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IEmployeeUserRepository _employeeUserRepository;
        private readonly IUtilityRepository _utilityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEntityPermissionRepository _entityPermissionRepository;

        public SecurityService(IEmployeeUserRepository employeeUserRepository, IUtilityRepository utilityRepository,
                                IUserRepository userRepository, IUserPermissionRepository userPermissionRepository,
                                IRoleRepository roleRepository, IUserLoginRepository userLoginRepository,
                                IEntityPermissionRepository entityPermissionRepository)
        {
            _employeeUserRepository = employeeUserRepository;
            _utilityRepository = utilityRepository;
            _userRepository = userRepository;
            _userPermissionRepository = userPermissionRepository;
            _userLoginRepository = userLoginRepository;
            _roleRepository = roleRepository;
            _entityPermissionRepository = entityPermissionRepository;
        }

        //============= Employee User Action Methods ==================//
        #region Employee User Action Methods
        public async Task<IList<EmployeeUser>> GetAllEmployeeUsersAsync()
        {
            List<EmployeeUser> employeeUsers = new List<EmployeeUser>();
            try
            {
                var entities = await _employeeUserRepository.GetAllAsync();
                employeeUsers = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeUsers.ToList();
        }
        public async Task<IList<EmployeeUser>> GetEmployeeUsersByNameAsync(string fullName)
        {
            List<EmployeeUser> employeeUsers = new List<EmployeeUser>();
            try
            {
                var entities = await _employeeUserRepository.GetByNameAsync(fullName);
                employeeUsers = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeUsers.ToList();
        }
        public async Task<IList<EmployeeUser>> SearchEmployeeUsersByNameAsync(string fullName)
        {
            List<EmployeeUser> employeeUsers = new List<EmployeeUser>();
            try
            {
                var entities = await _employeeUserRepository.SearchByNameAsync(fullName);
                employeeUsers = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return employeeUsers.ToList();
        }

        #endregion

        //============= User Action Methods ===========================//
        #region User Action Methods
        public async Task<IList<ApplicationUser>> GetUsersByLoginId(string loginId)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            try
            {
                var entities = await _userRepository.GetUsersByLoginIdAsync(loginId);
                applicationUsers = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return applicationUsers.ToList();
        }
        public async Task<IList<ApplicationUser>> GetUsersByUserId(string userId)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            try
            {
                var entities = await _userRepository.GetUsersByUserIdAsync(userId);
                applicationUsers = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return applicationUsers.ToList();
        }
        public async Task<bool> CreateUserAccountAsync(ApplicationUser user)
        {
            bool UserIsAdded = false;

            if (user == null) { throw new ArgumentNullException(nameof(user), "The required parameter [user] is missing."); }
            try
            {
                var entitiesWithSameUsername = await _userRepository.GetUsersByLoginIdAsync(user.UserName);
                if (entitiesWithSameUsername.Count > 0)
                {
                    throw new Exception("The Login ID you entered already exists.");
                }

                var entitiesWithSameUserId = await _userRepository.GetUsersByUserIdAsync(user.Id);
                if (entitiesWithSameUserId.Count > 0)
                {
                    throw new Exception("This user aleady has an existing user account.");
                }

                UserIsAdded = await _userRepository.AddUserAccountAsync(user);
                if (UserIsAdded)
                {
                    ActivityHistory history = new ActivityHistory
                    {
                        ActivityDetails = $"A new User Account belonging to {user.FullName} with Login ID [{user.UserName}] was created  on {user.CreatedTime} by {user.CreatedBy}.",
                        ActivityHistoryId = 0,
                        ActivityMachineName = "",
                        ActivitySource = "",
                        ActivitySourceIP = "",
                        ActivityTime = DateTime.UtcNow,
                        ActivityTimeZone = "",
                        ActivityUserFullName = user.CreatedBy,
                    };
                    return await _utilityRepository.InsertActivityHistoryAsync(history);
                }
                else
                {
                    throw new Exception("Sorry, an error was encountered while attempting to save the new user account details in the database.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> ResetUserPasswordAsync(ApplicationUser user)
        {
            bool IsSuccessful = false;

            if (user == null) { throw new ArgumentNullException(nameof(user), "The required parameter [user] is missing."); }
            if (string.IsNullOrEmpty(user.PasswordHash)) { throw new ArgumentNullException(nameof(user.PasswordHash), "The required parameter [PasswordHash] is missing."); }
            try
            {
                IsSuccessful = await _userRepository.UpdateUserPasswordAsync(user);
                if (IsSuccessful)
                {
                    ActivityHistory history = new ActivityHistory
                    {
                        ActivityDetails = $"A password reset operation was executed on a User Account belonging to {user.FullName} with Login ID [{user.UserName}] on {user.ModifiedTime} by {user.ModifiedBy}.",
                        ActivityHistoryId = 0,
                        ActivityMachineName = "",
                        ActivitySource = "",
                        ActivitySourceIP = "",
                        ActivityTime = DateTime.UtcNow,
                        ActivityTimeZone = "",
                        ActivityUserFullName = user.ModifiedBy,
                    };
                    await _utilityRepository.InsertActivityHistoryAsync(history);
                    return IsSuccessful;
                }
                else
                {
                    throw new Exception($"Sorry, an error was encountered while attempting to save the new password to the database.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            bool IsSuccessful = false;

            if (user == null) { throw new ArgumentNullException(nameof(user), "The required parameter [user] is missing."); }
            try
            {
                IsSuccessful = await _userRepository.UpdateUserAccountAsync(user);
                if (IsSuccessful)
                {
                    ActivityHistory history = new ActivityHistory
                    {
                        ActivityDetails = $"User Account belonging to {user.FullName} with Login ID [{user.UserName}] was updated on {user.ModifiedTime} by {user.ModifiedBy}.",
                        ActivityHistoryId = 0,
                        ActivityMachineName = "",
                        ActivitySource = "",
                        ActivitySourceIP = "",
                        ActivityTime = DateTime.UtcNow,
                        ActivityTimeZone = "",
                        ActivityUserFullName = user.ModifiedBy,
                    };
                    await _utilityRepository.InsertActivityHistoryAsync(history);
                    return IsSuccessful;
                }
                else
                {
                    throw new Exception($"Sorry, an error was encountered while attempting to save the changes to the database.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteUserAsync(string userId, string deletedBy)
        {
            bool PermissionsIsDeleted = false;
            bool UserIsDeleted = false;

            if (string.IsNullOrEmpty(userId)) { throw new ArgumentNullException(nameof(userId), "The required parameter [User ID] is missing."); }
            try
            {
                var users = await _userRepository.GetUsersByUserIdAsync(userId);
                if (users != null)
                {
                    ApplicationUser user = users.FirstOrDefault();
                    PermissionsIsDeleted = await _userPermissionRepository.DeleteUserPermissionByUserIdAsync(userId);
                    UserIsDeleted = await _userRepository.DeleteUserAccountByIdAsync(userId);
                    string deletedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} GMT";
                    ActivityHistory history = new ActivityHistory
                    {
                        ActivityDetails = $"User Account belonging to {user.FullName} with Login ID [{user.UserName}] was updated on {deletedTime} by {deletedBy}.",
                        ActivityHistoryId = 0,
                        ActivityMachineName = "",
                        ActivitySource = "",
                        ActivitySourceIP = "",
                        ActivityTime = DateTime.UtcNow,
                        ActivityTimeZone = "",
                        ActivityUserFullName = user.ModifiedBy,
                    };
                    await _utilityRepository.InsertActivityHistoryAsync(history);
                    return UserIsDeleted;
                }
                else
                {
                    throw new Exception($"Sorry, no record was found for the selected user.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> LoginIdIsAvailable(string userId, string loginId)
        {
            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            try
            {
                var entities = await _userRepository.GetOtherUsersWithSameLoginIdAsync(userId, loginId);
                applicationUsers = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return applicationUsers.Count < 1;
        }
        #endregion

        //============= Role Action Methods ===========================//
        #region Role Action Methods
        public async Task<IList<ApplicationRole>> GetUserRolesUnGrantedByUserIDAsync(string userId, string applicationId = null)
        {
            List<ApplicationRole> applicationRoles = new List<ApplicationRole>();
            try
            {
                if (string.IsNullOrEmpty(applicationId))
                {
                var entities = await _roleRepository.GetUnGrantedRolesByUserIdAsync(userId);
                applicationRoles = entities.ToList();
                }
                else
                {
                    var entities = await _roleRepository.GetUnGrantedRolesByUserIdAndApplicationIdAsync(userId, applicationId);
                    applicationRoles = entities.ToList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return applicationRoles.ToList();
        }

        #endregion
        
        //============= User Permission Action Methods =================//
        #region User Permission Action Methods
        public async Task<IList<UserPermission>> GetUserPermissionsByUserIdAsync(string userId)
        {
            List<UserPermission> permissions = new List<UserPermission>();
            try
            {
                var entities = await _userPermissionRepository.GetUserPermissionsByUserIdAsync(userId);
                permissions = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return permissions;
        }

        public async Task<IList<UserPermission>> GetUserPermissionsByUserIdAndAppIdAsync(string userId, string applicationId)
        {
            List<UserPermission> permissions = new List<UserPermission>();
            try
            {
                var entities = await _userPermissionRepository.GetUserPermissionsByUserIdAndApplicationIdAsync(userId, applicationId);
                permissions = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return permissions;
        }

        public async Task<IList<string>> GetUserPermissionListByUserIdAsync(string userId)
        {
            List<UserPermission> permissions = new List<UserPermission>();
            List<string> roles = new List<string>();
            try
            {
                var entities = await _userPermissionRepository.GetUserPermissionsByUserIdAsync(userId);
                permissions = entities.ToList();
                foreach (var p in permissions)
                {
                    roles.Add(p.RoleID);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return roles;
        }

        public async Task<IList<UserPermission>> GetAllUserPermissionsByUserIdAsync(string userId)
        {
            List<UserPermission> permissions = new List<UserPermission>();
            try
            {
                var entities = await _userPermissionRepository.GetFullUserPermissionsByUserIdAsync(userId);
                permissions = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return permissions;
        }

        public async Task<IList<UserPermission>> GetAllUserPermissionsByUserIdAndApplicationIdAsync(string userId, string applicationId)
        {
            List<UserPermission> permissions = new List<UserPermission>();
            try
            {
                var entities = await _userPermissionRepository.GetFullUserPermissionsByUserIdAndApplicationIdAsync(userId, applicationId);
                permissions = entities.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return permissions;
        }

        public async Task<bool> GrantPermissionAsync(string userId, string roleId, string actionBy)
        {
            bool PermissionsIsGranted = false;

            if (string.IsNullOrEmpty(userId)) { throw new ArgumentNullException(nameof(userId), "The required parameter [User ID] is missing."); }
            if (string.IsNullOrEmpty(roleId)) { throw new ArgumentNullException(nameof(roleId), "The required parameter [Role ID] is missing."); }

            try
            {
                PermissionsIsGranted = await _userPermissionRepository.AddUserPermissionAsync(userId, roleId, actionBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return PermissionsIsGranted;
        }

        public async Task<bool> RevokePermissionAsync(string userId, string roleId, string actionBy)
        {
            bool PermissionsIsRevoked = false;

            if (string.IsNullOrEmpty(userId)) { throw new ArgumentNullException(nameof(userId), "The required parameter [User ID] is missing."); }
            if (string.IsNullOrEmpty(roleId)) { throw new ArgumentNullException(nameof(roleId), "The required parameter [Role ID] is missing."); }

            try
            {
                PermissionsIsRevoked = await _userPermissionRepository.DeleteUserPermissionAsync(userId, roleId, actionBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return PermissionsIsRevoked;
        }

        #endregion

        //============= User Login History Action Methods ==============//
        #region UserLoginHistory Action Methods
        public async Task<bool> UpdateUserLoginHistoryAsync(UserLoginHistory userLoginHistory)
        {
            return await _userLoginRepository.AddAsync(userLoginHistory);
        }

        public async Task<IList<UserLoginHistory>> GetUserLoginHistoryByUserNameAndDateAsync(string UserName, int? LoginYear = null, int? LoginMonth = null, int? LoginDay = null)
        {
            List<UserLoginHistory> histories = new List<UserLoginHistory>();

            if (LoginYear != null && LoginYear.Value > 0)
            {
                if (LoginMonth != null && LoginMonth.Value > 0)
                {
                    if (LoginDay != null && LoginDay.Value > 0)
                    {
                        var entities = await _userLoginRepository.GetByUserNameAndYearAndMonthAndDayAsync(UserName, LoginYear.Value, LoginMonth.Value, LoginDay.Value);
                        if (entities != null) { histories = entities.ToList(); }
                    }
                    else
                    {
                        var entities = await _userLoginRepository.GetByUserNameAndYearAndMonthAsync(UserName, LoginYear.Value, LoginMonth.Value);
                        if (entities != null) { histories = entities.ToList(); }
                    }
                }
                else
                {
                    var entities = await _userLoginRepository.GetByUserNameAndYearAsync(UserName, LoginYear.Value);
                    if (entities != null) { histories = entities.ToList(); }
                }
            }
            else
            {
                LoginYear = DateTime.Now.Year;
                if (LoginMonth != null && LoginMonth.Value > 0)
                {
                    if (LoginDay != null && LoginDay.Value > 0)
                    {
                        var entities = await _userLoginRepository.GetByUserNameAndYearAndMonthAndDayAsync(UserName, LoginYear.Value, LoginMonth.Value, LoginDay.Value);
                        if (entities != null) { histories = entities.ToList(); }
                    }
                    else
                    {
                        var entities = await _userLoginRepository.GetByUserNameAndYearAndMonthAsync(UserName, LoginYear.Value, LoginMonth.Value);
                        if (entities != null) { histories = entities.ToList(); }
                    }
                }
                else
                {
                    var entities = await _userLoginRepository.GetByUserNameAndYearAsync(UserName, LoginYear.Value);
                    if (entities != null) { histories = entities.ToList(); }
                }
            }
            return histories;
        }

        public async Task<IList<UserLoginHistory>> GetUserLoginHistoryByDateOnlyAsync(int? LoginYear = null, int? LoginMonth = null, int? LoginDay = null)
        {
            List<UserLoginHistory> histories = new List<UserLoginHistory>();

            if (LoginYear != null && LoginYear.Value > 0)
            {
                if (LoginMonth != null && LoginMonth.Value > 0)
                {
                    if (LoginDay != null && LoginDay.Value > 0)
                    {
                        var entities = await _userLoginRepository.GetByYearAndMonthAndDayAsync(LoginYear.Value, LoginMonth.Value, LoginDay.Value);
                        if (entities != null) { histories = entities.ToList(); }
                    }
                    else
                    {
                        var entities = await _userLoginRepository.GetByYearAndMonthAsync(LoginYear.Value, LoginMonth.Value);
                        if (entities != null) { histories = entities.ToList(); }
                    }
                }
                else
                {
                    LoginMonth = DateTime.Now.Month;
                    var entities = await _userLoginRepository.GetByYearAndMonthAsync(LoginYear.Value, LoginMonth.Value);
                    if (entities != null) { histories = entities.ToList(); }
                }
            }
            else
            {
                LoginYear = DateTime.Now.Year;
                LoginMonth = DateTime.Now.Month;
                LoginDay = DateTime.Now.Day;
                var entities = await _userLoginRepository.GetByYearAndMonthAndDayAsync(LoginYear.Value, LoginMonth.Value, LoginDay.Value);
                if (entities != null) { histories = entities.ToList(); }
            }
            return histories;
        }

        #endregion

        //============= Security Cryptography Action Methods ===========//
        #region Security Cryptography Action Methods
        public string CreatePasswordHash(string plainTextPassword)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: plainTextPassword,
                                salt: Encoding.UTF8.GetBytes(PasswordSalt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        public bool ValidatePassword(string plainTextPassword, string hashedPassword)
          => CreatePasswordHash(plainTextPassword) == hashedPassword;

        private const string PasswordSalt = "CGYzqrN4plZekNC35Uxp1Q==";
        #endregion

        //============= Asset Permissions Action Methods ===============//
        #region Asset Permissions Action Methods
        public async Task<IList<AssetPermission>> GetAssetPermissionsByUserIdAsync(string userId)
        {
            List<AssetPermission> assetPermissions = new List<AssetPermission>();
            var entities = await _entityPermissionRepository.GetAssetPermissionsByUserIdAsync(userId);
            assetPermissions = entities.ToList();
            return assetPermissions;
        }

        public async Task<bool> GrantAssetPermissionAsync(AssetPermission assetPermission)
        {
                return await _entityPermissionRepository.AddAssetPermissionAsync(assetPermission);
        }

        public async Task<bool> RevokeAssetPermissionAsync(int assetPermissionId)
        {
            return   await _entityPermissionRepository.DeleteAssetPermissionAsync(assetPermissionId);
        }


        #endregion
    }

    public class SecurityConstants
    {
        public const string ChxCookieAuthentication = "OmAuthsVqr8b0zt";
    }
}
