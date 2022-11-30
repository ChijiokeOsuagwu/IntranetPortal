using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmployeesController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings, IErmService ermService,
                                    IBaseModelService baseModelService, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _ermService = ermService;
            _baseModelService = baseModelService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "ERMHMPGVW, XYALLACCZ")]
        public async Task<IActionResult> Search(string sn)
        {
            List<Employee> employees = new List<Employee>();
            if (!string.IsNullOrWhiteSpace(sn))
            {
                employees = await _ermService.SearchEmployeesByNameAsync(sn);
                if (employees != null) { ViewBag.EmployeeList = employees; }
            }
            return View();
        }

        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> Create()
        {
            CreateEmployeeViewModel model = new CreateEmployeeViewModel();
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

        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
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
                string ID = Guid.NewGuid().ToString();
                model.PersonID = ID;
                model.EmployeeID = ID;
                try
                {
                    string uploadsFolder = string.Empty;
                    string absoluteFilePath = string.Empty;
                    if (model.Image != null && model.Image.Length > 0)
                    {
                        var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        FileInfo fileInfo = new FileInfo(model.Image.FileName);
                        var fileExt = fileInfo.Extension;
                        if (!supportedTypes.Contains(fileExt))
                        {
                            throw new Exception("Invalid image format. Only images of type jpg, jpeg, png, gif are permitted.");
                        }
                        uploadsFolder = "/uploads/erm/" + Guid.NewGuid().ToString() + fileExt;
                        absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    }
                    Employee employee = model.ConvertToEmployee();
                    employee.ImagePath = uploadsFolder;
                    employee.ModifiedBy = employee.EmployeeModifiedBy = HttpContext.User.Identity.Name;
                    employee.ModifiedTime =  employee.EmployeeModifiedBy = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";
                    employee.CreatedBy =  employee.EmployeeCreatedBy = HttpContext.User.Identity.Name;
                    employee.CreatedTime = employee.EmployeeCreatedDate = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()} + GMT";

                    bool EmployeeIsCreated = await _ermService.CreateEmployeeAsync(employee);
                    if (EmployeeIsCreated)
                    {
                        if (model.Image != null && !string.IsNullOrWhiteSpace(uploadsFolder))
                        {
                            await model.Image.CopyToAsync(new FileStream(absoluteFilePath, FileMode.Create));
                        }
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


        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
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
                        model.StartUpDate = employee.StartUpDate;
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


        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
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

        public async Task<IActionResult> Profile(string id)
        {
            EmployeeProfileViewModel model = new EmployeeProfileViewModel();
            try
            {
                Employee employee = await _ermService.GetEmployeeByIdAsync(id);
                model.Address = employee.Address;
                model.CompanyName = employee.CompanyName;
                model.CurrentDesignation = employee.CurrentDesignation;
                model.DepartmentName = employee.DepartmentName;
                model.Email = employee.Email;
                model.EmployeeID = employee.EmployeeID;
                model.EmployeeNo1 = employee.EmployeeNo1;
                model.EmployeeNo2 = employee.EmployeeNo2;
                model.EmploymentStatus = employee.EmploymentStatus;
                model.FullName = employee.FullName;
                model.GeoPoliticalRegion = employee.GeoPoliticalRegion;
                model.ImagePath = employee.ImagePath;
                model.JobGrade = employee.JobGrade;
                model.LgaOfOrigin = employee.LgaOfOrigin;
                model.LocationName = employee.LocationName;
                model.MaritalStatus = employee.MaritalStatus;
                model.OfficialEmail = employee.OfficialEmail;
                model.PersonID = employee.PersonID;
                model.PhoneNo1 = employee.PhoneNo1;
                model.PhoneNo2 = employee.PhoneNo2;
                model.PlaceOfEngagement = employee.PlaceOfEngagement;
                model.Religion = employee.Religion;
                model.Sex = employee.Sex;
                model.StateOfOrigin = employee.StateOfOrigin;
                model.UnitName = employee.UnitName;

                if (employee.ConfirmationDate != null && employee.ConfirmationDate.HasValue)
                { model.ConfirmationDateFormatted = $"{employee.ConfirmationDate.Value.ToLongDateString()}"; }
                else { model.ConfirmationDateFormatted = string.Empty; }

                if (employee.DateOfLastPromotion != null && employee.DateOfLastPromotion.HasValue)
                { model.DateOfLastPromotionFormatted = $"{employee.DateOfLastPromotion.Value.ToLongDateString()}"; }
                else { model.DateOfLastPromotionFormatted = string.Empty; }

                if (employee.StartUpDate != null && employee.StartUpDate.HasValue)
                { model.StartUpDateFormatted = $"{employee.StartUpDate.Value.ToLongDateString()}"; }
                else { model.StartUpDateFormatted = string.Empty; }

                if (employee.BirthDay.HasValue && employee.BirthDay > 0 && employee.BirthMonth.HasValue && employee.BirthMonth > 0)
                {
                    DateTime dateOfBirth = new DateTime(2020, employee.BirthMonth.Value, employee.BirthDay.Value);
                    model.DateOfBirth = $"{dateOfBirth.ToString("MMMM")} {employee.BirthDay.Value.ToString()}";
                }

                if (employee.LengthOfService != null && employee.LengthOfService.Value > 0)
                {
                    if (employee.LengthOfService.Value < 364) { model.LengthOfServiceFormatted = $"{employee.LengthOfService} days"; }
                    else
                    {
                        model.LengthOfServiceFormatted = $"{employee.LengthOfService.Value / 364} years {employee.LengthOfService % 364} days";
                    }
                }
                else { model.LengthOfServiceFormatted = "0 days"; }
            }
            catch(Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Sorry an error was encountered while attempting to retrieve the record.");
                sb.Append(ex.Message);
                model.ViewModelErrorMessage = sb.ToString();
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            EmployeeProfileViewModel model = new EmployeeProfileViewModel();
            try
            {
                Employee employee = await _ermService.GetEmployeeByIdAsync(id);
                model.Address = employee.Address;
                model.CompanyName = employee.CompanyName;
                model.CurrentDesignation = employee.CurrentDesignation;
                model.DepartmentName = employee.DepartmentName;
                model.Email = employee.Email;
                model.EmployeeID = employee.EmployeeID;
                model.EmployeeNo1 = employee.EmployeeNo1;
                model.EmployeeNo2 = employee.EmployeeNo2;
                model.EmploymentStatus = employee.EmploymentStatus;
                model.FullName = employee.FullName;
                model.GeoPoliticalRegion = employee.GeoPoliticalRegion;
                model.ImagePath = employee.ImagePath;
                model.JobGrade = employee.JobGrade;
                model.LgaOfOrigin = employee.LgaOfOrigin;
                model.LocationName = employee.LocationName;
                model.MaritalStatus = employee.MaritalStatus;
                model.OfficialEmail = employee.OfficialEmail;
                model.PersonID = employee.PersonID;
                model.PhoneNo1 = employee.PhoneNo1;
                model.PhoneNo2 = employee.PhoneNo2;
                model.PlaceOfEngagement = employee.PlaceOfEngagement;
                model.Religion = employee.Religion;
                model.Sex = employee.Sex;
                model.StateOfOrigin = employee.StateOfOrigin;
                model.UnitName = employee.UnitName;

                if (employee.ConfirmationDate != null && employee.ConfirmationDate.HasValue)
                { model.ConfirmationDateFormatted = $"{employee.ConfirmationDate.Value.ToLongDateString()}"; }
                else { model.ConfirmationDateFormatted = string.Empty; }

                if (employee.DateOfLastPromotion != null && employee.DateOfLastPromotion.HasValue)
                { model.DateOfLastPromotionFormatted = $"{employee.DateOfLastPromotion.Value.ToLongDateString()}"; }
                else { model.DateOfLastPromotionFormatted = string.Empty; }

                if (employee.StartUpDate != null && employee.StartUpDate.HasValue)
                { model.StartUpDateFormatted = $"{employee.StartUpDate.Value.ToLongDateString()}"; }
                else { model.StartUpDateFormatted = string.Empty; }

                if (employee.BirthDay.HasValue && employee.BirthDay > 0 && employee.BirthMonth.HasValue && employee.BirthMonth > 0)
                {
                    DateTime dateOfBirth = new DateTime(2020, employee.BirthMonth.Value, employee.BirthDay.Value);
                    model.DateOfBirth = $"{dateOfBirth.ToString("MMMM")} {employee.BirthDay.Value.ToString()}";
                }

                if (employee.LengthOfService != null && employee.LengthOfService.Value > 0)
                {
                    if (employee.LengthOfService.Value < 364) { model.LengthOfServiceFormatted = $"{employee.LengthOfService} days"; }
                    else
                    {
                        model.LengthOfServiceFormatted = $"{employee.LengthOfService.Value / 364} years {employee.LengthOfService % 364} days";
                    }
                }
                else { model.LengthOfServiceFormatted = "0 days"; }
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

    }
}