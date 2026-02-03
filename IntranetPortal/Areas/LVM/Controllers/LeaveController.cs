using IntranetPortal.Areas.LVM.Models;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Controllers
{
    [Area("LVM")]
    public class LeaveController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IBaseModelService _baseModelService;
        private readonly ILeaveService _leaveService;
        private readonly IErmService _ermService;

        public LeaveController(IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration, IBaseModelService baseModelService,
            ILeaveService leaveService, IErmService ermService)
        {
            _configuration = configuration;
            _baseModelService = baseModelService;
            _leaveService = leaveService;
            _ermService = ermService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MyLeavePlans(int yr)
        {
            MyLeavePlansListViewModel model = new MyLeavePlansListViewModel();
            if (yr < 2020)
            {
                model.yr = DateTime.Now.Year;
            }
            else { model.yr = yr; }

            model.nm = HttpContext.User.Identity.Name;
            model.ei = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            try
            {
                model.LeavePlanList = await _leaveService.GetLeavePlansAsync(model.ei, model.yr);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        //public async Task<IActionResult> NewLeavePlan()
        //{
        //    LeavePlanViewModel model = new LeavePlanViewModel();
        //    model.LeaveStartDate = DateTime.Today;
        //    model.LeaveEndDate = null;
        //    model.LeaveYear = DateTime.Today.Year;
        //    model.LeaveStatus = LeaveStatus.Draft.ToString();
        //    model.IsPlan = true;
        //    //model.EmployeeFullName = HttpContext.User.Identity.Name;
        //    model.EmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
        //    Employee e = await _ermService.GetEmployeeByIdAsync(model.EmployeeId);
        //    if (e == null || string.IsNullOrWhiteSpace(e.FullName)) { throw new Exception("Sorry, no record was found for this staff."); }
        //    else
        //    {
        //        model.EmployeeId = e.EmployeeID;
        //        model.EmployeeFullName = e.FullName;
        //        model.DepartmentId = e.DepartmentID ?? 0;
        //        model.UnitId = e.UnitID ?? 0;
        //        model.LocationId = e.LocationID ?? 0;
        //    }
        //    List<LeaveType> entities = await _lmsService.GetLeaveTypes();
        //    if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name"); }
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> NewLeavePlan(LeavePlanViewModel model)
        //{
        //    try
        //    {
        //        EmployeeLeave d = new EmployeeLeave();
        //        if (ModelState.IsValid)
        //        {
        //            d = model.Convert();
        //            long LeaveId = await _lmsService.CreateLeaveAsync(d);
        //            if (LeaveId > 0)
        //            {
        //                return RedirectToAction("MyLeavePlans", new { yr = model.LeaveStartDate.Year });
        //            }
        //            else { throw new Exception("An error was encountered. New Leave Plan could not be added."); }
        //        }
        //        else { throw new Exception("Sorry, some key form parameters are missing."); }
        //    }
        //    catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
        //    List<LeaveType> entities = await _lmsService.GetLeaveTypes();
        //    if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
        //    return View(model);
        //}

    }
}
