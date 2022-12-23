using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.ERM.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.ERM.Controllers
{
    [Area("ERM")]
    public class ReportingController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        public ReportingController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings, IErmService ermService,
                                    IBaseModelService baseModelService)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _ermService = ermService;
            _baseModelService = baseModelService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }


        //======================== Employee ReportLines Action Methods =================================//
        #region Employee Report Lines Methods
        public async Task<IActionResult> ReportLines(string id, string sp)
        {
            EmployeeReportLineListViewModel model = new EmployeeReportLineListViewModel();
            model.sp = sp ?? "active";
            if (!string.IsNullOrWhiteSpace(id))
            {
                Person staff = await _baseModelService.GetPersonAsync(id);
                if (staff != null && !string.IsNullOrWhiteSpace(staff.FullName))
                {
                    model.StaffName = staff.FullName;
                }
                model.EmployeeID = id;
                if (!string.IsNullOrWhiteSpace(sp) && sp == "active")
                {
                    var entities = await _ermService.GetActiveEmployeeReportLinesByEmployeeIdAsync(id);
                    model.EmployeeReportLineList = entities.ToList();
                }
                else
                {
                    var entities = await _ermService.GetEmployeeReportLinesByEmployeeIdAsync(id);
                    model.EmployeeReportLineList = entities.ToList();
                }
            }
            return View(model);
        }

        public async Task<IActionResult> AddReportingLine(string id)
        {
            EmployeeReportLineViewModel model = new EmployeeReportLineViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                Person person = await _baseModelService.GetPersonAsync(id);
                model.EmployeeName = person.FullName;
                model.EmployeeID = person.PersonID;
            }
            else
            {
                model.OperationIsCompleted = true;
                model.ViewModelErrorMessage = $"Sorry an error was encountered. The required parameter [EmployeeID] is missing or has an invalid value.";
            }
            var teamEntities = await _globalSettingsService.GetTeamsAsync();
            ViewBag.TeamsList = new SelectList(teamEntities.ToList(), "TeamID", "TeamName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReportingLine(EmployeeReportLineViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool succeeded = false;
                try
                {
                    EmployeeReportLine employeeReportLine = model.ConvertToEmployeeReportLine();
                    Employee reportsToEmployee = await _ermService.GetEmployeeByNameAsync(model.ReportsToEmployeeName);
                    if (!string.IsNullOrWhiteSpace(reportsToEmployee.EmployeeID))
                    {
                        employeeReportLine.ReportsToEmployeeID = reportsToEmployee.EmployeeID;
                        employeeReportLine.ReportsToEmployeeName = reportsToEmployee.FullName;
                        employeeReportLine.UnitID = reportsToEmployee.UnitID;
                        employeeReportLine.DepartmentID = reportsToEmployee.DepartmentID;
                        if (string.IsNullOrWhiteSpace(model.ReportsToEmployeeRole))
                        {
                            employeeReportLine.ReportsToEmployeeRole = reportsToEmployee.CurrentDesignation;
                        }
                    }
                    else
                    {
                        model.OperationIsCompleted = true;
                        model.ViewModelErrorMessage = $"Sorry an error was encountered. The required parameter [ReportsToEmployeeID] is missing or has an invalid value.";
                    }
                    succeeded = await _ermService.CreateEmployeeReportLineAsync(employeeReportLine);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "New Reporting Line added successfully!";
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
            var teamEntities = await _globalSettingsService.GetTeamsAsync();
            ViewBag.TeamsList = new SelectList(teamEntities.ToList(), "TeamID", "TeamName");
            return View(model);
        }

        public async Task<IActionResult> EditReportingLine(int id)
        {
            EmployeeReportLineViewModel model = new EmployeeReportLineViewModel();
            if (id > 0)
            {
                EmployeeReportLine employeeReportLine = await _ermService.GetEmployeeReportLineByIdAsync(id);
                if (employeeReportLine != null)
                {
                    model.DepartmentID = employeeReportLine.DepartmentID;
                    model.DepartmentName = employeeReportLine.DepartmentName;
                    model.EmployeeID = employeeReportLine.EmployeeID;
                    model.EmployeeName = employeeReportLine.EmployeeName;
                    model.ReportEndDate = employeeReportLine.ReportEndDate;
                    model.ReportingLineID = employeeReportLine.ReportingLineID;
                    model.ReportStartDate = employeeReportLine.ReportStartDate;
                    model.ReportsToEmployeeID = employeeReportLine.ReportsToEmployeeID;
                    model.ReportsToEmployeeName = employeeReportLine.ReportsToEmployeeName;
                    model.ReportsToEmployeeRole = employeeReportLine.ReportsToEmployeeRole;
                    model.TeamID = employeeReportLine.TeamID;
                    model.TeamName = employeeReportLine.TeamName;
                    model.UnitID = employeeReportLine.UnitID;
                    model.UnitName = employeeReportLine.UnitName;
                }
                else
                {
                    model.OperationIsCompleted = true;
                    model.ViewModelErrorMessage = $"Sorry an error was encountered. The selected record could not be found.";
                }
            }
            else
            {
                model.OperationIsCompleted = true;
                model.ViewModelErrorMessage = $"Sorry an error was encountered. The required parameter [EmployeeID] is missing or has an invalid value.";
            }
            var teamEntities = await _globalSettingsService.GetTeamsAsync();
            ViewBag.TeamsList = new SelectList(teamEntities.ToList(), "TeamID", "TeamName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditReportingLine(EmployeeReportLineViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool succeeded = false;
                try
                {
                    EmployeeReportLine employeeReportLine = model.ConvertToEmployeeReportLine();
                    Employee employee = await _ermService.GetEmployeeByNameAsync(model.ReportsToEmployeeName);
                    if (!string.IsNullOrWhiteSpace(employee.EmployeeID))
                    {
                        employeeReportLine.ReportsToEmployeeID = employee.EmployeeID;
                        employeeReportLine.ReportsToEmployeeName = employee.FullName;
                        employeeReportLine.UnitID = employee.UnitID;
                        employeeReportLine.DepartmentID = employee.DepartmentID;
                        if (string.IsNullOrWhiteSpace(model.ReportsToEmployeeRole))
                        {
                            employeeReportLine.ReportsToEmployeeRole = employee.CurrentDesignation;
                        }
                    }
                    else
                    {
                        model.OperationIsCompleted = true;
                        model.ViewModelErrorMessage = $"Sorry an error was encountered. The required parameter [ReportsToEmployeeID] is missing or has an invalid value.";
                    }
                    succeeded = await _ermService.UpdateEmployeeReportLineAsync(employeeReportLine);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "New Reporting Line updated successfully!";
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
            var teamEntities = await _globalSettingsService.GetTeamsAsync();
            ViewBag.TeamsList = new SelectList(teamEntities.ToList(), "TeamID", "TeamName");
            return View(model);
        }

        public async Task<IActionResult> ReportingLineDetails(int id)
        {
            EmployeeReportLineViewModel model = new EmployeeReportLineViewModel();
            if (id > 0)
            {
                EmployeeReportLine employeeReportLine = await _ermService.GetEmployeeReportLineByIdAsync(id);
                if (employeeReportLine != null)
                {
                    model.DepartmentID = employeeReportLine.DepartmentID;
                    model.DepartmentName = employeeReportLine.DepartmentName;
                    model.EmployeeID = employeeReportLine.EmployeeID;
                    model.EmployeeName = employeeReportLine.EmployeeName;
                    model.ReportEndDate = employeeReportLine.ReportEndDate;
                    model.ReportingLineID = employeeReportLine.ReportingLineID;
                    model.ReportStartDate = employeeReportLine.ReportStartDate;
                    model.ReportsToEmployeeID = employeeReportLine.ReportsToEmployeeID;
                    model.ReportsToEmployeeName = employeeReportLine.ReportsToEmployeeName;
                    model.ReportsToEmployeeRole = employeeReportLine.ReportsToEmployeeRole;
                    model.TeamID = employeeReportLine.TeamID;
                    model.TeamName = employeeReportLine.TeamName;
                    model.UnitID = employeeReportLine.UnitID;
                    model.UnitName = employeeReportLine.UnitName;
                }
            }
            else
            {
                model.OperationIsCompleted = true;
                model.ViewModelErrorMessage = $"Sorry an error was encountered. The required parameter [EmployeeID] is missing or has an invalid value.";
            }
            //var teams = _globalSettingsService.GetTeamsAsync().Result.ToList();
            //ViewBag.TeamsList = new SelectList(teams, "TeamID", "TeamName");
            return View(model);
        }

        public async Task<IActionResult> DeleteReportingLine(int id)
        {
            EmployeeReportLineViewModel model = new EmployeeReportLineViewModel();
            if (id > 0)
            {
                EmployeeReportLine employeeReportLine = await _ermService.GetEmployeeReportLineByIdAsync(id);
                if (employeeReportLine != null)
                {
                    model.DepartmentID = employeeReportLine.DepartmentID;
                    model.DepartmentName = employeeReportLine.DepartmentName;
                    model.EmployeeID = employeeReportLine.EmployeeID;
                    model.EmployeeName = employeeReportLine.EmployeeName;
                    model.ReportEndDate = employeeReportLine.ReportEndDate;
                    model.ReportingLineID = employeeReportLine.ReportingLineID;
                    model.ReportStartDate = employeeReportLine.ReportStartDate;
                    model.ReportsToEmployeeID = employeeReportLine.ReportsToEmployeeID;
                    model.ReportsToEmployeeName = employeeReportLine.ReportsToEmployeeName;
                    model.ReportsToEmployeeRole = employeeReportLine.ReportsToEmployeeRole;
                    model.TeamID = employeeReportLine.TeamID;
                    model.TeamName = employeeReportLine.TeamName;
                    model.UnitID = employeeReportLine.UnitID;
                    model.UnitName = employeeReportLine.UnitName;
                }
            }
            else
            {
                model.OperationIsCompleted = true;
                model.ViewModelErrorMessage = $"Sorry an error was encountered. The required parameter [EmployeeID] is missing or has an invalid value.";
            }
            //var teams = _globalSettingsService.GetTeamsAsync().Result.ToList();
            //ViewBag.TeamsList = new SelectList(teams, "TeamID", "TeamName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReportingLine(EmployeeReportLineViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool succeeded = false;
                try
                {
                    if (model.ReportingLineID > 0)
                    {
                        succeeded = await _ermService.DeleteEmployeeReportLineAsync(model.ReportingLineID.Value);
                        if (succeeded)
                        {
                            return RedirectToAction("ReportLines", new { id = model.EmployeeID });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = $"Sorry, an error was encountered. Delete operation failed.";
                            model.OperationIsCompleted = true;
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = $"Sorry, an error was encountered. The required parameter [ReportLineID] has an invalid value.";
                        model.OperationIsCompleted = true;
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
            var teams = _globalSettingsService.GetTeamsAsync().Result.ToList();
            ViewBag.TeamsList = new SelectList(teams, "TeamID", "TeamName");
            return View(model);
        }

        #endregion

    }
}