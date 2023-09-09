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
    public class UnitsController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _employeeRecordService;
        private readonly IDataProtector _dataProtector;

        public UnitsController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    IErmService employeeRecordService, DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        //================================= Units Action Methods =====================================================================//
        #region Unit Action Methods

        [Authorize(Roles = "GBSVWASTT, GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> List()
        {
            UnitListViewModel model = new UnitListViewModel();
            var entitiesList = await _globalSettingsService.GetUnitsAsync();
            model.UnitList = entitiesList.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddNew()
        {
            UnitViewModel model = new UnitViewModel();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            ViewBag.DeptList = new SelectList(depts, "DepartmentID", "DepartmentName");
            model.UnitID = 0;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> AddNew(UnitViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Unit unit = model.ConvertToUnit();
                    bool succeeded = await _globalSettingsService.CreateUnitAsync(unit);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Unit was created successfully!";
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
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            ViewBag.DeptList = new SelectList(depts, "DepartmentID", "DepartmentName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Edit(int? id)
        {
            Unit unit = new Unit();
            UnitViewModel model = new UnitViewModel();
            if (id > 0)
            {
                unit = await _globalSettingsService.GetUnitAsync(id.Value);
                model.UnitHeadID2 = unit.UnitHeadID2;
                model.UnitHeadID1 = unit.UnitHeadID1;
                model.DepartmentID = unit.DepartmentID;
                model.UnitName = unit.UnitName;
                model.UnitID = unit.UnitID;
            }
            else
            {
                return RedirectToAction("AddNew", "Units");
            }
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            ViewBag.DeptList = new SelectList(depts, "DepartmentID", "DepartmentName");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Edit(UnitViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Unit unit = model.ConvertToUnit();
                    bool succeeded = await _globalSettingsService.UpdateUnitAsync(unit);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"Department was updated successfully!";
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
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            ViewBag.DeptList = new SelectList(depts, "DepartmentID", "DepartmentName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Delete(int? id)
        {
            Unit unit = new Unit();
            UnitViewModel model = new UnitViewModel();
            if (id > 0)
            {
                unit = await _globalSettingsService.GetUnitAsync(id.Value);
                model.UnitHeadName2 = unit.UnitHeadName2;
                model.UnitHeadName1 = unit.UnitHeadName1;
                model.UnitID = unit.UnitID;
                model.UnitName = unit.UnitName;
                model.DepartmentID = unit.DepartmentID;
                model.DepartmentName = unit.DepartmentName;
                model.UnitHeadID2 = unit.UnitHeadID2;
                model.UnitHeadID1 = unit.UnitHeadID1;
            }
            else
            {
                return RedirectToAction("AddNew", "Departments");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSMGASTT, XYALLACCZ")]
        public async Task<IActionResult> Delete(UnitViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _globalSettingsService.DeleteUnitAsync(model.UnitID);
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
    }
}