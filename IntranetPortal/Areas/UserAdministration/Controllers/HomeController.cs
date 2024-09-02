using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.UserAdministration.Models;
using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IntranetPortal.Areas.UserAdministration.Controllers
{
    [Area("UserAdministration")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IDataProtector _dataProtector;
        private readonly IBaseModelService _baseModelService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IAssetManagerService _assetManagerService;
        public HomeController(IConfiguration configuration, ISecurityService securityService, IDataProtectionProvider dataProtectionProvider,
                               DataProtectionEncryptionStrings dataProtectionEncryptionStrings, IBaseModelService baseModelService,
                               IGlobalSettingsService globalSettingsService, IAssetManagerService assetManagerService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _baseModelService = baseModelService;
            _globalSettingsService = globalSettingsService;
            _assetManagerService = assetManagerService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "XYALLACCZ")]
        public IActionResult Index()
        {
            return View();
        }

        #region User Account Controller Action Methods
        [HttpGet]
        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> ResetUserPassword(string id)
        {
            PasswordResetViewModel model = new PasswordResetViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var users = await _securityService.GetUsersByUserId(id);
                if (users == null || users.Count < 1)
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for the selected user.";
                    model.OperationIsCompleted = true;
                    return View();
                }

                ApplicationUser user = users.FirstOrDefault();
                model.ConfirmPassword = string.Empty;
                model.FullName = user.FullName;
                model.Password = string.Empty;
                model.UserID = user.Id;
                model.UserType = user.UserType;
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, no record was found for the selected staff.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> ResetUserPassword(PasswordResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(model.UserID) || string.IsNullOrEmpty(model.Password))
                    {
                        model.OperationIsCompleted = false;
                        model.OperationIsSuccessful = false;
                        model.ViewModelErrorMessage = $"Operation could not be completed. some key form values were not found.";
                        return View(model);
                    }
                    else
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            Id = model.UserID,
                            ModifiedBy = HttpContext.User.Identity.Name,
                            ModifiedTime = $"{DateTime.Now.Date.ToLongDateString()} {DateTime.Now.ToLongTimeString()}",
                        };

                        if (string.IsNullOrWhiteSpace(model.Password))
                        {
                            model.OperationIsCompleted = false;
                            model.OperationIsSuccessful = false;
                            model.ViewModelErrorMessage = $"Sorry, the operation could not be completed. Password has an invalid value. Please try again.";
                            return View(model);
                        }
                        user.PasswordHash = _securityService.CreatePasswordHash(model.Password);
                        bool result = await _securityService.ResetUserPasswordAsync(user);
                        if (result)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Password reset was completed successfully!";
                            return View(model);
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, there was a problem. The password reset failed.";
                            model.OperationIsCompleted = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = false;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> EditUserAccount(string id)
        {
            EditUserAccountViewModel model = new EditUserAccountViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var users = await _securityService.GetUsersByUserId(id);
                if (users == null || users.Count < 1)
                {
                    model.ViewModelErrorMessage = "Sorry, no record was found for the selected user.";
                    model.OperationIsCompleted = true;
                    return View();
                }

                ApplicationUser user = users.FirstOrDefault();
                model.EnableLockOut = user.LockoutEnabled;
                model.FullName = user.FullName;
                model.LoginID = user.UserName;
                model.OldLoginID = user.UserName;
                model.UserID = user.Id;
                model.UserType = user.UserType;
                model.LockOutEndDate = user.LockoutEnd;
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, no record was found for the selected staff.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> EditUserAccount(EditUserAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        Id = model.UserID,
                        ModifiedBy = HttpContext.User.Identity.Name,
                        UserName = model.LoginID,
                        FullName = model.FullName,
                        LockoutEnabled = model.EnableLockOut,
                        LockoutEnd = model.LockOutEndDate,
                        ModifiedTime = $"{DateTime.Now.Date.ToLongDateString()} {DateTime.Now.ToLongTimeString()}",
                    };

                    bool LoginIsAvailable = await _securityService.LoginIdIsAvailable(model.UserID, model.LoginID);
                    if (LoginIsAvailable)
                    {
                        bool IsSuccessful = await _securityService.UpdateUserAsync(user);
                        if (IsSuccessful)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Update completed successfully!";
                            return View(model);
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, there was a problem. The update operation failed.";
                            model.OperationIsCompleted = false;
                            return View(model);
                        }
                    }
                    else
                    {
                        model.OperationIsCompleted = false;
                        model.OperationIsSuccessful = false;
                        model.ViewModelErrorMessage = $"Sorry this Login ID already exists in the system. Please enter a different value.";
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = false;
            }
            return View(model);
        }
        #endregion

        #region User Permissions
        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> UserPermissions(string id, string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            string userId = id;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["UserID"] = id;
            ViewData["CurrentFilter"] = searchString;
            List<UserPermission> permissions = new List<UserPermission>();

            if (!String.IsNullOrEmpty(searchString))
            {
                var entities = await _securityService.GetAllUserPermissionsByUserIdAndApplicationIdAsync(userId, searchString);
                permissions = entities.ToList();
            }

            var applications = await _baseModelService.GetSystemApplicationsAsync();
            ViewBag.ApplicationList = new SelectList(applications, "ApplicationID", "ApplicationName", searchString);

            int pageSize = 50;
            return View(PaginatedList<UserPermission>.CreateAsync(permissions.AsQueryable(), pageNumber ?? 1, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "XYALLACCZ")]
        public string GrantUserPermission(string usd, string rld)
        {
            if(string.IsNullOrWhiteSpace(usd) && string.IsNullOrWhiteSpace(rld)){return "parameter";}
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if(_securityService.GrantPermissionAsync(usd, rld, actionBy).Result)
                {
                    return "granted";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        [HttpPost]
        [Authorize(Roles = "XYALLACCZ")]
        public string RevokeUserPermission(string usd, string rld)
        {
            if (string.IsNullOrWhiteSpace(usd) || string.IsNullOrWhiteSpace(rld)) { return "parameter"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_securityService.RevokePermissionAsync(usd, rld, actionBy).Result)
                {
                    return "revoked";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> PermissionsList(string id, string ad = null)
        {
            string userId = id;
            ApplicationRolesListViewModel model = new ApplicationRolesListViewModel();

            ViewData["UserID"] = id;
            ViewData["ApplicationID"] = ad;

            if (!string.IsNullOrWhiteSpace(id))
            {
                if( !string.IsNullOrWhiteSpace(ad))
                {
                var entities = await _securityService.GetUserRolesUnGrantedByUserIDAsync(userId, ad);
                model.ApplicationRoleList = entities.ToList();
                }
                else
                {
                    var entities = await _securityService.GetUserRolesUnGrantedByUserIDAsync(userId);
                    model.ApplicationRoleList = entities.ToList();
                }
            }

            var applications = await _baseModelService.GetSystemApplicationsAsync();
            ViewBag.ApplicationList = new SelectList(applications, "ApplicationID", "ApplicationName", ad);

            return View(model);
        }

        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> LoginHistory(string nm, int? yy = null, int? mm = null, int? dd = null)
        {
            LoginHistoryListViewModel model = new LoginHistoryListViewModel();
            if (!string.IsNullOrWhiteSpace(nm))
            {
                model.UserLoginHistories = await _securityService.GetUserLoginHistoryByUserNameAndDateAsync(nm, yy, mm, dd);
            }
            else
            {
                model.UserLoginHistories = await _securityService.GetUserLoginHistoryByDateOnlyAsync(yy, mm, dd);
            }
            model.nm = nm;
            model.yy = yy ?? DateTime.Today.Year;
            model.mm = mm ?? DateTime.Today.Month;
            model.dd = dd ?? DateTime.Today.Day;
            return View(model);
        }

        #endregion

        #region Asset Permission Controller Actions

        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> GrantUserAssetPermission(string id)
        {
            AssetPermissionViewModel model = new AssetPermissionViewModel();
            model.UserID = id;
            var assetDivisionEntities = await _assetManagerService.GetAssetDivisionsAsync();
            if(assetDivisionEntities != null && assetDivisionEntities.Count > 0)
            {
                ViewBag.AssetDivisionList = new SelectList(assetDivisionEntities, "ID", "Name");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> GrantUserAssetPermission(AssetPermissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AssetPermission assetPermission = model.ConvertToAssetPermission();
                    AssetDivision assetDivision = await _assetManagerService.GetAssetDivisionByIdAsync(assetPermission.AssetDivisionID);
                    bool PermissionIsGranted = await _securityService.GrantAssetPermissionAsync(assetPermission);

                    if (PermissionIsGranted)
                    {
                       return RedirectToAction("AssetsPermissions","Staff", new { id=assetPermission.UserID });
                    }
                    else
                    {
                        model.OperationIsCompleted = false;
                        model.OperationIsSuccessful = false;
                        model.ViewModelErrorMessage = $"Sorry, an error was encountered. New Asset Permission could not be granted. Please try again.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                    model.OperationIsCompleted = true;
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = false;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "XYALLACCZ")]
        public string RevokeUserAssetPermission(int assetPermissionId)
        {
            if (assetPermissionId < 1) { return "parameter"; }
            try
            {
                if (_securityService.RevokeAssetPermissionAsync(assetPermissionId).Result)
                {
                    return "revoked";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        #endregion

        #region Locations Permission Controller Actions
        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> LocationPermissions(string id)
        {
            LocationPermissionsListViewModel model = new LocationPermissionsListViewModel();
            model.id = id;

            if (!string.IsNullOrEmpty(id))
            {
                var entities = await _securityService.GetLocationPermissionsByUserIdAsync(id);
                model.LocationPermissionList = entities.ToList();
            }

            var locationEntities = await _globalSettingsService.GetAllLocationsAsync();
            if (locationEntities != null && locationEntities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(locationEntities, "LocationID", "LocationName");
            }
            return View(model);
        }

        [Authorize(Roles = "XYALLACCZ")]
        public async Task<IActionResult> GrantUserLocationPermission(string id)
        {
            GrantUserLocationPermissionViewModel model = new GrantUserLocationPermissionViewModel();
            model.id = id;
            var locationEntities = await _securityService.GetLocationsYetToBeGrantedByUserIdAsync(id);
            if (locationEntities != null && locationEntities.Count > 0)
            {
                model.LocationsList = locationEntities.ToList();
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "XYALLACCZ")]
        public string RevokeUserLocationPermission(int locationPermissionId)
        {
            if (locationPermissionId < 1) { return "parameter"; }
            try
            {
                bool IsRevoked = _securityService.RevokeLocationPermissionAsync(locationPermissionId).Result;
                if (IsRevoked)
                {
                    return "revoked";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        [HttpPost]
        [Authorize(Roles = "XYALLACCZ")]
        public string GrantLocationPermission(string userId, int locationId)
        {
            if (locationId < 1 || string.IsNullOrWhiteSpace(userId)) { return "parameter"; }
            LocationPermission locationPermission = new LocationPermission();
            locationPermission.UserId = userId;
            locationPermission.LocationId = locationId;
            locationPermission.PermissionTypeId = (int)EntityPermissionType.AssetLocation;
            
            try
            {
                bool IsGranted = false;
                IsGranted = _securityService.GrantLocationPermissionAsync(locationPermission).Result;
                if (IsGranted)
                {
                    return "granted";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }


        #endregion




        //========== Employees Helper Methods =========//
        #region Employees Helper Methods

        [HttpGet]
        public JsonResult GetNamesOfEmployeeUsers(string text)
        {
            List<string> employees = _securityService.SearchEmployeeUsersByNameAsync(text).Result.Select(x => x.FullName).ToList();
            return Json(employees);
        }

        [HttpGet]
        public JsonResult GetEmployeeUserParameters(string nm)
        {
            EmployeeUser employee = new EmployeeUser();

            employee = _securityService.GetEmployeeUsersByNameAsync(nm).Result.FirstOrDefault();

            if (employee == null)
            {
                employee = new EmployeeUser
                {
                    UserID = string.Empty,
                    //AssetTypeID = -1
                };
            }

            string model = JsonConvert.SerializeObject(new
            {
                usr_id = employee.UserID,
                usr_login = employee.UserName,
                usr_name = employee.FullName,
                unit_id = employee.UnitID,
                dept_id = employee.DepartmentID,
                station_id = employee.LocationID,
            }, Formatting.Indented);

            return Json(model);
        }
        #endregion
    }
}