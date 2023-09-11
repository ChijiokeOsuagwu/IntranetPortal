using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if((mm == null || mm < 1) && (dd == null || dd < 1)) 
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
            List<string> employees = _ermService.SearchOtherEmployeesByNameAsync(emp,text).Result.Select(x => x.FullName).ToList();
            return Json(employees);
        }


        [HttpGet]
        public JsonResult GetPersonNames(string text)
        {
            List<string> persons = _baseModelService.SearchPersonsByName(text).Result.Select(x => x.FullName).ToList();
            return Json(persons);
        }

        #endregion
    }
}