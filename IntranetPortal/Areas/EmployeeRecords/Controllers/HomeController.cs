using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.EmployeeRecords.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IntranetPortal.Areas.EmployeeRecords.Controllers
{
    [Area("EmployeeRecords")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IEmployeeRecordService _employeeRecordService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        public HomeController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings, IEmployeeRecordService employeeRecordService,
                                    IBaseModelService baseModelService)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
            _baseModelService = baseModelService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "ERMHMPGVW, XYALLACCZ")]
        public IActionResult index()
        {
            return View();
        }

        [Authorize(Roles = "ERMSTFVWL, XYALLACCZ")]
        public async Task<IActionResult> List(string sortOrder, string currentFilter, string searchString, int? pageNumber)
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
                employees = await _employeeRecordService.GetEmployeesByNameAsync(searchString);
            }
            else
            {
                employees = await _employeeRecordService.GetAllEmployeesAsync();
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

        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public IActionResult Create()
        {
            Employee model = new Employee();
            return View(model);
        }

        [Authorize(Roles = "ERMSTFVWD, XYALLACCZ")]
        public async Task<IActionResult> Details(string id)
        {
            Employee employee = new Employee();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    employee = await _employeeRecordService.GetEmployeesByIdAsync(id);
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

        [Authorize(Roles = "ERMSTFDLT, XYALLACCZ")]
        public async Task<IActionResult> Delete(string id)
        {
            Employee employee = new Employee();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    employee = await _employeeRecordService.GetEmployeesByIdAsync(id);
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
                ViewBag.ErrorMessage = ex.Message ?? string.Empty;
            }

            return View(employee);
        }

        [Authorize(Roles = "ERMSTFDLT, XYALLACCZ")]
        public async Task<IActionResult> CompleteDelete(string id)
        {
            try
            {
                bool IsDeleted = false;
                if (!string.IsNullOrEmpty(id))
                {
                    IsDeleted = await _employeeRecordService.DeleteEmployeeAsync(id);
                    if (IsDeleted)
                    {
                        return RedirectToAction("List");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message ?? string.Empty;
            }

            Employee employee = await _employeeRecordService.GetEmployeesByIdAsync(id);
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
            return View("Delete", employee);
        }

        [HttpGet]
        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> AddPersonalInfo(string id)
        {
            PersonInfoViewModel model = new PersonInfoViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                Person person = await _baseModelService.GetPersonAsync(id);
                model.Address = person.Address;
                model.BirthDay = person.BirthDay;
                model.BirthMonth = person.BirthMonth;
                model.BirthYear = person.BirthYear;
                model.Email = person.Email;
                model.FirstName = person.FirstName;
                model.MaritalStatus = person.MaritalStatus;
                model.OtherNames = person.OtherNames;
                model.PersonID = person.PersonID;
                model.PhoneNo1 = person.PhoneNo1;
                model.PhoneNo2 = person.PhoneNo2;
                model.Sex = person.Sex;
                model.Surname = person.Surname;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> AddPersonalInfo(PersonInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool PersonExists = false;
                bool succeeded = false;
                try
                {
                    Person person = model.ConvertToPerson();

                    if (!string.IsNullOrEmpty(model.PersonID))
                    {
                        PersonExists = await _baseModelService.PersonExistsAsync(person.PersonID);
                    }

                    if (PersonExists)
                    {
                        succeeded = await _baseModelService.UpdatePersonAsync(person);
                    }
                    else
                    {
                        person.PersonID = Guid.NewGuid().ToString();
                        succeeded = await _baseModelService.CreatePersonAsync(person);
                    }

                    if (succeeded)
                    {
                        TempData["PersonID"] = person.PersonID;
                        TempData["PersonName"] = person.FullName;
                        return RedirectToAction("AddEmployeeBasicInfo", new { id = person.PersonID });
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
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> AddEmployeeBasicInfo(string id)
        {
            string PersonID = string.Empty;
            string PersonName = string.Empty;
            if (!string.IsNullOrEmpty(id)) { PersonID = id; }
            else
            {
                PersonID = TempData["PersonID"].ToString();
            }

            PersonName = TempData["PersonName"].ToString();

            EmployeeBasicInfoViewModel model = new EmployeeBasicInfoViewModel();

            try
            {
                if (!string.IsNullOrEmpty(PersonID))
                {
                    Employee employee = await _employeeRecordService.GetEmployeesByIdAsync(PersonID);
                    if (employee != null)
                    {
                        model.CompanyCode = employee.CompanyID;
                        model.DepartmentID = employee.DepartmentID;
                        model.EmployeeID = employee.EmployeeID;
                        model.EmployeeName = employee.FullName;
                        model.EmployeeNo1 = employee.EmployeeNo1;
                        model.EmployeeNo2 = employee.EmployeeNo2;
                        model.EmploymentStatus = employee.EmploymentStatus;
                        model.GeoPoliticalRegion = employee.GeoPoliticalRegion;
                        model.JobGrade = employee.JobGrade;
                        model.LgaOfOrigin = employee.LgaOfOrigin;
                        model.LocationID = employee.LocationID;
                        model.OfficialEmail = employee.OfficialEmail;
                        model.Religion = employee.Religion;
                        model.ReportsTo1_EmployeeID = employee.ReportsTo1_EmployeeID;
                        model.ReportsTo2_EmployeeID = employee.ReportsTo2_EmployeeID;
                        model.ReportsTo3_EmployeeID = employee.ReportsTo3_EmployeeID;
                        model.StartUpDate = employee.StartUpDate;
                        model.StartUpDesignation = employee.StartUpDesignation;
                        model.StateOfOrigin = employee.StateOfOrigin;
                        model.UnitID = employee.UnitID;
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = $"Sorry, an error was encountered. A call to retrieve this staff's personal info returned an invalid value.";
                    model.OperationIsCompleted = true;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                model.EmployeeID = id;
            }
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var companies = await _globalSettingsService.GetCompaniesAsync();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.CompanyList = new SelectList(companies, "CompanyCode", "CompanyName");
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> AddEmployeeBasicInfo(EmployeeBasicInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    EmployeeBasicInfo employeeInfo = model.ConvertToEmployeeBasicInfo();
                    string StartYear = string.Empty;
                    string StartMonth = string.Empty;
                    if (employeeInfo.StartUpDate != null)
                    {
                        StartYear = employeeInfo.StartUpDate.Value.Year.ToString().Substring(2, 2);
                        StartMonth = String.Format("{0:00}", employeeInfo.StartUpDate.Value.Month);
                    }

                    string CodeNumber = await _baseModelService.GenerateAutoNumberAsync("empno");
                    if (!string.IsNullOrEmpty(CodeNumber))
                    {
                        employeeInfo.EmployeeNo1 = $"{employeeInfo.CompanyCode}{StartYear}{StartMonth}{CodeNumber}";
                        State state = await _globalSettingsService.GetStateAsync(employeeInfo.StateOfOrigin);
                        if (state != null) { employeeInfo.GeoPoliticalRegion = state.Region; }
                        else
                        {
                            model.ViewModelErrorMessage = $"An error was encountered. A call to retrieve the GeoPolitical Region returned an invalid value.";
                            model.OperationIsSuccessful = false;
                        }

                        Unit unit = await _globalSettingsService.GetUnitAsync(model.UnitID.Value);

                        if (unit != null) { employeeInfo.DepartmentID = unit.DepartmentID; }
                        else
                        {
                            model.ViewModelErrorMessage = $"An error was encountered. A call to retrieve the GeoPolitical Region returned an invalid value.";
                            model.OperationIsSuccessful = false;
                        }

                        bool IsSuccess = false;

                        if (await _employeeRecordService.EmployeeExistsAsync(model.EmployeeID))
                        {
                            IsSuccess = await _employeeRecordService.UpdateEmployeeBasicInfoAsync(employeeInfo);
                        }
                        else
                        {
                            IsSuccess = await _employeeRecordService.CreateEmployeeBasicInfoAsync(employeeInfo);
                        }

                        if (IsSuccess)
                        {
                            return RedirectToAction("AddEmployeeNextOfKinInfo", new { id = employeeInfo.EmployeeID });
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"An error was encountered. Employee Number returned an invalid value.";
                        model.OperationIsSuccessful = false;
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
                model.OperationIsCompleted = true;
            }

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var companies = await _globalSettingsService.GetCompaniesAsync();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.CompanyList = new SelectList(companies, "CompanyCode", "CompanyName");
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> AddEmployeeNextOfKinInfo(string id)
        {
            string employeeId = string.Empty;
            if (!string.IsNullOrEmpty(id)) { employeeId = id; }
            else
            {
                employeeId = TempData["EmployeeID"].ToString();
            }

            EmployeeNextOfKinInfoViewModel model = new EmployeeNextOfKinInfoViewModel();
            try
            {
                if (!string.IsNullOrEmpty(employeeId))
                {
                    Employee employee = await _employeeRecordService.GetEmployeesByIdAsync(employeeId);
                    if (employee != null)
                    {
                        model.EmployeeID = employee.EmployeeID;
                        model.EmployeeName = employee.FullName;
                        model.EmployeeNo1 = employee.EmployeeNo1;
                        model.NextOfKinAddress = employee.NextOfKinAddress;
                        model.NextOfKinEmail = employee.NextOfKinEmail;
                        model.NextOfKinName = employee.NextOfKinName;
                        model.NextOfKinPhone = employee.NextOfKinPhone;
                        model.NextOfKinRelationship = employee.NextOfKinRelationship;
                    }
                    else
                    {
                        model.EmployeeID = employeeId;
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = $"Sorry, an error was encountered. A call to retrieve this staff's info returned an invalid value.";
                    model.OperationIsCompleted = true;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                model.EmployeeID = id;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> AddEmployeeNextOfKinInfo(EmployeeNextOfKinInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    EmployeeNextOfKinInfo employeeInfo = model.ConvertToEmployeeNextOfKinInfo();
                    bool IsAdded = await _employeeRecordService.UpdateEmployeeNextOfKinInfoAsync(employeeInfo);
                    if (IsAdded)
                    {
                        TempData["EmployeeID"] = employeeInfo.EmployeeID;
                        return RedirectToAction("AddEmployeeHistoryInfo");
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
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> AddEmployeeHistoryInfo(string id)
        {
            string employeeId = string.Empty;
            if (!string.IsNullOrEmpty(id)) { employeeId = id; }
            else
            {
                employeeId = TempData["EmployeeID"].ToString();
            }

            EmployeeHistoryInfoViewModel model = new EmployeeHistoryInfoViewModel();
            try
            {
                if (!string.IsNullOrEmpty(employeeId))
                {
                    Employee employee = await _employeeRecordService.GetEmployeesByIdAsync(employeeId);
                    if (employee != null)
                    {
                        model.EmployeeID = employee.EmployeeID;
                        model.EmployeeName = employee.FullName;
                        model.EmployeeNo1 = employee.EmployeeNo1;
                        model.ConfirmationDate = employee.ConfirmationDate;
                        model.CurrentDesignation = employee.CurrentDesignation;
                        model.DateOfLastPromotion = employee.DateOfLastPromotion;
                        model.EmployeeModifiedBy = employee.EmployeeModifiedBy;
                        model.EmployeeModifiedDate = employee.EmployeeModifiedDate;
                        model.LengthOfService = employee.LengthOfService;
                        model.PlaceOfEngagement = employee.PlaceOfEngagement;
                        model.YearsOfExperience = employee.YearsOfExperience;
                    }
                    else
                    {
                        model.EmployeeID = employeeId;
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = $"Sorry, an error was encountered. A call to retrieve this staff's info returned an invalid value.";
                    model.OperationIsCompleted = true;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
                model.EmployeeID = id;
                model.OperationIsCompleted = true;
            }

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationDescription", "LocationDescription");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "ERMSTFADN, XYALLACCZ")]
        public async Task<IActionResult> AddEmployeeHistoryInfo(EmployeeHistoryInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    EmployeeHistoryInfo employeeHistoryInfo = model.ConvertToEmployeeHistoryInfo();
                    bool IsAdded = await _employeeRecordService.UpdateEmployeeHistoryInfoAsync(employeeHistoryInfo);
                    if (IsAdded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Employee History Information was saved successfully!";
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
                model.OperationIsCompleted = true;
            }

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationsList = new SelectList(locations, "LocationDescription", "LocationDescription");
            return View(model);
        }

        //======================================= Employees Helper Methods ===========================================//
        #region Employees Helper Methods

        [HttpGet]
        public JsonResult GetEmployeeNames(string text)
        {
            List<string> employees = _employeeRecordService.GetEmployeesByNameAsync(text).Result.Select(x => x.FullName).ToList();
            return Json(employees);
        }

        [HttpGet]
        public JsonResult GetEmployeeParameters(string nm)
        {
            Employee employee = new Employee();

            List<Employee> employees = _employeeRecordService.GetEmployeesByNameAsync(nm).Result.ToList();
           
            if (employees.Count == 1)
            {
                employee = employees.FirstOrDefault();
            }
            else
            {
                employee = new Employee
                {
                    EmployeeID = string.Empty,
                    //AssetTypeID = -1
                };
            }

            string model = JsonConvert.SerializeObject(new 
            { emp_id = employee.EmployeeID, 
                emp_name = employee.FullName, 
                unit_id = employee.UnitID, 
                dept_id = employee.DepartmentID,
            station_id = employee.LocationID,
            }, Formatting.Indented);

            return Json(model);
        }

        #endregion
    }
}