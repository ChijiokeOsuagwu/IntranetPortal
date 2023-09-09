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
    public class TeamsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _employeeRecordService;
        private readonly IDataProtector _dataProtector;
        public TeamsController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    IErmService employeeRecordService, DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        //==================== Teams Controller Actions ====================================================================//
        #region Teams Actions

        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> List(string searchString = null)
        {
            TeamListViewModel model = new TeamListViewModel();
            IEnumerable<Team> teamList;
            if (string.IsNullOrEmpty(searchString))
            {
                teamList = await _globalSettingsService.GetTeamsAsync();
            }
            else
            {
                teamList = await _globalSettingsService.SearchTeamsByNameAsync(searchString);
            }
            ViewData["CurrentFilter"] = searchString;
            model.TeamList = teamList.ToList();
            return View(model);
        }

        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        [HttpGet]
        public async Task<IActionResult> AddNew()
        {
            TeamViewModel model = new TeamViewModel();
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddNew(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Team team = model.ConvertToTeam();
                    team.ModifiedBy = HttpContext.User.Identity.Name;
                    bool succeeded = await _globalSettingsService.CreateTeamAsync(team);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Team was created successfully!";
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
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Edit(string id)
        {
            Team team = new Team();
            TeamViewModel model = new TeamViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                team = await _globalSettingsService.GetTeamByIdAsync(id);
                model.TeamID = team.TeamID;
                model.TeamName = team.TeamName;
                model.TeamDescription = team.TeamDescription;
                model.TeamLocationID = team.TeamLocationID;
                model.TeamLocationName = team.TeamLocationName;
            }
            else
            {
                return RedirectToAction("AddNew", "Teams");
            }
            var locations = await _globalSettingsService.GetAllLocationsAsync();
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Edit(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Team team = model.ConvertToTeam();
                    team.ModifiedBy = HttpContext.User.Identity.Name;
                    bool succeeded = await _globalSettingsService.UpdateTeamAsync(team);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Team was updated successfully!";
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
            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Delete(string id)
        {
            Team team = new Team();
            TeamViewModel model = new TeamViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                team = await _globalSettingsService.GetTeamByIdAsync(id);
                model.TeamID = team.TeamID;
                model.TeamName = team.TeamName;
                model.TeamDescription = team.TeamDescription;
                model.TeamLocationID = team.TeamLocationID;
                model.TeamLocationName = team.TeamLocationName;
            }
            else
            {
                return RedirectToAction("List", "Teams");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Delete(TeamViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _globalSettingsService.DeleteTeamAsync(model.TeamID);
                    if (succeeded)
                    {
                        return RedirectToAction("List");
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
        #endregion

        //======================== Team Members Controller Actions ==================================================================//
        #region Team Members Actions

        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Members(string id, string searchString = null)
        {
            TeamMembersListViewModel model = new TeamMembersListViewModel();
            List<TeamMember> teamMembersList = new List<TeamMember>();
            if (!string.IsNullOrEmpty(id))
            {
                model.TeamID = id;
                if (string.IsNullOrWhiteSpace(searchString))
                {
                    var entities = await _globalSettingsService.GetTeamMembersByTeamIdAsync(id);
                    teamMembersList = entities.ToList();
                }
                else
                {
                    var entities = await _globalSettingsService.GetTeamMembersByMemberNameAsync(id, searchString);
                    teamMembersList = entities.ToList();
                }
            }

            model.TeamMembersList = teamMembersList;
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
    }
}