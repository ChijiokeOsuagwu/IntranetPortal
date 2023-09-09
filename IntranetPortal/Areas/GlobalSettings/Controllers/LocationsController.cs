using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.GlobalSettings.Models;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.GlobalSettings.Controllers
{
    [Area("GlobalSettings")]
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _employeeRecordService;
        private readonly IDataProtector _dataProtector;
        public LocationsController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    IErmService employeeRecordService, DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        //================================= Station Action Methods =====================================================================//
        #region Station Action Methods

        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Stations()
        {
            LocationsListViewModel model = new LocationsListViewModel();
            var entitiesList = await _globalSettingsService.GetStationsAsync();
            model.LocationList = entitiesList.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddStation()
        {
            LocationAddViewModel model = new LocationAddViewModel();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            model.LocationType = "Station";
            model.LocationID = 0;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddStation(LocationAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                Location location = model.ConvertToLocation();
                location.ModifiedBy = "System Administrator";
                bool succeeded = await _globalSettingsService.CreateLocationAsync(location);
                if (succeeded)
                {
                    model.OperationIsCompleted = true;
                    model.OperationIsSuccessful = true;
                    model.ViewModelSuccessMessage = $"New Station was created successfully!";
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            model.LocationType = "Station";
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditStation(int? id)
        {
            Location location = new Location();
            LocationAddViewModel model = new LocationAddViewModel();
            if (id > 0)
            {
                location = await _globalSettingsService.GetLocationByIdAsync(id.Value);
                model.Country = location.LocationCountry;
                model.LocationHeadID1 = location.LocationHeadID1;
                model.LocationHeadID2 = location.LocationHeadID2;
                model.LocationID = location.LocationID;
                model.LocationType = location.LocationType;
                model.Name = location.LocationName;
                model.State = location.LocationState;
            }
            else
            {
                return RedirectToAction("AddStation", "Locations");
            }
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditStation(LocationAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                Location location = model.ConvertToLocation();
                location.ModifiedBy = "System Administrator";
                bool succeeded = await _globalSettingsService.UpdateLocationAsync(location);
                if (succeeded)
                {
                    model.OperationIsCompleted = true;
                    model.OperationIsSuccessful = true;
                    model.ViewModelSuccessMessage = $"Station was updated successfully!";
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            model.LocationType = "Station";
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteStation(int? id)
        {
            Location location = new Location();
            LocationAddViewModel model = new LocationAddViewModel();
            if (id > 0)
            {
                location = await _globalSettingsService.GetLocationByIdAsync(id.Value);
                model.Country = location.LocationCountry;
                model.LocationHeadID1 = location.LocationHeadID1;
                model.LocationHeadID2 = location.LocationHeadID2;
                model.LocationID = location.LocationID;
                model.LocationType = location.LocationType;
                model.Name = location.LocationName;
                model.State = location.LocationState;
            }
            else
            {
                return RedirectToAction("Stations", "Locations");
            }
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteStation(LocationAddViewModel model)
        {
            if (model != null)
            {
                bool succeeded = await _globalSettingsService.DeleteLocationAsync(model.LocationID);
                if (succeeded)
                {
                    return RedirectToAction("Stations", "Locations");
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }
        #endregion

        //==================================== Bureau Action Methods =================================================//
        #region Bureaus Action Methods

        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Bureaus()
        {
            LocationsListViewModel model = new LocationsListViewModel();
            var entitiesList = await _globalSettingsService.GetBureausAsync();
            model.LocationList = entitiesList.ToList();
            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddBureau()
        {
            LocationAddViewModel model = new LocationAddViewModel();
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            model.LocationType = "Bureau";
            model.LocationID = 0;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddBureau(LocationAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                Location location = model.ConvertToLocation();
                location.ModifiedBy = "System Administrator";
                bool succeeded = await _globalSettingsService.CreateLocationAsync(location);
                if (succeeded)
                {
                    model.OperationIsCompleted = true;
                    model.OperationIsSuccessful = true;
                    model.ViewModelSuccessMessage = $"New Bureau was created successfully!";
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            model.LocationType = "Bureau";
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditBureau(int? id)
        {
            Location location = new Location();
            LocationAddViewModel model = new LocationAddViewModel();
            if (id > 0)
            {
                location = await _globalSettingsService.GetLocationByIdAsync(id.Value);
                model.Country = location.LocationCountry;
                model.LocationHeadID1 = location.LocationHeadID1;
                model.LocationHeadID2 = location.LocationHeadID2;
                model.LocationID = location.LocationID;
                model.LocationType = location.LocationType;
                model.Name = location.LocationName;
                model.State = location.LocationState;
            }
            else
            {
                return RedirectToAction("AddBureau", "Locations");
            }
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditBureau(LocationAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                Location location = model.ConvertToLocation();
                location.ModifiedBy = "System Administrator";
                bool succeeded = await _globalSettingsService.UpdateLocationAsync(location);
                if (succeeded)
                {
                    model.OperationIsCompleted = true;
                    model.OperationIsSuccessful = true;
                    model.ViewModelSuccessMessage = $"Bureau was updated successfully!";
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            model.LocationType = "Bureau";
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteBureau(int? id)
        {
            Location location = new Location();
            LocationAddViewModel model = new LocationAddViewModel();
            if (id > 0)
            {
                location = await _globalSettingsService.GetLocationByIdAsync(id.Value);
                model.Country = location.LocationCountry;
                model.LocationHeadID1 = location.LocationHeadID1;
                model.LocationHeadID2 = location.LocationHeadID2;
                model.LocationID = location.LocationID;
                model.LocationType = location.LocationType;
                model.Name = location.LocationName;
                model.State = location.LocationState;
            }
            else
            {
                return RedirectToAction("Bureaus", "Locations");
            }
            var states = await _globalSettingsService.GetStatesAsync();
            var countries = await _globalSettingsService.GetCountriesAsync();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StateList = new SelectList(states, "Name", "Name");
            ViewBag.CountryList = new SelectList(countries, "CountryName", "CountryName");
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteBureau(LocationAddViewModel model)
        {
            if (model != null)
            {
                bool succeeded = await _globalSettingsService.DeleteLocationAsync(model.LocationID);
                if (succeeded)
                {
                    return RedirectToAction("Bureaus", "Locations");
                }
            }
            else
            {
                model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        #endregion

        //======================================= Locations Helper Methods ===========================================//
        #region Locations Helper Methods
        [HttpGet]
        public JsonResult GetStateNames(string stateName)
        {
            List<string> locations = _globalSettingsService.SearchStatesAsync(stateName).Result.Select(x => x.Name).ToList();
            return Json(locations);
        }
        #endregion

    }
}