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

        #region Location Groups Actions

        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> LocationGroups()
        {
            LocationGroupListViewModel model = new LocationGroupListViewModel();
            try
            {
                model.LocationGroupList = await _globalSettingsService.GetAllLocationGroupsAsync();
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        [HttpGet]
        public async Task<IActionResult> ManageLocationGroup(int id)
        {
            LocationGroupViewModel model = new LocationGroupViewModel();
            if (id > 0)
            {
                var entity = await _globalSettingsService.GetLocationGroupByIdAsync(id);
                if (entity != null)
                {
                    model.LocationGroupId = entity.LocationGroupId;
                    model.LocationGroupName = entity.LocationGroupName;
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> ManageLocationGroup(LocationGroupViewModel model)
        {
            try
            {
                LocationGroup locationGroup = new LocationGroup();
                locationGroup.LocationGroupName = model.LocationGroupName.ToUpper();
                locationGroup.LocationGroupId = model.LocationGroupId;
                if (ModelState.IsValid)
                {
                    if (model.LocationGroupId < 1)
                    {
                        if (await _globalSettingsService.CreateLocationGroupAsync(locationGroup))
                        {
                            return RedirectToAction("LocationGroups");
                        }
                    }
                    else
                    {
                        if (await _globalSettingsService.UpdateLocationGroupAsync(locationGroup))
                        {
                            return RedirectToAction("LocationGroups");
                        }
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = $"Ooops! It appears some fields have missing or invalid values. Please correct this and try again.";
                    model.OperationIsCompleted = true;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        //==== Location Group Members Controller Actions ===//
        #region Location Group Members Actions

        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> GroupLocationList(int id)
        {
            LocationGroupMembersListViewModel model = new LocationGroupMembersListViewModel();
            model.LocationGroupMembersList = new List<LocationGroupMember>();
            if (id > 0)
            {
                model.LocationGroupId = id;
                if (model.LocationGroupId > 0)
                {
                    var entities = await _globalSettingsService.GetLocationGroupMembersByLocationGroupIdAsync(id);
                    if(entities != null && entities.Count > 0)
                    {
                        model.LocationGroupMembersList = entities.ToList();
                        model.LocationGroupName = entities[0].LocationGroupName;
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddMember(string id)
        {
            TeamMemberViewModel model = new TeamMemberViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                model.TeamID = id;
                var employees = await _globalSettingsService.GetNonTeamMembersByTeamIdAsync(id);
                ViewBag.StaffList = new SelectList(employees, "EmployeeID", "FullName");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddMember(TeamMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TeamMember teamMember = model.ConvertToTeamMember();
                    teamMember.ModifiedBy = HttpContext.User.Identity.Name;
                    bool succeeded = await _globalSettingsService.CreateTeamMemberAsync(teamMember);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Team Member was added successfully!";
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
            var employees = await _globalSettingsService.GetNonTeamMembersByTeamIdAsync(model.TeamID);
            ViewBag.StaffList = new SelectList(employees, "EmployeeID", "FullName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditMember(int id)
        {
            TeamMemberViewModel model = new TeamMemberViewModel();
            if (id >= 1)
            {
                var member = await _globalSettingsService.GetTeamMemberByIdAsync(id);
                model.TeamMemberID = member.TeamMemberID;
                model.TeamID = member.TeamID;
                model.MemberID = member.MemberID;
                model.MemberName = member.FullName;
                model.MemberRole = member.MemberRole;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> EditMember(TeamMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TeamMember teamMember = model.ConvertToTeamMember();
                    teamMember.ModifiedBy = HttpContext.User.Identity.Name;
                    bool succeeded = await _globalSettingsService.UpdateTeamMemberAsync(teamMember);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Team Member was updated successfully!";
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
                model.ViewModelErrorMessage = $"Ooops! Some fields have invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> RemoveMember(int id)
        {
            TeamMemberViewModel model = new TeamMemberViewModel();
            if (id >= 1)
            {
                var member = await _globalSettingsService.GetTeamMemberByIdAsync(id);
                model.TeamMemberID = member.TeamMemberID;
                model.TeamID = member.TeamID;
                model.MemberID = member.MemberID;
                model.MemberName = member.FullName;
                model.MemberRole = member.MemberRole;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> RemoveMember(TeamMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool succeeded = await _globalSettingsService.DeleteTeamMemberAsync(model.TeamMemberID.Value);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Team Member was removed successfully!";
                        return RedirectToAction("Members", new { id = model.TeamID });
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
                model.ViewModelErrorMessage = $"Ooops! Some fields have invalid values. Please correct this and try again.";
                model.OperationIsCompleted = true;
            }
            return View(model);
        }

        #endregion


        //========= Locations Helper Methods =========//
        #region Locations Helper Methods
        [HttpGet]
        public JsonResult GetStateNames(string stateName)
        {
            List<string> locations = _globalSettingsService.SearchStatesAsync(stateName).Result.Select(x => x.Name).ToList();
            return Json(locations);
        }
        [HttpGet]
        public JsonResult GetLocationNames(string text)
        {
            List<string> locations = _globalSettingsService.SearchLocationsAsync(text).Result.Select(x => x.LocationName).ToList();
            return Json(locations);
        }
        public string AddLocationToLocationGroup(string ln, int id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ln)) { throw new Exception("Required parameter Location Name has an invalid value."); }
                if (id < 1) { throw new Exception("Required parameter Location Group ID has an invalid value."); }
                LocationGroupMember locationGroupMember = new LocationGroupMember();
                locationGroupMember.LocationGroupId = id;
                
                Location location = _globalSettingsService.GetLocationByNameAsync(ln).Result;
                if (location != null)
                {
                    locationGroupMember.LocationID = location.LocationID;
                }

                if (_globalSettingsService.AddLocationGroupMemberAsync(locationGroupMember).Result)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string RemoveLocationFromLocationGroup(int id)
        {
            try
            {
                if (id < 1)
                {
                    return "parameter";
                }

                if (_globalSettingsService.DeleteLocationGroupMemberAsync(id).Result)
                {
                    return "success";
                }
                else
                {
                    return "failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}