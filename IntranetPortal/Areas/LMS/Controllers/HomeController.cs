using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.LMS.Models;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IntranetPortal.Areas.LMS.Controllers
{
    [Area("LMS")]
    public class HomeController : Controller
    {
        private readonly ILmsService _lmsService;
        private readonly IErmService _ermService;

        public HomeController(ILmsService lmsService, IErmService ermService)
        {
            _lmsService = lmsService;
            _ermService = ermService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Leave Plan Controller Action Methods
        public async Task<IActionResult> MyLeavePlans(int yr)
        {
            LeavePlanListViewModel model = new LeavePlanListViewModel();
            if(yr < 1)
            {
                model.yr = DateTime.Now.Year;
            }
            else { model.yr = yr; }
           
            model.nm = HttpContext.User.Identity.Name;
            model.ei = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            try
            {
                model.LeavePlanList = await _lmsService.GetLeavePlans(model.ei, model.yr);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> NewLeavePlan()
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            model.LeaveStartDate = DateTime.Today;
            model.LeaveEndDate = null;
            model.EmployeeFullName = HttpContext.User.Identity.Name;
            model.EmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name"); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewLeavePlan(LeavePlanViewModel model)
        {
            try
            {
                EmployeeLeave d = new EmployeeLeave();
                if (ModelState.IsValid)
                {
                    Employee e = await _ermService.GetEmployeeByIdAsync(model.EmployeeId);
                    if (e == null || string.IsNullOrWhiteSpace(e.FullName)) { throw new Exception("Sorry, no record was found for this staff."); }
                    else
                    {
                        d.Id = model.Id;
                        d.EmployeeId = model.EmployeeId;
                        d.DepartmentId = e.DepartmentID ?? 0;
                        d.UnitId = e.UnitID ?? 0;
                        d.LocationId = e.LocationID ?? 0;
                        d.LeaveTypeCode = model.LeaveTypeCode;
                        d.Duration = model.Duration;
                        d.DurationTypeId = model.DurationTypeId;
                        d.IsPlan = true;
                        d.LeaveStartDate = model.LeaveStartDate;
                        d.LeaveEndDate = model.LeaveEndDate ?? model.LeaveStartDate;
                        d.LeaveReason = model.LeaveReason;
                        d.LeaveStatus = model.LeaveStatus;
                        d.LeaveYear = model.LeaveStartDate.Year;
                        
                        //d.LeaveEndDate = _lmsService.GenerateLeaveEndDate(model.LeaveStartDate, model.DurationType, model.Duration);
                        bool IsCreated = await _lmsService.CreateLeavePlan(d);
                        if(IsCreated)
                        {
                            return RedirectToAction("MyLeavePlans", new { yr = model.LeaveStartDate.Year });
                        }
                        else { throw new Exception("An error was encountered. New Leave Plan could not be added."); }
                    }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        #endregion

        #region Leave Helper Methods
        public JsonResult GetLeaveEndDate(string sd, int dr, int dt)
        {
            ResultObject returnObj = new ResultObject();
            string leaveEndDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
            string errorMessage = string.Empty;
            try
            {
                if (sd != null)
                {
                    DateTime convertedStartDate = Convert.ToDateTime(sd);
                    leaveEndDate = _lmsService.GenerateLeaveEndDate(convertedStartDate,dt,dr).ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            returnObj.errorMessage = errorMessage;
            returnObj.result = leaveEndDate;
            var jsonObj = System.Text.Json.JsonSerializer.Serialize(returnObj);
            return Json(jsonObj);
        }

        #endregion
    }
}