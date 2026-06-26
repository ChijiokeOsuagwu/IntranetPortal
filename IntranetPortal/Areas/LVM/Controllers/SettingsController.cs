using IntranetPortal.Areas.LVM.Models;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Base.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Controllers
{
    [Area("LVM")]
    public class SettingsController : Controller
    {
        private readonly ILeaveService _leaveService;
        private readonly IErmService _ermService;
        public SettingsController(ILeaveService leaveService, IErmService ermService)
        {
            _leaveService = leaveService;
            _ermService = ermService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Leave Types Controller Action Methods
        public async Task<IActionResult> LeaveTypes()
        {
            LeaveTypesListViewModel model = new LeaveTypesListViewModel();
            try { model.LeaveTypesList = await _leaveService.GetLeaveTypes(); }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public IActionResult NewLeaveType()
        {
            LeaveTypeViewModel model = new LeaveTypeViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                LeaveType e = new LeaveType();
                if (ModelState.IsValid)
                {
                    e.Code = model.Code;
                    e.Name = model.Name;
                    e.Description = model.Description;
                    if (await _leaveService.CreateLeaveType(e))
                    {
                        return RedirectToAction("LeaveTypes");
                    }
                    else { throw new Exception("An error was encountered. New Leave Type was not created."); }

                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> EditLeaveType(string id)
        {
            LeaveTypeViewModel model = new LeaveTypeViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var leaveType = await _leaveService.GetLeaveType(id);
                    if (leaveType != null)
                    {
                        model.Code = leaveType.Code;
                        model.Name = leaveType.Name;
                        model.Description = leaveType.Description;
                    }
                    else { throw new Exception("No record was found for the selected item."); }
                }
                else { throw new Exception("Required parameter Leave Type ID cannot be null."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                LeaveType e = new LeaveType();
                if (ModelState.IsValid)
                {
                    e.Code = model.Code;
                    e.Name = model.Name;
                    e.Description = model.Description;
                    if (await _leaveService.UpdateLeaveType(e))
                    {
                        return RedirectToAction("LeaveTypes");
                    }
                    else { throw new Exception("An error was encountered. Leave Type was not updated."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> DeleteLeaveType(string id)
        {
            LeaveTypeViewModel model = new LeaveTypeViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var leaveType = await _leaveService.GetLeaveType(id);
                    if (leaveType != null)
                    {
                        model.Code = leaveType.Code;
                        model.Name = leaveType.Name;
                        model.Description = leaveType.Description;
                    }
                    else { throw new Exception("No record was found for the selected item."); }
                }
                else { throw new Exception("Required parameter Leave Type ID cannot be null."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Code))
                {
                    if (await _leaveService.DeleteLeaveType(model.Code))
                    {
                        return RedirectToAction("LeaveTypes");
                    }
                    else { throw new Exception("An error was encountered. Leave Type was not deleted."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        #endregion

        #region Public Holidays Controller Action Methods
        public async Task<IActionResult> Holidays(int yr)
        {
            HolidaysListViewModel model = new HolidaysListViewModel();
            if (yr < 1) { yr = DateTime.Now.Year; }
            model.yr = yr;
            try
            {
                model.HolidayList = await _leaveService.GetPublicHolidays(yr);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public IActionResult NewHoliday()
        {
            HolidayViewModel model = new HolidayViewModel();
            model.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            model.EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            model.HolidayYear = DateTime.Now.Year;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewHoliday(HolidayViewModel model)
        {
            try
            {
                PublicHoliday e = new PublicHoliday();
                if (ModelState.IsValid)
                {
                    e.Id = model.Id;
                    e.Name = model.Name;
                    e.Type = model.Type;
                    e.StartDate = model.StartDate;
                    e.EndDate = model.EndDate;
                    e.HolidayYear = model.StartDate.Year;
                    e.Reason = model.Reason;
                    if (model.EndDate == model.StartDate) { e.NoOfDays = 1; }
                    else { e.NoOfDays = Convert.ToInt32((model.EndDate - model.StartDate).TotalDays); }
                    if (await _leaveService.CreatePublicHoliday(e))
                    {
                        return RedirectToAction("Holidays");
                    }
                    else { throw new Exception("An error was encountered. New Holiday was not created."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> EditHoliday(int id)
        {
            HolidayViewModel model = new HolidayViewModel();
            var entity = await _leaveService.GetPublicHoliday(id);
            if (entity != null)
            {
                model.EndDate = entity.EndDate;
                model.HolidayYear = entity.HolidayYear;
                model.Id = entity.Id;
                model.Name = entity.Name;
                model.NoOfDays = entity.NoOfDays;
                model.Reason = entity.Reason;
                model.StartDate = entity.StartDate;
                model.Type = entity.Type;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditHoliday(HolidayViewModel model)
        {
            try
            {
                PublicHoliday e = new PublicHoliday();
                if (ModelState.IsValid)
                {
                    e.Id = model.Id;
                    e.Name = model.Name;
                    e.Type = model.Type;
                    e.StartDate = model.StartDate;
                    e.EndDate = model.EndDate;
                    e.HolidayYear = model.StartDate.Year;
                    e.Reason = model.Reason;
                    if (model.EndDate < model.StartDate) { throw new Exception("Error: Invalid dates!"); }
                    if (model.EndDate == model.StartDate) { e.NoOfDays = 1; }
                    else { e.NoOfDays = Convert.ToInt32((model.EndDate - model.StartDate).TotalDays); }
                    if (await _leaveService.UpdatePublicHoliday(e))
                    {
                        return RedirectToAction("Holidays");
                    }
                    else { throw new Exception("An error was encountered. Holiday was not updated."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> DeleteHoliday(int id)
        {
            HolidayViewModel model = new HolidayViewModel();
            var entity = await _leaveService.GetPublicHoliday(id);
            if (entity != null)
            {
                model.EndDate = entity.EndDate;
                model.HolidayYear = entity.HolidayYear;
                model.Id = entity.Id;
                model.Name = entity.Name;
                model.NoOfDays = entity.NoOfDays;
                model.Reason = entity.Reason;
                model.StartDate = entity.StartDate;
                model.Type = entity.Type;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHoliday(HolidayViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    if (await _leaveService.DeletePublicHoliday(model.Id))
                    {
                        return RedirectToAction("Holidays");
                    }
                    else { throw new Exception("Sorry, an error was encountered. Holiday was not deleted."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        #endregion

        #region Leave Profiles Controller Action Methods
        public async Task<IActionResult> LeaveProfiles()
        {
            LeaveProfilesListViewModel model = new LeaveProfilesListViewModel();
            try { model.LeaveProfilesList = await _leaveService.GetLeaveProfiles(); }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        
        public IActionResult NewLeaveProfile()
        {
            LeaveProfileViewModel model = new LeaveProfileViewModel();
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> NewLeaveProfile(LeaveProfileViewModel model)
        {
            try
            {
                LeaveProfile e = new LeaveProfile();
                if (ModelState.IsValid)
                {
                    e.Name = model.Name;
                    e.Description = model.Description;
                    e.Code = model.Code;
                    if (await _leaveService.CreateLeaveProfile(e))
                    {
                        return RedirectToAction("LeaveProfiles");
                    }
                    else { throw new Exception("An error was encountered. New Leave Profile was not created."); }

                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        
        public async Task<IActionResult> EditLeaveProfile(string cd)
        {
            LeaveProfileViewModel model = new LeaveProfileViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cd))
                {
                    var leaveProfile = await _leaveService.GetLeaveProfileByCode(cd);
                    if (leaveProfile != null)
                    {
                        model.Name = leaveProfile.Name;
                        model.Description = leaveProfile.Description;
                        model.Code = leaveProfile.Code;
                    }
                    else { throw new Exception("No record was found for the selected item."); }
                }
                else { throw new Exception("Required parameter Leave Profile ID cannot be null."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditLeaveProfile(LeaveProfileViewModel model)
        {
            try
            {
                LeaveProfile e = new LeaveProfile();
                if (ModelState.IsValid)
                {
                    e.Name = model.Name;
                    e.Description = model.Description;
                    e.Code = model.Code;
                    if (await _leaveService.UpdateLeaveProfile(e))
                    {
                        return RedirectToAction("LeaveProfiles");
                    }
                    else { throw new Exception("Sorry, an error was encountered. Leave Profile was not updated."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        
        public async Task<IActionResult> DeleteLeaveProfile(string cd)
        {
            LeaveProfileViewModel model = new LeaveProfileViewModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(cd))
                {
                    var leaveProfile = await _leaveService.GetLeaveProfileByCode(cd);
                    if (leaveProfile != null)
                    {
                        model.Code = leaveProfile.Code;
                        model.Name = leaveProfile.Name;
                        model.Description = leaveProfile.Description;
                    }
                    else { throw new Exception("No record was found for the selected item."); }
                }
                else { throw new Exception("Required parameter Leave Profile ID cannot be null."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteLeaveProfile(LeaveProfileViewModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Code))
                {
                    if (await _leaveService.DeleteLeaveProfile(model.Code))
                    {
                        return RedirectToAction("LeaveProfiles");
                    }
                    else { throw new Exception("Sorry, an error was encountered. Leave Profile was not deleted."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        
        public async Task<IActionResult> LeaveProfileEmployees(string cd)
        {
            LeaveProfileEmployeesListViewModel model = new LeaveProfileEmployeesListViewModel();
            model.LeaveProfileCode = cd;
            try
            {
                if (!string.IsNullOrWhiteSpace(model.LeaveProfileCode))
                {
                    model.EmployeeRollsList = await _ermService.GetEmployeeRollsByLeaveProfileCodeAsync(model.LeaveProfileCode);
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        #endregion

        #region Leave Profile Details Controller Action Methods
        public async Task<IActionResult> LeaveProfileDetails(string cd, string nm)
        {
            LeaveProfileDetailsListViewModel model = new LeaveProfileDetailsListViewModel();
            model.LeaveProfileCode = cd;
            model.LeaveProfileName = nm;
            try { model.LeaveProfileDetailList = await _leaveService.GetLeaveProfileDetails(model.LeaveProfileCode); }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        
        public async Task<IActionResult> NewProfileDetail(string cd, string nm)
        {
            LeaveProfileDetailViewModel model = new LeaveProfileDetailViewModel();
            model.ProfileCode = cd;
            model.ProfileName = nm;
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name"); }
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> NewProfileDetail(LeaveProfileDetailViewModel model)
        {
            try
            {
                LeaveProfileDetail d = new LeaveProfileDetail();
                if (ModelState.IsValid)
                {
                    d.Id = model.Id;
                    d.ProfileCode = model.ProfileCode;
                    d.IsYearly = model.IsYearly;
                    d.LeaveTypeCode = model.LeaveTypeCode;
                    d.Duration = model.Duration;
                    d.DurationTypeId = model.DurationTypeId;
                    d.CanBeCarriedOver = model.CanBeCarriedOver;
                    d.CanBeMonetized = model.CanBeMonetized;
                    d.CarryOverEndMonth = model.CarryOverEndMonth;

                    if (await _leaveService.CreateLeaveProfileDetail(d))
                    {
                        return RedirectToAction("LeaveProfileDetails", new { cd = model.ProfileCode, nm = model.ProfileName });
                    }
                    else { throw new Exception("An error was encountered. New Profile option could not be added."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }
        
        public async Task<IActionResult> EditProfileDetail(int id)
        {
            LeaveProfileDetailViewModel model = new LeaveProfileDetailViewModel();
            if (id > 0)
            {
                var entity = await _leaveService.GetLeaveProfileDetail(id);
                if (entity != null) { throw new Exception("Sorry, the system could not find this record."); }

                model.Id = entity.Id;
                model.CanBeCarriedOver = entity.CanBeCarriedOver;
                model.CanBeMonetized = entity.CanBeMonetized;
                model.Duration = entity.Duration;
                model.DurationTypeId = entity.DurationTypeId;
                model.IsYearly = entity.IsYearly;
                model.LeaveTypeCode = entity.LeaveTypeCode;
                model.ProfileCode = entity.ProfileCode;
                model.ProfileName = entity.ProfileName;
                model.CarryOverEndMonth = entity.CarryOverEndMonth;
            }
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name"); }
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditProfileDetail(LeaveProfileDetailViewModel model)
        {
            try
            {
                LeaveProfileDetail d = new LeaveProfileDetail();
                if (ModelState.IsValid)
                {
                    d.Id = model.Id;
                    d.ProfileCode = model.ProfileCode;
                    d.IsYearly = model.IsYearly;
                    d.LeaveTypeCode = model.LeaveTypeCode;
                    d.Duration = model.Duration;
                    d.DurationTypeId = model.DurationTypeId;
                    d.CanBeMonetized = model.CanBeMonetized;
                    d.CanBeCarriedOver = model.CanBeCarriedOver;
                    d.CarryOverEndMonth = model.CarryOverEndMonth;

                    if (await _leaveService.UpdateLeaveProfileDetail(d))
                    {
                        return RedirectToAction("LeaveProfileDetails", new { id = model.ProfileCode, nm = model.ProfileName });
                    }
                    else { throw new Exception("An error was encountered. Profile option could not be added."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }
        
        public async Task<IActionResult> DeleteProfileDetail(int id)
        {
            LeaveProfileDetailViewModel model = new LeaveProfileDetailViewModel();
            if (id > 0)
            {
                var entity = await _leaveService.GetLeaveProfileDetail(id);
                if (entity != null) { throw new Exception("Sorry, the system could not find this record."); }
                model.Id = entity.Id;
                model.CanBeCarriedOver = entity.CanBeCarriedOver;
                model.CanBeMonetized = entity.CanBeMonetized;
                model.CarryOverEndMonthName = entity.CarryOverEndMonthName;
                model.Duration = entity.Duration;
                model.DurationTypeId = entity.DurationTypeId;
                model.DurationTypeDescription = entity.DurationTypeDescription;
                model.IsYearly = entity.IsYearly;
                model.LeaveTypeCode = entity.LeaveTypeCode;
                model.LeaveTypeName = entity.LeaveTypeName;
                model.ProfileCode = entity.ProfileCode;
                model.ProfileName = entity.ProfileName;
            }
            return View(model);
        }
        [HttpPost]
        
        public async Task<IActionResult> DeleteProfileDetail(LeaveProfileDetailViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {

                    if (await _leaveService.DeleteLeaveProfileDetail(model.Id))
                    {
                        return RedirectToAction("LeaveProfileDetails", new { cd = model.ProfileCode, nm = model.ProfileName });
                    }
                    else { throw new Exception("An error was encountered. Profile option could not be deleted."); }
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        
        public async Task<IActionResult> ViewProfileDetail(int id)
        {
            LeaveProfileDetailViewModel model = new LeaveProfileDetailViewModel();
            if (id > 0)
            {
                var entity = await _leaveService.GetLeaveProfileDetail(id);
                if (entity != null) { throw new Exception("Sorry, the system could not find this record."); }
                model.Id = entity.Id;
                model.CanBeCarriedOver = entity.CanBeCarriedOver;
                model.CanBeMonetized = entity.CanBeMonetized;
                model.CarryOverEndMonthName = entity.CarryOverEndMonthName;
                model.Duration = entity.Duration;
                model.DurationTypeId = entity.DurationTypeId;
                model.DurationTypeDescription = entity.DurationTypeDescription;
                model.IsYearly = entity.IsYearly;
                model.LeaveTypeCode = entity.LeaveTypeCode;
                model.LeaveTypeName = entity.LeaveTypeName;
                model.ProfileCode = entity.ProfileCode;
                model.ProfileName = entity.ProfileName;
            }
            return View(model);
        }

        #endregion

    }
}
