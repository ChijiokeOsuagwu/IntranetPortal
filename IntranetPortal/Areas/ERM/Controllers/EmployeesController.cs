using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IntranetPortal.Areas.ERM.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.ERM.Controllers
{
    [Area("ERM")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILmsService _lmsService;
        public EmployeesController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings, IErmService ermService,
                                    IBaseModelService baseModelService, IWebHostEnvironment webHostEnvironment,
                                    ILmsService lmsService)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _ermService = ermService;
            _baseModelService = baseModelService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
            _webHostEnvironment = webHostEnvironment;
            _lmsService = lmsService;
        }

        [Authorize(Roles = "ERMVWAEMR, XYALLACCZ")]
        public async Task<IActionResult> Search(string sn)
        {
            List<Employee> employees = new List<Employee>();
            if (!string.IsNullOrWhiteSpace(sn))
            {
                employees = await _ermService.SearchEmployeesByNameAsync(sn);
                if (employees != null)
                {
                    if (employees.Count == 1)
                    {
                        Employee employee = employees.First();
                        return RedirectToAction("Profile", new { id = employee.EmployeeID });
                    }
                    ViewBag.EmployeeList = employees;
                }
            }
            return View();
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        public async Task<IActionResult> SelectPerson(string pn)
        {
            Person person = new Person();
            if (!string.IsNullOrWhiteSpace(pn))
            {
                person = await _baseModelService.GetPersonbyNameAsync(pn);
                if (person != null && !string.IsNullOrWhiteSpace(person.PersonID))
                {
                    return RedirectToAction("Create", new { id = person.PersonID });
                }
                else
                {
                    List<Person> persons = await _baseModelService.SearchPersonsByName(pn);
                    if (persons != null)
                    {
                        ViewBag.PersonList = persons;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"No record was found for the selected person.";
                    }
                }
            }
            return View();
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        public async Task<IActionResult> Create(string id = null)
        {
            CreateEmployeeViewModel model = new CreateEmployeeViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                Person person = new Person();
                person = await _baseModelService.GetPersonAsync(id);
                model.FirstName = person.FirstName;
                model.Surname = person.Surname;
                model.PersonID = person.PersonID;
                model.EmployeeID = person.PersonID;
                model.OtherNames = person.OtherNames;
                model.BirthDay = person.BirthDay;
                model.BirthMonth = person.BirthMonth;
                model.BirthYear = person.BirthYear;
                model.MaritalStatus = person.MaritalStatus;
                model.PhoneNo1 = person.PhoneNo1;
                model.PhoneNo2 = person.PhoneNo2;
                model.Sex = person.Sex;
            }
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var companies = await _globalSettingsService.GetCompaniesAsync();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.CompanyList = new SelectList(companies, "CompanyCode", "CompanyName");
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");
            return View(model);
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ViewModelErrorMessage = $"Ooops! Some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            else
            {
                bool EmployeeIsCreated = false;
                try
                {
                    if (string.IsNullOrWhiteSpace(model.PersonID))
                    {
                        string ID = Guid.NewGuid().ToString();
                        model.PersonID = ID;
                        model.EmployeeID = ID;

                        Employee employee = model.ConvertToEmployee();
                        employee.ModifiedBy = employee.EmployeeModifiedBy = HttpContext.User.Identity.Name;
                        employee.ModifiedTime = employee.EmployeeModifiedBy = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";
                        employee.CreatedBy = employee.EmployeeCreatedBy = HttpContext.User.Identity.Name;
                        employee.CreatedTime = employee.EmployeeCreatedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";

                        EmployeeIsCreated = await _ermService.CreateEmployeeAsync(employee, false);
                    }
                    else
                    {
                        Employee employee = model.ConvertToEmployee();
                        employee.ModifiedBy = employee.EmployeeModifiedBy = HttpContext.User.Identity.Name;
                        employee.ModifiedTime = employee.EmployeeModifiedBy = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";
                        employee.CreatedBy = employee.EmployeeCreatedBy = HttpContext.User.Identity.Name;
                        employee.CreatedTime = employee.EmployeeCreatedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";

                        EmployeeIsCreated = await _ermService.CreateEmployeeAsync(employee, true);
                    }

                    if (EmployeeIsCreated)
                    {
                        return RedirectToAction("Profile", new { id = model.EmployeeID });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An unknown error was encountered. Creating new employee failed. ";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var companies = await _globalSettingsService.GetCompaniesAsync();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.CompanyList = new SelectList(companies, "CompanyCode", "CompanyName");
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");
            return View(model);
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        public async Task<IActionResult> Edit(string id)
        {
            CreateEmployeeViewModel model = new CreateEmployeeViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    Employee employee = await _ermService.GetEmployeeByIdAsync(id);
                    if (employee != null)
                    {
                        model.Address = employee.Address;
                        model.BirthDay = employee.BirthDay;
                        model.BirthMonth = employee.BirthMonth;
                        model.BirthYear = employee.BirthYear;
                        model.CompanyCode = employee.CompanyID;
                        model.CompanyName = employee.CompanyName;
                        model.ConfirmationDate = employee.ConfirmationDate;
                        model.CurrentDesignation = employee.CurrentDesignation;
                        model.DateOfBirth = employee.DateOfBirth;
                        model.DateOfLastPromotion = employee.DateOfLastPromotion;
                        model.DepartmentID = employee.DepartmentID;
                        model.DepartmentName = employee.DepartmentName;
                        model.Email = employee.Email;
                        model.EmployeeID = employee.EmployeeID;
                        model.EmployeeName = employee.FullName;
                        model.EmployeeNo1 = employee.EmployeeNo1;
                        model.EmployeeNo2 = employee.EmployeeNo2;
                        model.EmploymentStatus = employee.EmploymentStatus;
                        model.FirstName = employee.FirstName;
                        model.FullName = employee.FullName;
                        model.ImagePath = employee.ImagePath;
                        model.JobGrade = employee.JobGrade;
                        model.LgaOfOrigin = employee.LgaOfOrigin;
                        model.LocationID = employee.LocationID;
                        model.LocationName = employee.LocationName;
                        model.MaritalStatus = employee.MaritalStatus;
                        model.OfficialEmail = employee.OfficialEmail;
                        model.OtherNames = employee.OtherNames;
                        model.PersonID = employee.PersonID;
                        model.PhoneNo1 = employee.PhoneNo1;
                        model.PhoneNo2 = employee.PhoneNo2;
                        model.PlaceOfEngagement = employee.PlaceOfEngagement;
                        model.Religion = employee.Religion;
                        model.Sex = employee.Sex;
                        model.StartUpDate = employee.StartUpDate?? DateTime.Today;
                        model.StartUpDesignation = employee.StartUpDesignation;
                        model.StateOfOrigin = employee.StateOfOrigin;
                        model.Surname = employee.Surname;
                        model.Title = employee.Title;
                        model.UnitID = employee.UnitID;
                        model.UnitName = employee.UnitName;
                        model.YearsOfExperience = employee.YearsOfExperience;
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, the requested record was not found. It may have been deleted.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, the requested record was not found. It may have been deleted.";
            }
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var companies = await _globalSettingsService.GetCompaniesAsync();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.CompanyList = new SelectList(companies, "CompanyCode", "CompanyName");
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");
            return View(model);
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> Edit(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ViewModelErrorMessage = $"Ooops! Some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            else
            {
                try
                {
                    Employee employee = model.ConvertToEmployee();
                    bool EmployeeIsUpdated = await _ermService.UpdateEmployeeAsync(employee);
                    if (EmployeeIsUpdated)
                    {
                        model.ViewModelSuccessMessage = "Employee update completed successfully!";
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An unknown error was encountered. Updating employee record failed. ";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var companies = await _globalSettingsService.GetCompaniesAsync();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.CompanyList = new SelectList(companies, "CompanyCode", "CompanyName");
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");
            return View(model);
        }

        public async Task<IActionResult> Profile(string id, string src)
        {
            EmployeeProfileViewModel model = new EmployeeProfileViewModel();
            model.Source = src;
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                }

                Employee employee = new Employee();
                employee = await _ermService.GetEmployeeByIdAsync(id);
                if (employee != null)
                {
                    model.Address = employee.Address.ToUpper();
                    model.CompanyName = employee.CompanyName.ToUpper();
                    model.CurrentDesignation = employee.CurrentDesignation.ToUpper();
                    model.DepartmentName = employee.DepartmentName.ToUpper();
                    model.Email = employee.Email;
                    model.EmployeeID = employee.EmployeeID;
                    model.EmployeeNo1 = employee.EmployeeNo1.ToUpper();
                    model.EmployeeNo2 = employee.EmployeeNo2.ToUpper();
                    model.EmploymentStatus = employee.EmploymentStatus.ToUpper();
                    model.FullName = employee.FullName.ToUpper();
                    model.GeoPoliticalRegion = employee.GeoPoliticalRegion.ToUpper();
                    model.ImagePath = employee.ImagePath;
                    model.JobGrade = employee.JobGrade.ToUpper();
                    model.LgaOfOrigin = employee.LgaOfOrigin.ToUpper();
                    model.LocationName = employee.LocationName.ToUpper();
                    model.MaritalStatus = employee.MaritalStatus.ToUpper();
                    model.OfficialEmail = employee.OfficialEmail;
                    model.PersonID = employee.PersonID;
                    model.PhoneNo1 = employee.PhoneNo1;
                    model.PhoneNo2 = employee.PhoneNo2;
                    model.PlaceOfEngagement = employee.PlaceOfEngagement.ToUpper();
                    model.Religion = employee.Religion.ToUpper();
                    model.Sex = employee.Sex.ToUpper();
                    model.StateOfOrigin = employee.StateOfOrigin.ToUpper();
                    model.UnitName = employee.UnitName.ToUpper();

                    if (employee.ConfirmationDate != null && employee.ConfirmationDate.HasValue)
                    { model.ConfirmationDateFormatted = $"{employee.ConfirmationDate.Value.ToLongDateString()}".ToUpper(); }
                    else { model.ConfirmationDateFormatted = string.Empty; }

                    if (employee.DateOfLastPromotion != null && employee.DateOfLastPromotion.HasValue)
                    { model.DateOfLastPromotionFormatted = $"{employee.DateOfLastPromotion.Value.ToLongDateString()}".ToUpper(); }
                    else { model.DateOfLastPromotionFormatted = string.Empty; }

                    if (employee.StartUpDate != null && employee.StartUpDate.HasValue)
                    { model.StartUpDateFormatted = $"{employee.StartUpDate.Value.ToLongDateString()}".ToUpper(); }
                    else { model.StartUpDateFormatted = string.Empty; }

                    if (employee.BirthDay != null && employee.BirthDay > 0 && employee.BirthMonth != null && employee.BirthMonth > 0)
                    {
                        DateTime dateOfBirth = new DateTime(2020, employee.BirthMonth.Value, employee.BirthDay.Value);
                        model.DateOfBirth = $"{dateOfBirth.ToString("MMMM")} {employee.BirthDay.Value.ToString()}".ToUpper();
                    }

                    if (employee.LengthOfService != null && employee.LengthOfService.Value > 0)
                    {
                        if (employee.LengthOfService.Value < 365) { model.LengthOfServiceFormatted = $"{employee.LengthOfService} days".ToUpper(); }
                        else
                        {
                            model.LengthOfServiceFormatted = $"~ {employee.LengthOfService.Value / 365} years {employee.LengthOfService % 365} days".ToUpper();
                        }
                    }
                    else { model.LengthOfServiceFormatted = "0 days".ToUpper(); }
                }
                else
                {
                    model.ViewModelErrorMessage = "No profile record was found for the selected staff.";
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Sorry an error was encountered while attempting to retrieve the record.");
                sb.Append(ex.Message);
                model.ViewModelErrorMessage = sb.ToString();
            }
            return View(model);
        }
       public async Task<IActionResult> Info(string id, string src)
        {
            EmployeeProfileViewModel model = new EmployeeProfileViewModel();
            model.Source = src;
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    var claims = HttpContext.User.Claims.ToList();
                    id = claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                }

                Employee employee = new Employee();
                employee = await _ermService.GetEmployeeByIdAsync(id);
                if (employee != null)
                {
                    model.Address = employee.Address.ToUpper();
                    model.CompanyName = employee.CompanyName.ToUpper();
                    model.CurrentDesignation = employee.CurrentDesignation.ToUpper();
                    model.DepartmentName = employee.DepartmentName.ToUpper();
                    model.Email = employee.Email;
                    model.EmployeeID = employee.EmployeeID;
                    model.EmployeeNo1 = employee.EmployeeNo1.ToUpper();
                    model.EmployeeNo2 = employee.EmployeeNo2.ToUpper();
                    model.EmploymentStatus = employee.EmploymentStatus.ToUpper();
                    model.FullName = employee.FullName.ToUpper();
                    model.GeoPoliticalRegion = employee.GeoPoliticalRegion.ToUpper();
                    model.ImagePath = employee.ImagePath;
                    model.JobGrade = employee.JobGrade.ToUpper();
                    model.LgaOfOrigin = employee.LgaOfOrigin.ToUpper();
                    model.LocationName = employee.LocationName.ToUpper();
                    model.MaritalStatus = employee.MaritalStatus.ToUpper();
                    model.OfficialEmail = employee.OfficialEmail;
                    model.PersonID = employee.PersonID;
                    model.PhoneNo1 = employee.PhoneNo1;
                    model.PhoneNo2 = employee.PhoneNo2;
                    model.PlaceOfEngagement = employee.PlaceOfEngagement.ToUpper();
                    model.Religion = employee.Religion.ToUpper();
                    model.Sex = employee.Sex.ToUpper();
                    model.StateOfOrigin = employee.StateOfOrigin.ToUpper();
                    model.UnitName = employee.UnitName.ToUpper();

                    if (employee.ConfirmationDate != null && employee.ConfirmationDate.HasValue)
                    { model.ConfirmationDateFormatted = $"{employee.ConfirmationDate.Value.ToLongDateString()}".ToUpper(); }
                    else { model.ConfirmationDateFormatted = string.Empty; }

                    if (employee.DateOfLastPromotion != null && employee.DateOfLastPromotion.HasValue)
                    { model.DateOfLastPromotionFormatted = $"{employee.DateOfLastPromotion.Value.ToLongDateString()}".ToUpper(); }
                    else { model.DateOfLastPromotionFormatted = string.Empty; }

                    if (employee.StartUpDate != null && employee.StartUpDate.HasValue)
                    { model.StartUpDateFormatted = $"{employee.StartUpDate.Value.ToLongDateString()}".ToUpper(); }
                    else { model.StartUpDateFormatted = string.Empty; }

                    if (employee.BirthDay != null && employee.BirthDay > 0 && employee.BirthMonth != null && employee.BirthMonth > 0)
                    {
                        DateTime dateOfBirth = new DateTime(2020, employee.BirthMonth.Value, employee.BirthDay.Value);
                        model.DateOfBirth = $"{dateOfBirth.ToString("MMMM")} {employee.BirthDay.Value.ToString()}".ToUpper();
                    }

                    if (employee.LengthOfService != null && employee.LengthOfService.Value > 0)
                    {
                        if (employee.LengthOfService.Value < 365) { model.LengthOfServiceFormatted = $"{employee.LengthOfService} days".ToUpper(); }
                        else
                        {
                            model.LengthOfServiceFormatted = $"~ {employee.LengthOfService.Value / 365} years {employee.LengthOfService % 365} days".ToUpper();
                        }
                    }
                    else { model.LengthOfServiceFormatted = "0 days".ToUpper(); }
                }
                else
                {
                    model.ViewModelErrorMessage = "No profile record was found for the selected staff.";
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Sorry an error was encountered while attempting to retrieve the record.");
                sb.Append(ex.Message);
                model.ViewModelErrorMessage = sb.ToString();
            }
            return View(model);
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        public async Task<IActionResult> Delete(string id)
        {
            EmployeeProfileViewModel model = new EmployeeProfileViewModel();
            try
            {
                Employee employee = await _ermService.GetEmployeeByIdAsync(id);
                model.Address = employee.Address.ToUpper();
                model.CompanyName = employee.CompanyName.ToUpper();
                model.CurrentDesignation = employee.CurrentDesignation.ToUpper();
                model.DepartmentName = employee.DepartmentName.ToUpper();
                model.Email = employee.Email;
                model.EmployeeID = employee.EmployeeID;
                model.EmployeeNo1 = employee.EmployeeNo1;
                model.EmployeeNo2 = employee.EmployeeNo2;
                model.EmploymentStatus = employee.EmploymentStatus.ToUpper();
                model.FullName = employee.FullName.ToUpper();
                model.GeoPoliticalRegion = employee.GeoPoliticalRegion.ToUpper();
                model.ImagePath = employee.ImagePath;
                model.JobGrade = employee.JobGrade.ToUpper();
                model.LgaOfOrigin = employee.LgaOfOrigin.ToUpper();
                model.LocationName = employee.LocationName.ToUpper();
                model.MaritalStatus = employee.MaritalStatus.ToUpper();
                model.OfficialEmail = employee.OfficialEmail;
                model.PersonID = employee.PersonID;
                model.PhoneNo1 = employee.PhoneNo1;
                model.PhoneNo2 = employee.PhoneNo2;
                model.PlaceOfEngagement = employee.PlaceOfEngagement.ToUpper();
                model.Religion = employee.Religion.ToUpper();
                model.Sex = employee.Sex.ToUpper();
                model.StateOfOrigin = employee.StateOfOrigin.ToUpper();
                model.UnitName = employee.UnitName.ToUpper();

                if (employee.ConfirmationDate != null && employee.ConfirmationDate.HasValue)
                { model.ConfirmationDateFormatted = $"{employee.ConfirmationDate.Value.ToLongDateString()}".ToUpper(); }
                else { model.ConfirmationDateFormatted = string.Empty; }

                if (employee.DateOfLastPromotion != null && employee.DateOfLastPromotion.HasValue)
                { model.DateOfLastPromotionFormatted = $"{employee.DateOfLastPromotion.Value.ToLongDateString()}".ToUpper(); }
                else { model.DateOfLastPromotionFormatted = string.Empty; }

                if (employee.StartUpDate != null && employee.StartUpDate.HasValue)
                { model.StartUpDateFormatted = $"{employee.StartUpDate.Value.ToLongDateString()}".ToUpper(); }
                else { model.StartUpDateFormatted = string.Empty; }

                if (employee.BirthDay.HasValue && employee.BirthDay > 0 && employee.BirthMonth.HasValue && employee.BirthMonth > 0)
                {
                    DateTime dateOfBirth = new DateTime(2020, employee.BirthMonth.Value, employee.BirthDay.Value);
                    model.DateOfBirth = $"{dateOfBirth.ToString("MMMM")} {employee.BirthDay.Value.ToString()}".ToUpper();
                }

                if (employee.LengthOfService != null && employee.LengthOfService.Value > 0)
                {
                    if (employee.LengthOfService.Value < 364) { model.LengthOfServiceFormatted = $"{employee.LengthOfService} days".ToUpper(); }
                    else
                    {
                        model.LengthOfServiceFormatted = $"{employee.LengthOfService.Value / 364} years {employee.LengthOfService % 364} days".ToUpper();
                    }
                }
                else { model.LengthOfServiceFormatted = "0 days".ToUpper(); }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Sorry an error was encountered while attempting to retrieve the record.");
                sb.Append(ex.Message);
                model.ViewModelErrorMessage = sb.ToString();
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        public async Task<IActionResult> Delete(EmployeeProfileViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.EmployeeID))
            {
                model.ViewModelErrorMessage = "Oops! It appears an important value is missing. This record cannot be deleted at this time.";
                return View(model);
            }
            try
            {
                string DeletedBy = HttpContext.User.Identity.Name;
                string DeletedTime = $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}";
                bool IsDeleted = await _ermService.DeleteEmployeeAsync(model.EmployeeID, DeletedBy, DeletedTime);
                if (IsDeleted) { return RedirectToAction("Search"); }
                else { model.ViewModelErrorMessage = "Delete operation failed! Sorry, an error was encountered while attempting to delete this record. Please try again."; }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Sorry an error was encountered while attempting to retrieve the record.");
                sb.Append(ex.Message);
                model.ViewModelErrorMessage = sb.ToString();
            }
            return View(model);
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        public async Task<IActionResult> UploadImage(string id, string nm)
        {
            UploadImageViewModel model = new UploadImageViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                Employee employee = await _ermService.GetEmployeeByIdAsync(id);
                if (employee != null)
                {
                    model.OldImagePath = employee.ImagePath;
                }
            }
            model.EmployeeID = id;
            model.EmployeeName = nm;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        public async Task<IActionResult> UploadImage(UploadImageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ViewModelErrorMessage = "Sorry, image cannot be uploaded. Some key parameters are missing. Please try again.";
                return View(model);
            }

            try
            {
                string uploadsFolder = null;
                string absoluteFilePath = null;

                if (model.UploadedImage != null && model.UploadedImage.Length > 0)
                {
                    if (model.UploadedImage.Length / (1048576) > 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, this image is too large. Image must not exceed 1MB.";
                        return View(model);
                    }
                    var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.UploadedImage.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid image format. Only images of type jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }
                    if (!string.IsNullOrWhiteSpace(model.OldImagePath))
                    {
                        string newImagePath = model.OldImagePath.Substring(1);
                        string newImageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, newImagePath);
                        System.IO.File.Delete(newImageFilePath);
                    }

                    uploadsFolder = "uploads/erm/" + Guid.NewGuid().ToString() + fileExt; //"_" + model.ImageUpload.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    string uploadFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/erm/");
                    //FileInfo file = new FileInfo(absoluteFilePath);
                    if (!Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(absoluteFilePath);
                    }

                    await model.UploadedImage.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                }

                string employeeId = model.EmployeeID;
                string imagePath = "/" + uploadsFolder;
                string updatedBy = HttpContext.User.Identity.Name;

                if (await _ermService.UpdateEmployeeImagePathAsync(employeeId, imagePath, updatedBy))
                {
                    model.OperationIsCompleted = true;
                    model.OperationIsSuccessful = true;
                    model.ViewModelSuccessMessage = $"Image was uploaded successfully!";
                }
                else
                {
                    if (!string.IsNullOrEmpty(absoluteFilePath))
                    {
                        FileInfo file = new FileInfo(absoluteFilePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    model.ViewModelErrorMessage = $"Error! An error was encountered. Image upload failed.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        public async Task<IActionResult> Options(string id)
        {
            EmployeeOptionsViewModel model = new EmployeeOptionsViewModel();
            try
            {
                if (string.IsNullOrWhiteSpace(id)) { throw new Exception("Sorry, required parameter [Employee ID] is missing."); }
                EmployeeOptions o = await _ermService.GetEmployeeOptionsAsync(id);
                if (o != null)
                {
                    model.EmployeeFullName = o.EmployeeFullName;
                    model.EmployeeId = o.EmployeeId;
                    model.LeaveProfileId = o.LeaveProfileId;
                    model.LeaveProfileName = o.LeaveProfileName;
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the requested record was not found. It may have been deleted.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            var leaveProfiles = await _lmsService.GetLeaveProfiles();
            ViewBag.LeaveProfileList = new SelectList(leaveProfiles, "Id", "Name");
            return View(model);
        }

        [Authorize(Roles = "ERMMGAEMR, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> Options(EmployeeOptionsViewModel model)
        {
            try
            {
                EmployeeOptions o = new EmployeeOptions
                {
                    EmployeeId = model.EmployeeId,
                    LeaveProfileId = model.LeaveProfileId
                };

                bool EmployeeIsUpdated = await _ermService.UpdateEmployeeOptionsAsync(o);
                if (EmployeeIsUpdated)
                {
                    model.ViewModelSuccessMessage = "Employee Options updated successfully!";
                }
                else
                {
                    model.ViewModelErrorMessage = "An unknown error was encountered. Updating employee record failed. ";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var leaveProfiles = await _lmsService.GetLeaveProfiles();
            ViewBag.LeaveProfileList = new SelectList(leaveProfiles, "Id", "Name");
            return View(model);
        }
    }
}