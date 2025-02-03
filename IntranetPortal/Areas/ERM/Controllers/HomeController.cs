using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using IntranetPortal.Areas.ERM.Models;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IntranetPortal.Areas.ERM.Controllers
{
    [Area("ERM")]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        public HomeController(IConfiguration configuration,
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

        [Authorize(Roles = "ERMVWAEMR, XYALLACCZ")]
        public IActionResult index()
        {
            return View();
        }

        [Authorize(Roles = "ERMVWAEMR, XYALLACCZ")]
        public async Task<IActionResult> ActiveList(EmployeeListViewModel model)
        {
            List<Employee> employees = new List<Employee>();

            try
            {
                if (model.TerminalDate == null)
                {
                    model.TerminalDate = DateTime.Today;
                }

                if (string.IsNullOrWhiteSpace(model.CompanyCode))
                {
                    if (model.LocationID != null && model.LocationID > 0)
                    {
                        if (model.DepartmentID != null && model.DepartmentID > 0)
                        {
                            if (model.UnitID != null && model.UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByLocationAsync(model.LocationID.Value, model.DepartmentID.Value, model.UnitID.Value, model.TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByLocationAsync(model.LocationID.Value, model.DepartmentID.Value, model.TerminalDate);
                            }
                        }
                        else
                        {
                            if (model.UnitID != null && model.UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByLocationAndUnitAsync(model.LocationID.Value, model.UnitID.Value, model.TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByLocationAsync(model.LocationID.Value, model.TerminalDate);
                            }
                        }
                    }
                    else
                    {
                        if (model.DepartmentID != null && model.DepartmentID > 0)
                        {
                            if (model.UnitID != null && model.UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByUnitIDAsync(model.UnitID.Value, model.TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByDepartmentIDAsync(model.DepartmentID.Value, model.TerminalDate);
                            }
                        }
                        else
                        {
                            if (model.UnitID != null && model.UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByUnitIDAsync(model.UnitID.Value, model.TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetAllEmployeesAsync(model.TerminalDate);
                            }
                        }
                    }
                }
                else
                {
                    if (model.LocationID != null && model.LocationID > 0)
                    {
                        if (model.DepartmentID != null && model.DepartmentID > 0)
                        {
                            if (model.UnitID != null && model.UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(model.CompanyCode, model.LocationID.Value, model.UnitID.Value, model.TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndLocationAsync(model.CompanyCode, model.LocationID.Value, model.DepartmentID.Value, model.TerminalDate);
                            }
                        }
                        else
                        {
                            if (model.UnitID != null && model.UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(model.CompanyCode, model.LocationID.Value, model.UnitID.Value, model.TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndLocationAsync(model.CompanyCode, model.LocationID.Value, model.TerminalDate);
                            }
                        }
                    }
                    else
                    {
                        if (model.DepartmentID != null && model.DepartmentID > 0)
                        {
                            if (model.UnitID != null && model.UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(model.CompanyCode, model.UnitID.Value, model.TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndDepartmentAsync(model.CompanyCode, model.DepartmentID.Value, model.TerminalDate);
                            }
                        }
                        else
                        {
                            if (model.UnitID != null && model.UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(model.CompanyCode, model.UnitID.Value, model.TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAsync(model.CompanyCode, model.TerminalDate);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            finally
            {
                model.EmployeesList = employees;
                var locations = await _globalSettingsService.GetAllLocationsAsync();
                var companies = await _globalSettingsService.GetCompaniesAsync();
                var units = await _globalSettingsService.GetUnitsAsync();
                var depts = await _globalSettingsService.GetDepartmentsAsync();

                ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
                ViewBag.CompanyList = new SelectList(companies, "CompanyCode", "CompanyName");
                ViewBag.DepartmentList = new SelectList(depts, "DepartmentID", "DepartmentName");
                ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");
            }
            return View(model);
        }

        [Authorize(Roles = "ERMVWAEMR, XYALLACCZ")]
        public async Task<IActionResult> BirthDayList(int? mm, int? dd)
        {
            BirthDayListViewModel model = new BirthDayListViewModel();
            if ((mm == null || mm < 1) && (dd == null || dd < 1))
            {
                model.mm = DateTime.Now.Month;
                model.dd = DateTime.Now.Day;
            }
            else
            {
                model.mm = mm;
                model.dd = dd;
            }

            model.EmployeesList = await _ermService.GetEmployeesByBirthDayAsync(mm, dd);
            return View(model);
        }

        public async Task<FileResult> DownloadEmployeeRegister(DateTime? td = null, string cd = null, int? ld = null, int? dd = null, int? ud = null)
        {
            List<Employee> employees = new List<Employee>();
            string CompanyCode = cd;
            int LocationID = ld ?? 0;
            int DepartmentID = dd ?? 0;
            int UnitID = ud ?? 0;
            DateTime TerminalDate = td ?? DateTime.Today;

            string fileName = $"Staff Register {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
            try
            {
                if (string.IsNullOrWhiteSpace(CompanyCode))
                {
                    if (LocationID > 0)
                    {
                        if (DepartmentID > 0)
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByLocationAsync(LocationID, DepartmentID, UnitID, TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByLocationAsync(LocationID, DepartmentID, TerminalDate);
                            }
                        }
                        else
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByLocationAndUnitAsync(LocationID, UnitID, TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByLocationAsync(LocationID, TerminalDate);
                            }
                        }
                    }
                    else
                    {
                        if (DepartmentID > 0)
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByUnitIDAsync(UnitID, TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByDepartmentIDAsync(DepartmentID, TerminalDate);
                            }
                        }
                        else
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByUnitIDAsync(UnitID, TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetAllEmployeesAsync(TerminalDate);
                            }
                        }
                    }
                }
                else
                {
                    if (LocationID > 0)
                    {
                        if (DepartmentID > 0)
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(CompanyCode, LocationID, UnitID, TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndLocationAsync(CompanyCode, LocationID, DepartmentID, TerminalDate);
                            }
                        }
                        else
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(CompanyCode, LocationID, UnitID, TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndLocationAsync(CompanyCode, LocationID, TerminalDate);
                            }
                        }
                    }
                    else
                    {
                        if (DepartmentID > 0)
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(CompanyCode, UnitID, TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndDepartmentAsync(CompanyCode, DepartmentID, TerminalDate);
                            }
                        }
                        else
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(CompanyCode, UnitID, TerminalDate);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAsync(CompanyCode, TerminalDate);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return GenerateEmployeeRegisterInExcel(fileName, employees);
        }

        #region Employee Separation Controller Actions

        public async Task<IActionResult> Separation(DateTime? sd = null, DateTime? ed = null)
        {
            EmployeeSeparationViewModel model = new EmployeeSeparationViewModel();
            try
            {
                if (sd == null) { model.sd = DateTime.Today.AddYears(-1); }
                else { model.sd = sd.Value; }
                if (ed == null) { model.ed = DateTime.Today; }
                else { model.ed = ed.Value; }
                var entities = await _ermService.GetEmployeeSeparationsAsync(model.sd, model.ed);
                if (entities != null)
                {
                    model.EmployeeSeparationList = entities;
                    model.RowCount = entities.Count;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> ManageSeparation(int id)
        {
            ManageSeparationViewModel model = new ManageSeparationViewModel();
            try
            {
                if (id > 0)
                {
                    var entity = await _ermService.GetEmployeeSeparationAsync(id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
                else
                {
                    model.ExpectedLastWorkedDate = DateTime.Today;
                    model.ActualLastWorkedDate = DateTime.Today;
                    model.NoticeServedDate = DateTime.Today;
                    model.NoticePeriodInMonths = 0;
                    model.EligibleForRehire = true;
                    model.ReturnedAssignedAssets = true;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }


            var types = await _ermService.GetEmployeeSeparationTypesAsync();
            var reasons = await _ermService.GetEmployeeSeparationReasonsAsync();

            if (types != null) { ViewBag.SeparationTypesList = new SelectList(types, "Id", "Description"); }

            if (reasons != null) { ViewBag.SeparationReasonsList = new SelectList(reasons, "Id", "Description"); }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageSeparation(ManageSeparationViewModel model)
        {
            EmployeeSeparation employeeSeparation = new EmployeeSeparation();
            bool IsSuccessful = false;
            if (ModelState.IsValid)
            {
                try
                {
                    employeeSeparation = model.Convert();
                    if (string.IsNullOrWhiteSpace(employeeSeparation.EmployeeId) || employeeSeparation.UnitId < 1 || employeeSeparation.DepartmentId < 1 || employeeSeparation.LocationId < 1)
                    {
                        Employee employee = await _ermService.GetEmployeeByNameAsync(model.EmployeeName);
                        if (employee != null)
                        {
                            employeeSeparation.EmployeeId = employee.EmployeeID;
                            employeeSeparation.UnitId = employee.UnitID.Value;
                            employeeSeparation.DepartmentId = employee.DepartmentID.Value;
                            employeeSeparation.LocationId = employee.LocationID.Value;
                        }
                    }

                    if (employeeSeparation.EmployeeSeparationId < 1)
                    {
                        employeeSeparation.RecordCreatedBy = HttpContext.User.Identity.Name;
                        employeeSeparation.RecordCreatedDate = DateTime.UtcNow;
                        IsSuccessful = await _ermService.AddEmployeeSeparationAsync(employeeSeparation);
                        if (IsSuccessful)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Employee Separation added successfully!";
                        }
                    }
                    else
                    {
                        employeeSeparation.RecordModifiedBy = HttpContext.User.Identity.Name;
                        employeeSeparation.RecordModifiedDate = DateTime.UtcNow;

                        IsSuccessful = await _ermService.EditEmployeeSeparationAsync(employeeSeparation);
                        if (IsSuccessful)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Employee Separation updated successfully!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            var types = await _ermService.GetEmployeeSeparationTypesAsync();
            var reasons = await _ermService.GetEmployeeSeparationReasonsAsync();
            if (types != null) { ViewBag.SeparationTypesList = new SelectList(types, "Id", "Description"); }
            if (reasons != null) { ViewBag.SeparationReasonsList = new SelectList(reasons, "Id", "Description"); }

            return View(model);
        }

        public async Task<IActionResult> ViewSeparation(int id)
        {
            ManageSeparationViewModel model = new ManageSeparationViewModel();
            try
            {
                if (id > 0)
                {
                    var entity = await _ermService.GetEmployeeSeparationAsync(id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
                else
                {
                    return RedirectToAction("ManageSeparation");
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }


            //var types = await _ermService.GetEmployeeSeparationTypesAsync();
            //var reasons = await _ermService.GetEmployeeSeparationReasonsAsync();

            //if (types != null) { ViewBag.SeparationTypesList = new SelectList(types, "Id", "Description"); }

            //if (reasons != null) { ViewBag.SeparationReasonsList = new SelectList(reasons, "Id", "Description"); }

            return View(model);
        }

        public async Task<IActionResult> DeleteSeparation(int id)
        {
            ManageSeparationViewModel model = new ManageSeparationViewModel();
            try
            {
                if (id > 0)
                {
                    var entity = await _ermService.GetEmployeeSeparationAsync(id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
                else
                {
                    return RedirectToAction("ManageSeparation");
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSeparation(ManageSeparationViewModel model)
        {
            string DeletedBy = HttpContext.User.Identity.Name;

            try
            {
                var entities = await _ermService.GetSeparationOutstandingsAsync(model.EmployeeSeparationId);
                if (entities != null && entities.Count < 1)
                {
                    if (await _ermService.DeleteEmployeeSeparationAsync(model.EmployeeSeparationId, DeletedBy))
                    {
                        return RedirectToAction("Separation");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. Delete operation failed. Please try again.";
                    }
                }
                else
                {
                    model.OperationIsCompleted = true;
                    model.OperationIsSuccessful = true;
                    model.ViewModelErrorMessage = "Invalid Operation! Editing is not permitted on this record because it has pending Outstanding items.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        public async Task<FileResult> DownloadEmployeeSeparationsReport(DateTime? sd = null, DateTime? ed = null)
        {
            List<EmployeeSeparation> employeeSeparationList = new List<EmployeeSeparation>();
            DateTime StartDate = sd ?? DateTime.Today.AddYears(-1);
            DateTime EndDate = ed ?? DateTime.Today;
            string fileName = $"Staff Separation Report {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
            try
            {
                var entities = await _ermService.GetEmployeeSeparationsAsync(StartDate, EndDate);
                if (entities != null)
                {
                    employeeSeparationList = entities;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return GenerateEmployeeSeparationReportInExcel(fileName, employeeSeparationList);
        }

        #endregion

        #region Employee Separation Outstanding Controller Actions

        public async Task<IActionResult> SeparationOutstanding(int id, string en = "")
        {
            SeparationOutstandingListViewModel model = new SeparationOutstandingListViewModel();
            model.id = id;
            model.EmployeeName = en;
            try
            {
                if (id > 0)
                {
                    var entities = await _ermService.GetSeparationOutstandingsAsync(model.id);
                    if (entities != null)
                    {
                        model.SeparationOutstandingList = entities;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ManageSeparationOutstanding(int id = 0, int sd = 0)
        {
            ManageSeparationOutstandingViewModel model = new ManageSeparationOutstandingViewModel();
            model.EmployeeSeparationId = sd;
            model.Id = id;
            try
            {
                if (id > 0)
                {
                    var entity = await _ermService.GetSeparationOutstandingAsync(id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
                else
                {
                    if (sd > 0)
                    {
                        var entity = await _ermService.GetEmployeeSeparationAsync(sd);
                        if (entity != null)
                        {
                            model.EmployeeSeparationId = entity.EmployeeSeparationId;
                            model.EmployeeId = entity.EmployeeId;
                            model.EmployeeName = entity.EmployeeName;
                            model.Currency = "NGN";
                        }
                    }
                }
                var items = await _ermService.GetSeparationOutstandingItemsAsync();
                var currencies = await _globalSettingsService.GetCurrenciesAsync();

                if (items != null) { ViewBag.ItemsList = new SelectList(items, "Description", "Description"); }
                if (currencies != null) { ViewBag.CurrenciesList = new SelectList(currencies, "Code", "Name"); }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageSeparationOutstanding(ManageSeparationOutstandingViewModel model)
        {
            EmployeeSeparationOutstanding e = new EmployeeSeparationOutstanding();
            bool IsSuccessful = false;
            if (ModelState.IsValid)
            {
                try
                {
                    e = model.Convert();
                    e.AmountFormatted = $"{model.Currency} {model.Amount}";
                    e.AmountBalance = model.Amount;
                    if (model.Id > 0)
                    {
                        var payments = await _ermService.GetSeparationPaymentsBySeparationOutstandingIdAsync(model.Id);
                        if (payments != null && payments.Count > 0)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelErrorMessage = "Invalid Operation! Editing is not permitted on this record because it will affect the balance of payments that have already been made on it. ";
                        }
                        else
                        {
                            IsSuccessful = await _ermService.UpdateEmployeeSeparationOutstandingAsync(e);
                            if (IsSuccessful)
                            {
                                model.OperationIsCompleted = true;
                                model.OperationIsSuccessful = true;
                                model.ViewModelSuccessMessage = "Employee Separation Outstanding updated successfully!";
                            }
                            else
                            {
                                model.OperationIsCompleted = true;
                                model.OperationIsSuccessful = true;
                                model.ViewModelErrorMessage = "Sorry update operation failed. Please try again.";
                            }
                        }
                    }
                    else
                    {
                        IsSuccessful = await _ermService.AddEmployeeSeparationOutstandingAsync(e);
                        if (IsSuccessful)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Employee Separation Outstanding added successfully!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            var items = await _ermService.GetSeparationOutstandingItemsAsync();
            var currencies = await _globalSettingsService.GetCurrenciesAsync();

            if (items != null) { ViewBag.ItemsList = new SelectList(items, "Description", "Description"); }
            if (currencies != null) { ViewBag.CurrenciesList = new SelectList(currencies, "Code", "Name"); }

            return View(model);
        }

        public async Task<IActionResult> ViewSeparationOutstanding(int id)
        {
            ManageSeparationOutstandingViewModel model = new ManageSeparationOutstandingViewModel();
            model.Id = id;
            try
            {
                if (id > 0)
                {
                    var entity = await _ermService.GetSeparationOutstandingAsync(id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteSeparationOutstanding(int id)
        {
            ManageSeparationOutstandingViewModel model = new ManageSeparationOutstandingViewModel();
            model.Id = id;
            try
            {
                if (id > 0)
                {
                    var entity = await _ermService.GetSeparationOutstandingAsync(id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSeparationOutstanding(ManageSeparationOutstandingViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var payments = await _ermService.GetSeparationPaymentsBySeparationOutstandingIdAsync(model.Id);
                    if (payments != null && payments.Count > 0)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelErrorMessage = "Invalid Operation! Delete operation is not permitted on this record because it will affect the balance of payments that have already been made on it. ";
                    }
                    else
                    {
                        if (await _ermService.DeleteEmployeeSeparationOutstandingAsync(model.Id))
                        {
                            return RedirectToAction("SeparationOutstanding", new { id = model.EmployeeSeparationId, en = model.EmployeeName });
                        }
                        else
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelErrorMessage = "Sorry delete operation failed. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        #endregion

        #region Employee Separation Payments Actions

        public async Task<IActionResult> SeparationOutstandingPayments(int id, string ed, string en)
        {
            EmployeeSeparationPaymentsListViewModel model = new EmployeeSeparationPaymentsListViewModel();
            model.EmployeeId = ed;
            model.EmployeeName = en;
            model.EmployeeSeparationId = id;

            try
            {
                if (model.EmployeeSeparationId > 0)
                {
                    var entities = await _ermService.GetSeparationPaymentsAsync(model.EmployeeSeparationId);
                    if (entities != null)
                    {
                        model.EmployeeSeparationPayments = entities;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(model.EmployeeId))
                {
                    var entities = await _ermService.GetSeparationPaymentsAsync(model.EmployeeId);
                    if (entities != null)
                    {
                        model.EmployeeSeparationPayments = entities;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ManageSeparationPayment(int id = 0, int od = 0)
        {
            ManageSeparationPaymentViewModel model = new ManageSeparationPaymentViewModel();
            model.OutstandingId = od;
            model.Id = id;
            try
            {
                if (model.Id > 0)
                {
                    var entity = await _ermService.GetSeparationPaymentAsync(model.Id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
                else
                {
                    if (model.OutstandingId > 0)
                    {
                        var entity = await _ermService.GetSeparationOutstandingAsync(model.OutstandingId);
                        if (entity != null)
                        {
                            model.EmployeeSeparationId = entity.EmployeeSeparationId;
                            model.EmployeeId = entity.EmployeeId;
                            model.EmployeeName = entity.EmployeeName;
                            model.ItemDescription = entity.ItemDescription;
                            model.ItemTypeDescription = entity.TypeDescription;
                            model.Currency = entity.Currency;
                            model.PaymentDate = DateTime.Today;
                            model.ItemDescriptionFormatted = $"{entity.ItemDescription} ({entity.TypeDescription})";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var currencies = await _globalSettingsService.GetCurrenciesAsync();
            if (currencies != null) { ViewBag.CurrenciesList = new SelectList(currencies, "Code", "Name", model.Currency); }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageSeparationPayment(ManageSeparationPaymentViewModel model)
        {
            EmployeeSeparationPayments e = new EmployeeSeparationPayments();
            bool IsSuccessful = false;
            if (ModelState.IsValid)
            {
                try
                {
                    e = model.Convert();
                    e.EnteredBy = HttpContext.User.Identity.Name;
                    e.EnteredDate = DateTime.UtcNow;

                    if (e.Id > 0)
                    {
                        IsSuccessful = await _ermService.UpdateEmployeeSeparationPaymentAsync(e);
                        if (IsSuccessful)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Employee Separation Payment updated successfully!";
                        }
                    }
                    else
                    {
                        IsSuccessful = await _ermService.AddEmployeeSeparationPaymentAsync(e);
                        if (IsSuccessful)
                        {
                            model.OperationIsCompleted = true;
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "New Employee Separation Payment added successfully!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            var currencies = await _globalSettingsService.GetCurrenciesAsync();
            if (currencies != null) { ViewBag.CurrenciesList = new SelectList(currencies, "Code", "Name"); }

            return View(model);
        }

        public async Task<IActionResult> ViewSeparationPayment(int id)
        {
            ManageSeparationPaymentViewModel model = new ManageSeparationPaymentViewModel();
            model.Id = id;
            try
            {
                if (id > 0)
                {
                    var entity = await _ermService.GetSeparationPaymentAsync(id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteSeparationPayment(int id)
        {
            ManageSeparationPaymentViewModel model = new ManageSeparationPaymentViewModel();
            model.Id = id;
            try
            {
                if (id > 0)
                {
                    var entity = await _ermService.GetSeparationPaymentAsync(id);
                    if (entity != null && !string.IsNullOrEmpty(entity.EmployeeName))
                    {
                        model = model.Extract(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSeparationPayment(ManageSeparationPaymentViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    if (await _ermService.DeleteEmployeeSeparationPaymentAsync(model.Id))
                    {
                        return RedirectToAction("SeparationOutstandingPayments", new { id = model.EmployeeSeparationId, ed = model.EmployeeId, en = model.EmployeeName });
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        #endregion

        //======== Employees Helper Methods =======//
        #region Employees Helper Methods

        [HttpGet]
        public JsonResult GetEmployeeNames(string text)
        {
            List<string> employees = _ermService.SearchEmployeesByNameAsync(text).Result.Select(x => x.FullName).ToList();
            return Json(employees);
        }

        [HttpGet]
        public JsonResult GetEmployeeParameters(string nm)
        {
            Employee employee = new Employee();

            employee = _ermService.GetEmployeeByNameAsync(nm).Result;

            if (employee == null)
            {
                employee = new Employee
                {
                    EmployeeID = string.Empty,
                    //AssetTypeID = -1
                };
            }

            string model = JsonConvert.SerializeObject(new
            {
                emp_id = employee.EmployeeID,
                emp_name = employee.FullName,
                unit_id = employee.UnitID,
                dept_id = employee.DepartmentID,
                station_id = employee.LocationID,
            }, Formatting.Indented);

            return Json(model);
        }

        [HttpGet]
        public JsonResult GetOtherEmployeeNames(string text, string emp)
        {
            List<string> employees = _ermService.SearchOtherEmployeesByNameAsync(emp, text).Result.Select(x => x.FullName).ToList();
            return Json(employees);
        }

        [HttpGet]
        public JsonResult GetPersonNames(string text)
        {
            List<string> persons = _baseModelService.SearchNonEmployeePersonsByName(text).Result.Select(x => x.FullName).ToList();
            return Json(persons);
        }

        public JsonResult GetExpectedLastWorkDate(string nd, int np)
        {
            ResultObject returnObj = new ResultObject();
            DateTime expectedLastWorkDate = DateTime.Today.Date;
            string errorMessage = string.Empty;
            try
            {
                if (nd != null)
                {
                    DateTime convertedDate = Convert.ToDateTime(nd);
                    expectedLastWorkDate = convertedDate.AddMonths(np);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            returnObj.errorMessage = errorMessage;
            returnObj.result = expectedLastWorkDate.ToString("yyyy-MM-dd");
            var jsonObj = System.Text.Json.JsonSerializer.Serialize(returnObj);
            return Json(jsonObj);
        }

        public JsonResult GetOutstandingWorkDays(string xd, string ad)
        {
            DateTime expectedLastWorkDate = DateTime.Today.Date;
            TimeSpan daysOutstanding = new TimeSpan();
            string errorMessage = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(xd) && !string.IsNullOrWhiteSpace(ad))
                {
                    DateTime convertedExpectedLastWorkDate = Convert.ToDateTime(xd);
                    DateTime convertedActualLastWorkDate = Convert.ToDateTime(ad);
                    daysOutstanding = convertedExpectedLastWorkDate - convertedActualLastWorkDate;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            ResultObject returnObj = new ResultObject { errorMessage = errorMessage, result = daysOutstanding.Days.ToString() };
            var jsonObj = System.Text.Json.JsonSerializer.Serialize(returnObj);
            return Json(jsonObj);
        }

        private FileResult GenerateEmployeeRegisterInExcel(string fileName, IEnumerable<Employee> employees)
        {
            DataTable dataTable = new DataTable("employees");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("#"),
                new DataColumn("Employee No"),
                new DataColumn("Full Name"),
                new DataColumn("Sex"),
                new DataColumn("Email"),
                new DataColumn("Phone"),
                new DataColumn("Alt. Phone"),
                new DataColumn("Designation"),
                new DataColumn("Unit"),
                new DataColumn("Department"),
                new DataColumn("Location"),
                new DataColumn("Type"),
            });

            int rowCount = 0;
            foreach (var employee in employees)
            {
                rowCount++;
                dataTable.Rows.Add(
                    rowCount.ToString(),
                    employee.EmployeeNo1,
                    employee.FullName,
                    employee.Sex,
                    employee.OfficialEmail,
                    employee.PhoneNo1,
                    employee.PhoneNo2,
                    employee.CurrentDesignation,
                    employee.UnitName,
                    employee.DepartmentName,
                    employee.LocationName,
                    employee.EmploymentStatus
                    );
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        private FileResult GenerateEmployeeSeparationReportInExcel(string fileName, IEnumerable<EmployeeSeparation> employeeSeparations)
        {
            DataTable dataTable = new DataTable("Separations");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("#"),
                new DataColumn("Full Name"),
                new DataColumn("Last Work Date"),
                new DataColumn("Unit"),
                new DataColumn("Department"),
                new DataColumn("Location"),
                new DataColumn("Type"),
                new DataColumn("Reason"),
                new DataColumn("Details"),
                new DataColumn("Is Indebted"),
                new DataColumn("Is Owed"),
            });

            int rowCount = 0;
            foreach (var e in employeeSeparations)
            {
                rowCount++;
                dataTable.Rows.Add(
                    rowCount.ToString(),
                    e.EmployeeName,
                    e.ActualLastWorkedDate.Value.ToString("yyyy-MM-dd"),
                    e.UnitName,
                    e.DepartmentName,
                    e.LocationName,
                    e.SeparationTypeDescription,
                    e.SeparationReasonDescription,
                    e.SeparationReasonExplanation,
                    e.IsIndebted,
                    e.IsOwed
                    );
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


        #endregion
    }
}