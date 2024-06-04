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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
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


        public async Task<FileResult> DownloadEmployeeRegister(string cd, int? ld = null, int? dd = null, int? ud = null)
        {
            List<Employee> employees = new List<Employee>();
            string CompanyCode = cd;
            int LocationID = ld ?? 0;
            int DepartmentID = dd ?? 0;
            int UnitID = ud ?? 0;

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
                                employees = await _ermService.GetEmployeesByLocationAsync(LocationID, DepartmentID, UnitID);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByLocationAsync(LocationID, DepartmentID);
                            }
                        }
                        else
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByLocationAndUnitAsync(LocationID, UnitID);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByLocationAsync(LocationID);
                            }
                        }
                    }
                    else
                    {
                        if (DepartmentID > 0)
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByUnitIDAsync(UnitID);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByDepartmentIDAsync(DepartmentID);
                            }
                        }
                        else
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByUnitIDAsync(UnitID);
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
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(CompanyCode, LocationID, UnitID);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndLocationAsync(CompanyCode, LocationID, DepartmentID);
                            }
                        }
                        else
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(CompanyCode, LocationID, UnitID);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndLocationAsync(CompanyCode, LocationID);
                            }
                        }
                    }
                    else
                    {
                        if (DepartmentID > 0)
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(CompanyCode, UnitID);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndDepartmentAsync(CompanyCode, DepartmentID);
                            }
                        }
                        else
                        {
                            if (UnitID > 0)
                            {
                                employees = await _ermService.GetEmployeesByCompanyAndUnitAsync(CompanyCode, UnitID);
                            }
                            else
                            {
                                employees = await _ermService.GetEmployeesByCompanyAsync(CompanyCode);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
                //TempData["ErrorMessage"] = ex.Message;
                //return RedirectToAction("ResultReport", new { id, lc, dc, uc });
            }
            return GenerateEmployeeRegisterInExcel(fileName, employees);
        }




        //======================== Employees Helper Methods ======================================//
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

        #endregion
    }
}