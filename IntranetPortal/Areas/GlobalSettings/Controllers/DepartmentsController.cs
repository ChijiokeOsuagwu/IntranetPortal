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
    public class DepartmentsController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _employeeRecordService;
        private readonly IDataProtector _dataProtector;
        public DepartmentsController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    IErmService employeeRecordService, DataProtectionEncryptionStrings dataProtectionEncryptionStrings)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _employeeRecordService = employeeRecordService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        //================================= Departments Action Methods =====================================================================//
        #region Department Action Methods

        [Authorize(Roles = "GBSDPTVWL, XYALLACCZ")]
        public async Task<IActionResult> List()
        {
            DepartmentListViewModel model = new DepartmentListViewModel();
            var entitiesList = await _globalSettingsService.GetDepartmentsAsync();
            model.DepartmentList = entitiesList.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSDPTADN, XYALLACCZ")]
        public async Task<IActionResult> AddNew()
        {
            DepartmentViewModel model = new DepartmentViewModel();
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            model.DepartmentID = 0;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSDPTADN, XYALLACCZ")]
        public async Task<IActionResult> AddNew(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Department department = model.ConvertToDepartment();
                    bool succeeded = await _globalSettingsService.CreateDepartmentAsync(department);
                    if (succeeded)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = $"New Department was created successfully!";
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
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSDPTEDT, XYALLACCZ")]
        public async Task<IActionResult> Edit(int? id)
        {
            Department department = new Department();
            DepartmentViewModel model = new DepartmentViewModel();
            if (id > 0)
            {
                department = await _globalSettingsService.GetDepartmentAsync(id.Value);
                model.DepartmentHeadID2 = department.DepartmentHeadID2;
                model.DepartmentHeadID1 = department.DepartmentHeadID1;
                model.DepartmentID = department.DepartmentID;
                model.DepartmentName = department.DepartmentName;
            }
            else
            {
                return RedirectToAction("AddNew", "Departments");
            }
            var staff = await _employeeRecordService.GetAllEmployeesAsync();
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSDPTEDT, XYALLACCZ")]
        public async Task<IActionResult> Edit(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Department department = model.ConvertToDepartment();
                    bool succeeded = await _globalSettingsService.UpdateDepartmentAsync(department);
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
            ViewBag.StaffList = new SelectList(staff, "EmployeeID", "FullName");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "GBSDPTDLT, XYALLACCZ")]
        public async Task<IActionResult> Delete(int? id)
        {
            Department department = new Department();
            DepartmentViewModel model = new DepartmentViewModel();
            if (id > 0)
            {
                department = await _globalSettingsService.GetDepartmentAsync(id.Value);
                model.DepartmentHeadName2 = department.DepartmentHeadName2;
                model.DepartmentHeadName1 = department.DepartmentHeadName1;
                model.DepartmentID = department.DepartmentID;
                model.DepartmentName = department.DepartmentName;
            }
            else
            {
                return RedirectToAction("AddNew", "Departments");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "GBSDPTDLT, XYALLACCZ")]
        public async Task<IActionResult> Delete(DepartmentViewModel model)
        {
            if (model != null)
            {
                try
                {
                    bool succeeded = await _globalSettingsService.DeleteDepartmentAsync(model.DepartmentID);
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