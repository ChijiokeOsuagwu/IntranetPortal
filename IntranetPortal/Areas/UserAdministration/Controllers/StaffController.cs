using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntranetPortal.Areas.UserAdministration.Models;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.UserAdministration.Controllers
{
    [Area("UserAdministration")]
    [Authorize]
    public class StaffController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _employeeRecordService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        public StaffController(IConfiguration configuration, IGlobalSettingsService globalSettingsService, IErmService employeeRecordService,
                                    ISecurityService securityService, IDataProtectionProvider dataProtectionProvider, IBaseModelService baseModelService,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _configuration = configuration;
            _securityService = securityService;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
            _baseModelService = baseModelService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "UADSTFVWL, XYALLACCZ")]
        public async Task<IActionResult> UserList(string currentFilter, string searchString, int? pageNumber)
        {

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            List<EmployeeUser> users = new List<EmployeeUser>();

            if (!String.IsNullOrEmpty(searchString))
            {
                var entities = await _securityService.SearchEmployeeUsersByNameAsync(searchString).ConfigureAwait(false);
                users = entities.ToList();
            }
            int pageSize = 10;
            return View(PaginatedList<EmployeeUser>.CreateAsync(users.AsQueryable(), pageNumber ?? 1, pageSize));
        }

        [Authorize(Roles = "UADSTFADN, XYALLACCZ")]
        public async Task<IActionResult> SelectStaff(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["UnitSortParm"] = sortOrder == "unit" ? "unit_desc" : "unit";
            ViewData["DeptSortParm"] = sortOrder == "dept" ? "dept_desc" : "dept";
            ViewData["LocSortParm"] = sortOrder == "loc" ? "loc_desc" : "loc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            List<Employee> employees = new List<Employee>();

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = await _employeeRecordService.GetNonUserEmployeesByNameAsync(searchString);
            }
            else
            {
                employees = await _employeeRecordService.GetAllNonUserEmployeesAsync();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(s => s.FullName).ToList();
                    break;
                case "unit":
                    employees = employees.OrderBy(s => s.UnitName).ToList();
                    break;
                case "unit_desc":
                    employees = employees.OrderByDescending(s => s.UnitName).ToList();
                    break;
                case "dept":
                    employees = employees.OrderBy(s => s.DepartmentName).ToList();
                    break;
                case "dept_desc":
                    employees = employees.OrderByDescending(s => s.DepartmentName).ToList();
                    break;
                case "loc":
                    employees = employees.OrderBy(s => s.LocationName).ToList();
                    break;
                case "loc_desc":
                    employees = employees.OrderByDescending(s => s.LocationName).ToList();
                    break;
                default:
                    employees = employees.OrderBy(s => s.FullName).ToList();
                    break;
            }

            int pageSize = 10;
            return View(PaginatedList<Employee>.CreateAsync(employees.AsQueryable(), pageNumber ?? 1, pageSize));
        }

        [Authorize(Roles = "UADUSRVWD, XYALLACCZ")]
        public async Task<IActionResult> ShowDetails(string id)
        {
            Employee employee = new Employee();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    employee = await _employeeRecordService.GetEmployeeByIdAsync(id);
                    if (employee != null)
                    {
                        if (employee.ConfirmationDate != null)
                        {
                            employee.ConfirmationDateFormatted = employee.ConfirmationDate.Value.ToLongDateString();
                        }

                        if (employee.DateOfLastPromotion != null)
                        {
                            employee.DateOfLastPromotionFormatted = employee.DateOfLastPromotion.Value.ToLongDateString();
                        }

                        if (employee.StartUpDate != null)
                        {
                            employee.StartUpDateFormatted = employee.StartUpDate.Value.ToLongDateString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(employee);
        }

        [HttpGet]
        [Authorize(Roles = "UADSTFADN, XYALLACCZ")]
        public async Task<IActionResult> CreateUserAccount(string id)
        {
            EmployeeUserViewModel model = new EmployeeUserViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var users = await _securityService.GetUsersByUserId(id);
                if (users.Count > 0)
                {
                    ApplicationUser user = users.FirstOrDefault();
                    return RedirectToAction("EmployeeUserDetails", new { id = user.Id });
                }

                Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(id);
                model.UserID = employee.EmployeeID;
                model.FullName = employee.FullName;
                model.EmployeeNumber = employee.EmployeeNo1;
                model.CompanyCode = employee.CompanyID;
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, no record was found for the selected staff.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "UADSTFADN, XYALLACCZ")]
        public async Task<IActionResult> CreateUserAccount(EmployeeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(model.UserID) || string.IsNullOrEmpty(model.LoginID))
                    {
                        model.OperationIsCompleted = false;
                        model.OperationIsSuccessful = false;
                        model.ViewModelErrorMessage = $"Operation could not be completed. some key form values were not found.";
                        return View(model);
                    }
                    else
                    {
                        var applicationUserIdUsers = await _securityService.GetUsersByUserId(model.UserID);
                        if (applicationUserIdUsers.Count > 0)
                        {
                            model.OperationIsCompleted = false;
                            model.OperationIsSuccessful = false;
                            model.ViewModelErrorMessage = $"Sorry this staff already has an existing user account in the system.";
                            return View(model);
                        }


                        var applicationLoginUsers = await _securityService.GetUsersByLoginId(model.LoginID);
                        if (applicationLoginUsers.Count > 0)
                        {
                            model.OperationIsCompleted = false;
                            model.OperationIsSuccessful = false;
                            model.ViewModelErrorMessage = $"Sorry this Login ID already exists in the system. Please enter a different value.";
                            return View(model);
                        }

                        ApplicationUser user = new ApplicationUser
                        {
                            AccessFailedCount = 0,
                            CompanyCode = model.CompanyCode ?? string.Empty,
                            ConcurrencyStamp = string.Empty,
                            CreatedBy = "System Administrator",
                            CreatedTime = $"{DateTime.Now.Date.ToLongDateString()} {DateTime.Now.ToLongTimeString()}",
                            EmailConfirmed = false,
                            FullName = model.FullName,
                            Id = model.UserID,
                            LockoutEnabled = model.EnableLockOut,
                            LockoutEnd = null,
                            ModifiedBy = "System Administrator",
                            ModifiedTime = $"{DateTime.Now.Date.ToLongDateString()} {DateTime.Now.ToLongTimeString()}",
                            NormalizedUserName = model.LoginID.ToUpper(),
                            PasswordHash = _securityService.CreatePasswordHash(model.Password),
                            SecurityStamp = string.Empty,
                            TwoFactorEnabled = false,
                            UserName = model.LoginID,
                            UserType = "Employee",
                        };

                        var result = await _securityService.CreateUserAccountAsync(user);
                        if (result)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New User Account was created successfully!";
                            return View(model);
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry new user account could not be created.";
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

        [Authorize(Roles = "UADSTFVWD, XYALLACCZ")]
        public async Task<IActionResult> EmployeeUserDetails(string id)
        {
            EmployeeUserDetailsViewModel model = new EmployeeUserDetailsViewModel();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var entities = await _securityService.GetUsersByUserId(id);
                    if (entities.Count > 0)
                    {
                        ApplicationUser user = entities.FirstOrDefault();
                        model.FullName = user.FullName;
                        model.EnableLockOut = user.LockoutEnabled;
                        model.LoginID = user.UserName;
                        model.UserID = user.Id;
                        model.UserType = user.UserType;
                    }

                    Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(id);
                    if (employee != null)
                    {
                        model.CompanyName = employee.CompanyName;
                        model.DepartmentName = employee.DepartmentName;
                        model.UnitName = employee.UnitName;
                        model.EmployeeNo1 = employee.EmployeeNo1;
                        model.LocationName = employee.LocationName;
                        model.OfficialEmail = employee.OfficialEmail;
                        model.PersonalEmail = employee.Email;
                        model.PhoneNo1 = employee.PhoneNo1;
                        model.PhoneNo2 = employee.PhoneNo2;
                        model.Sex = employee.Sex;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "UADSTFEDT, XYALLACCZ")]
        public async Task<IActionResult> EditEmployeeUserAccount(string id)
        {
            EmployeeUserViewModel model = new EmployeeUserViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var users = await _securityService.GetUsersByUserId(id);
                if (users == null || users.Count < 1)
                {
                    return RedirectToAction("CreateUserAccount", new {id});
                }
                
                ApplicationUser user = users.FirstOrDefault();
                model.LoginID = user.UserName;
                model.EnableLockOut = user.LockoutEnabled;

                Employee employee = await _employeeRecordService.GetEmployeeByIdAsync(id);
                model.UserID = employee.EmployeeID;
                model.FullName = employee.FullName;
                model.EmployeeNumber = employee.EmployeeNo1;
                model.CompanyCode = employee.CompanyID;
               
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, no record was found for the selected staff.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "UADSTFEDT, XYALLACCZ")]
        public async Task<IActionResult> EditEmployeeUserAccount(EmployeeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(model.UserID) || string.IsNullOrEmpty(model.LoginID))
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
                            LockoutEnabled = model.EnableLockOut,
                            PasswordHash = _securityService.CreatePasswordHash(model.Password),
                            ModifiedBy = "System Administrator",
                            ModifiedTime = $"{DateTime.Now.Date.ToLongDateString()} {DateTime.Now.ToLongTimeString()}",
                        };

                        // var result = await _userManager.CreateAsync(user, model.Password);
                        var result = await _securityService.CreateUserAccountAsync(user);

                        if (result)
                        {
                            //await _signManager.SignInAsync(user, false);
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "User Account was updated successfully!";
                            return View(model);
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry user account could not be updated.";
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

        [Authorize(Roles = "UADSTFDLT, XYALLACCZ")]
        public async Task<IActionResult> DeleteUserAccount(string id)
        {
            UserDetailViewModel model = new UserDetailViewModel();
            ApplicationUser user = new ApplicationUser();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var users = await _securityService.GetUsersByUserId(id);
                    user = users.FirstOrDefault();
                    if (user != null)
                    {
                        model.UserID = user.Id == null ? string.Empty : user.Id;
                        model.FullName = user.FullName == null ? string.Empty : user.FullName;
                        model.Username = user.UserName == null ? string.Empty : user.UserName;
                        model.UserType = user.UserType == null ? string.Empty : user.UserType;
                        model.IsLocked = user.LockoutEnabled;
                        model.Company = user.CompanyCode;
                        model.LastModifiedBy = user.ModifiedBy == null ? string.Empty : user.ModifiedBy;
                        model.LastModifiedDate = user.ModifiedTime == null ? string.Empty : user.ModifiedTime;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message ?? string.Empty;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "UADSTFDLT, XYALLACCZ")]
        public async Task<IActionResult> DeleteUserAccount(UserDetailViewModel model)
        {
            try
            {
                bool IsDeleted = false;
                string DeletedBy = "System Administrator";
                if (!string.IsNullOrEmpty(model.UserID))
                {
                    IsDeleted = await _securityService.DeleteUserAsync(model.UserID, DeletedBy);
                    if (IsDeleted)
                    {
                        return RedirectToAction("UserList");
                    }
                    model.ViewModelErrorMessage = $"Sorry, there was an error. User was not deleted. Please try again.";
                        return View(model);
                }

                model.ViewModelErrorMessage = $"Ooops! No record was found for this User.";
                return View(model);
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message ?? string.Empty;
                return View(model);
            }
        }
    }
}