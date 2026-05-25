using IntranetPortal.Areas.LVM.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Helpers;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authentication;
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
    public class LeaveController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IBaseModelService _baseModelService;
        private readonly ILeaveService _leaveService;
        private readonly IErmService _ermService;
        private readonly IGlobalSettingsService _globalSettingsService;

        public LeaveController(IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration, IBaseModelService baseModelService,
            ILeaveService leaveService, IErmService ermService,
            IGlobalSettingsService globalSettingsService)
        {
            _configuration = configuration;
            _baseModelService = baseModelService;
            _leaveService = leaveService;
            _ermService = ermService;
            _webHostEnvironment = webHostEnvironment;
            _globalSettingsService = globalSettingsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> MyLeaveRecords(int yr)
        {
            MyLeaveRecordsViewModel model = new MyLeaveRecordsViewModel();
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
                model.LeaveRequestList = await _leaveService.GetLeaveRequestsAsync(model.ei, model.yr);
                model.CurrentLeaveBalances = await _leaveService.GetLeaveBalancesAsync("ANL", DateTime.Now.Year, model.ei, model.nm);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }


        #region Leave Plans
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
                model.LeaveRequestList = await _leaveService.GetLeaveRequestsAsync(model.ei, model.yr);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        public async Task<IActionResult> NewLeavePlan()
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            model.LeavePlanStatusId = 0;
            model.LeaveYear = DateTime.Today.Year;
            model.LeaveEmployeeName = HttpContext.User.Identity.Name;
            model.LeaveEmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewLeavePlan(LeavePlanViewModel model)
        {
            try
            {
                LeavePlan d = new LeavePlan();
                if (ModelState.IsValid)
                {
                    d = model.Convert();
                    Employee e = await _ermService.GetEmployeeByIdAsync(d.LeaveEmployeeId);
                    if (e == null || string.IsNullOrWhiteSpace(e.FullName)) { throw new Exception("Sorry, no record was found for this staff."); }
                    else
                    {
                        d.LeaveEmployeeId = e.EmployeeID;
                        d.LeaveEmployeeName = e.FullName;
                        d.LeaveDepartmentId = e.DepartmentID ?? 0;
                        d.LeaveUnitId = e.UnitID ?? 0;
                        d.LeaveLocationId = e.LocationID ?? 0;
                    }

                    if (!_validateEndDate(d.LeavePlanStartDate.Value, d.LeavePlanEndDate.Value)) { throw new Exception("Error: Invalid Leave Start Date or End Date."); }
                    if (!_validateResumptionDate(d.LeavePlanResumptionDate.Value, d.LeavePlanEndDate.Value)) { throw new Exception("Error: Invalid Resumption Date."); }

                    long LeaveId = await _leaveService.CreateLeavePlanAsync(d);
                    if (LeaveId > 0)
                    {
                        return RedirectToAction("MyLeaveRecords", new { yr = model.LeavePlanStartDate?.Year });
                    }
                    else { throw new Exception("An error was encountered. New Leave Plan could not be added."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> EditLeavePlan(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            LeavePlan plan = new LeavePlan();
            if (id < 1)
            {
                return RedirectToAction("NewLeavePlan");
            }
            var entity = await _leaveService.GetLeavePlanAsync(id);
            if (entity != null) { plan = entity; }
            model = model.Extract(plan);

            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeavePlan(LeavePlanViewModel model)
        {
            try
            {
                LeavePlan d = new LeavePlan();
                if (ModelState.IsValid)
                {
                    d = model.Convert();

                    if (!_validateEndDate(d.LeavePlanStartDate.Value, d.LeavePlanEndDate.Value)) { throw new Exception("Error: Invalid Leave Start Date or End Date."); }
                    if (!_validateResumptionDate(d.LeavePlanResumptionDate.Value, d.LeavePlanEndDate.Value)) { throw new Exception("Error: Invalid Resumption Date."); }

                    if (await _leaveService.UpdateLeavePlanAsync(d))
                    {
                        return RedirectToAction("MyLeavePlans", new { yr = model.LeavePlanStartDate?.Year });
                    }
                    else { throw new Exception("An error was encountered. Attempt to update Leave Plan was not successful."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> DeleteLeavePlan(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            var leavePlan = await _leaveService.GetLeavePlanAsync(id);
            if (leavePlan != null)
            {
                model = model.Extract(leavePlan);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLeavePlan(LeavePlanViewModel model)
        {
            try
            {
                if (model.LeavePlanId > 0)
                {
                    bool IsDeleted = await _leaveService.DeleteLeavePlanAsync(model.LeavePlanId);
                    if (IsDeleted)
                    {
                        return RedirectToAction("MyLeavePlans", new { yr = model.LeavePlanStartDate.Value.Year });
                    }
                    else { throw new Exception("An error was encountered. Leave Plan could not be deleted."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ViewLeavePlan(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            LeavePlan plan = new LeavePlan();
            if (id < 1)
            {
                return RedirectToAction("NewLeavePlan");
            }
            var entity = await _leaveService.GetLeavePlanAsync(id);
            if (entity != null) { plan = entity; }
            model = model.Extract(plan);

            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> SubmitLeavePlan(long? pd = null, long? rd = null)
        {
            SubmitLeaveViewModel model = new SubmitLeaveViewModel();
            try
            {
                model.LeavePlanId = pd;
                model.LeaveRequestId = rd;
                model.FromEmployeeName = HttpContext.User.Identity.Name;
                string _loggedInEmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                var entities = await _ermService.GetActiveEmployeeReportLinesByEmployeeIdAsync(_loggedInEmployeeId);
                if (entities != null)
                {
                    ViewBag.ReportingLines = new SelectList(entities, "ReportsToEmployeeID", "ReportsToEmployeeName");
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitLeavePlan(SubmitLeaveViewModel model)
        {
            if (ModelState.IsValid)
            {
                LeaveSubmission leaveSubmission = new LeaveSubmission();
                leaveSubmission = model.Convert();
                leaveSubmission.TimeSubmitted = DateTime.Now;
                try
                {
                    bool IsSubmitted = await _leaveService.SubmitLeaveAsync(leaveSubmission);
                    if (IsSubmitted)
                    {
                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByNameAsync(model.FromEmployeeName);
                        Employee approver = new Employee();
                        approver = await _ermService.GetEmployeeByNameAsync(model.ToEmployeeName);

                        //===== Send Notificiation Message to Approver ========//
                        Message message = new Message
                        {
                            MessageID = Guid.NewGuid().ToString(),
                            RecipientID = approver.EmployeeID,
                            RecipientName = approver.FullName,
                            SentBy = sender.FullName
                        };

                        //===== Send Email Notifications to Approver =========//
                        bool approverEmailCopySent = false;
                        UtilityHelper utilityHelper = new UtilityHelper(_configuration);
                        EmailModel recipientEmailCopy = new EmailModel();
                        recipientEmailCopy.RecipientName = approver.FullName;
                        if (!string.IsNullOrWhiteSpace(approver.OfficialEmail))
                        {
                            recipientEmailCopy.RecipientEmail = approver.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.RecipientEmail = approver.Email;
                        }

                        recipientEmailCopy.RecipientEmail = approver.OfficialEmail;
                        recipientEmailCopy.SenderName = sender.FullName;
                        switch (leaveSubmission.Purpose)
                        {
                            case "Approval":
                                recipientEmailCopy.Subject = "Request for Leave Plan Approval";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetLeavePlanApprovalEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetLeavePlanApprovalEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Request for Leave Plan Approval";
                                message.MessageBody = UtilityHelper.GetLeavePlanApprovalMessageContent(sender.FullName);
                                break;
                            case "Notification":
                                recipientEmailCopy.Subject = "Notice of Leave Plan";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetLeavePlanNoticeEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetLeavePlanNoticeEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Notice of Leave Plan";
                                message.MessageBody = UtilityHelper.GetLeavePlanNoticeMessageContent(sender.FullName);
                                break;
                            default:
                                break;
                        }

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                        {
                           // approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
                        }
                        return RedirectToAction("MyLeavePlans", new { yr = DateTime.Now.Year });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted submission failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }
        
        public async Task<IActionResult> LeavePendingApproval(int? yr = null)
        {
            LeavePendingApprovalListViewModel model = new LeavePendingApprovalListViewModel();
            if(yr == null || yr < 2020) { model.yr = DateTime.Now.Year; }
            string userId = string.Empty;
            string userFullName = string.Empty;
            userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }

            model.nm = HttpContext.User.Identity.Name;
            model.ei = userId;

            if (!string.IsNullOrWhiteSpace(model.nm))
            {
                var entities = await _leaveService.GetLeaveSubmissionsByApproverIdAsync(model.nm, model.yr);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveSubmissionList = entities.ToList();
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LeavePlanApproval(long id, long sd)
        {
            LeavePlanApprovalViewModel model = new LeavePlanApprovalViewModel();
            model.LeaveSubmissionId = sd;
            model.LeavePlanId = id;
            
            LeavePlan plan = new LeavePlan();
            if (model.LeavePlanId < 1)
            {
                return RedirectToAction("LeavePendingApproval");
            }
            var entity = await _leaveService.GetLeavePlanAsync(model.LeavePlanId);
            if (entity != null) { plan = entity; }
            model.LeaveEmployeeId = plan.LeaveEmployeeId;
            model.LeaveEmployeeName = plan.LeaveEmployeeName;
            model.LeavePlanDurationDescription = plan.LeavePlanDurationDescription;
            model.LeavePlanEndDate = plan.LeavePlanEndDate;
            model.LeavePlanId = plan.LeavePlanId;
            model.LeavePlanResumptionDate = plan.LeavePlanResumptionDate;
            model.LeavePlanStartDate = plan.LeavePlanStartDate;
            model.LeavePlanStatusDescription = plan.LeavePlanStatusDescription;
            model.LeavePlanStatusId = plan.LeavePlanStatusId;
            model.LeaveTypeName = plan.LeaveTypeName;
            model.LeaveTypeCode = plan.LeaveTypeCode;
            model.LeaveYear = plan.LeaveYear;
            model.LeavePlanDuration = plan.LeavePlanDuration;
            model.LeavePlanDurationTypeId = plan.LeavePlanDurationTypeId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LeavePlanApproval(LeavePlanApprovalViewModel model)
        {
            if (ModelState.IsValid)
            {
                LeaveSubmission leaveSubmission = new LeaveSubmission();
                LeaveApproval leaveApproval = new LeaveApproval();
                
                try
                {
                    leaveSubmission = await _leaveService.GetLeaveSubmissionByIdAsync(model.LeaveSubmissionId);
                    if(leaveSubmission == null) { throw new Exception("Error! This submission record was not found. Please try again."); }
                    leaveApproval.ApproverName = HttpContext.User.Identity.Name;
                    leaveApproval.ApproverRole = leaveSubmission.ToEmployeeRole;
                    leaveApproval.IsApproved = true;
                    leaveApproval.LeavePlanId = model.LeavePlanId;
                    leaveApproval.TimeApproved = DateTime.Now;
                    leaveApproval.ApplicantName = leaveSubmission.FromEmployeeName;

                    bool IsApproved = await _leaveService.ApproveLeavePlanAsync(leaveApproval, leaveSubmission);
                    if (!IsApproved)
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted submission failed.";
                    }
                    else
                    {
                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByNameAsync(leaveSubmission.FromEmployeeName);
                        Employee approver = new Employee();
                        approver = await _ermService.GetEmployeeByNameAsync(leaveSubmission.ToEmployeeName);

                        //===== Send Notificiation Message to Approver ========//
                        Message message = new Message
                        {
                            MessageID = Guid.NewGuid().ToString(),
                            RecipientID = approver.EmployeeID,
                            RecipientName = approver.FullName,
                            SentBy = sender.FullName
                        };

                        //===== Send Email Notifications to Approver =========//
                        bool approverEmailCopySent = false;
                        UtilityHelper utilityHelper = new UtilityHelper(_configuration);
                        EmailModel recipientEmailCopy = new EmailModel();
                        recipientEmailCopy.RecipientName = approver.FullName;
                        if (!string.IsNullOrWhiteSpace(approver.OfficialEmail))
                        {
                            recipientEmailCopy.RecipientEmail = approver.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.RecipientEmail = approver.Email;
                        }

                        recipientEmailCopy.RecipientEmail = approver.OfficialEmail;
                        recipientEmailCopy.SenderName = sender.FullName;
                        switch (leaveSubmission.Purpose)
                        {
                            case "Approval":
                                recipientEmailCopy.Subject = "Request for Leave Plan Approval";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetLeavePlanApprovalEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetLeavePlanApprovalEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Request for Leave Plan Approval";
                                message.MessageBody = UtilityHelper.GetLeavePlanApprovalMessageContent(sender.FullName);
                                break;
                            case "Notification":
                                recipientEmailCopy.Subject = "Notice of Leave Plan";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetLeavePlanNoticeEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetLeavePlanNoticeEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Notice of Leave Plan";
                                message.MessageBody = UtilityHelper.GetLeavePlanNoticeMessageContent(sender.FullName);
                                break;
                            default:
                                break;
                        }

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                        {
                            // approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
                        }
                        return RedirectToAction("LeavePendingApproval", new { yr = DateTime.Now.Year });
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LeavePlanDecline(long id, long sd)
        {
            LeavePlanApprovalViewModel model = new LeavePlanApprovalViewModel();
            model.LeaveSubmissionId = sd;
            model.LeavePlanId = id;

            LeavePlan plan = new LeavePlan();
            if (model.LeavePlanId < 1)
            {
                return RedirectToAction("LeavePendingApproval");
            }
            var entity = await _leaveService.GetLeavePlanAsync(model.LeavePlanId);
            if (entity != null) { plan = entity; }
            model.LeaveEmployeeId = plan.LeaveEmployeeId;
            model.LeaveEmployeeName = plan.LeaveEmployeeName;
            model.LeavePlanDurationDescription = plan.LeavePlanDurationDescription;
            model.LeavePlanEndDate = plan.LeavePlanEndDate;
            model.LeavePlanId = plan.LeavePlanId;
            model.LeavePlanResumptionDate = plan.LeavePlanResumptionDate;
            model.LeavePlanStartDate = plan.LeavePlanStartDate;
            model.LeavePlanStatusDescription = plan.LeavePlanStatusDescription;
            model.LeavePlanStatusId = plan.LeavePlanStatusId;
            model.LeaveTypeName = plan.LeaveTypeName;
            model.LeaveTypeCode = plan.LeaveTypeCode;
            model.LeaveYear = plan.LeaveYear;
            model.LeavePlanDuration = plan.LeavePlanDuration;
            model.LeavePlanDurationTypeId = plan.LeavePlanDurationTypeId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LeavePlanDecline(LeavePlanApprovalViewModel model)
        {
            if (ModelState.IsValid)
            {
                LeaveSubmission leaveSubmission = new LeaveSubmission();
                LeaveApproval leaveApproval = new LeaveApproval();

                try
                {
                    leaveSubmission = await _leaveService.GetLeaveSubmissionByIdAsync(model.LeaveSubmissionId);
                    if (leaveSubmission == null) { throw new Exception("Error! This submission record was not found. Please try again."); }
                    leaveApproval.ApproverName = HttpContext.User.Identity.Name;
                    leaveApproval.ApproverRole = leaveSubmission.ToEmployeeRole;
                    leaveApproval.IsApproved = false;
                    leaveApproval.LeavePlanId = model.LeavePlanId;
                    leaveApproval.TimeApproved = DateTime.Now;
                    leaveApproval.ApplicantName = leaveSubmission.FromEmployeeName;

                    bool IsDeclined = await _leaveService.DeclineLeavePlanAsync(leaveApproval, leaveSubmission);
                    if (!IsDeclined)
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The operation failed.";
                    }
                    else
                    {
                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByNameAsync(leaveSubmission.FromEmployeeName);
                        Employee approver = new Employee();
                        approver = await _ermService.GetEmployeeByNameAsync(leaveSubmission.ToEmployeeName);

                        //===== Send Notificiation Message to Approver ========//
                        Message message = new Message
                        {
                            MessageID = Guid.NewGuid().ToString(),
                            RecipientID = approver.EmployeeID,
                            RecipientName = approver.FullName,
                            SentBy = sender.FullName
                        };

                        //===== Send Email Notifications to Approver =========//
                        bool approverEmailCopySent = false;
                        UtilityHelper utilityHelper = new UtilityHelper(_configuration);
                        EmailModel recipientEmailCopy = new EmailModel();
                        recipientEmailCopy.RecipientName = approver.FullName;
                        if (!string.IsNullOrWhiteSpace(approver.OfficialEmail))
                        {
                            recipientEmailCopy.RecipientEmail = approver.OfficialEmail;
                        }
                        else
                        {
                            recipientEmailCopy.RecipientEmail = approver.Email;
                        }

                        recipientEmailCopy.RecipientEmail = approver.OfficialEmail;
                        recipientEmailCopy.SenderName = sender.FullName;
                        switch (leaveSubmission.Purpose)
                        {
                            case "Approval":
                                recipientEmailCopy.Subject = "Request for Leave Plan Approval";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetLeavePlanApprovalEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetLeavePlanApprovalEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Request for Leave Plan Approval";
                                message.MessageBody = UtilityHelper.GetLeavePlanApprovalMessageContent(sender.FullName);
                                break;
                            case "Notification":
                                recipientEmailCopy.Subject = "Notice of Leave Plan";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetLeavePlanNoticeEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetLeavePlanNoticeEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Notice of Leave Plan";
                                message.MessageBody = UtilityHelper.GetLeavePlanNoticeMessageContent(sender.FullName);
                                break;
                            default:
                                break;
                        }

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                        {
                            // approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
                        }
                        return RedirectToAction("LeavePendingApproval", new { yr = DateTime.Now.Year });
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LeavePlans(int? l = null, int? u = null, int? y = null, int? m = null, string n = null)
        {
            LeavePlansListViewModel model = new LeavePlansListViewModel();
            model.l = l;
            model.u = u;
            model.y = y;
            model.m = m;
            model.n = n;
            if (model.y == null || model.y < 2020)
            {
                model.y = DateTime.Now.Year;
            }

            if (model.m == null || model.m < 1)
            {
                model.m = DateTime.Now.Month;
            }

            try
            {
               model.LeavePlanList = await _leaveService.SearchLeavePlansAsync(model.y.Value, model.m.Value, model.n, model.l, model.u);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", l);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", u);
            }
            return View(model);
        }
        #endregion


        #region Leave Requests
        public async Task<IActionResult> NewLeaveRequest(long? pd = null)
        {
            LeaveRequestViewModel model = new LeaveRequestViewModel();
            if(pd > 0)
            {
                LeavePlan plan = await _leaveService.GetLeavePlanAsync(pd.Value);
                if(plan != null) { model = model.ExtractFromLeavePlan(plan); }
            }
            else
            {
                model.RequestedStartDate = DateTime.Now.Date;
                model.LeaveYear = DateTime.Today.Year;
                model.LeaveEmployeeName = HttpContext.User.Identity.Name;
                model.LeaveEmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            }
            model.LeaveRequestStatusId = 0;
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewLeaveRequest(LeaveRequestViewModel model)
        {
            try
            {
                LeaveRequest r = new LeaveRequest();
                if (ModelState.IsValid)
                {
                    r = model.Convert();
                    Employee e = await _ermService.GetEmployeeByIdAsync(r.LeaveEmployeeId);
                    if (e == null || string.IsNullOrWhiteSpace(e.FullName)) { throw new Exception("Sorry, no record was found for this staff."); }
                    else
                    {
                        r.LeaveEmployeeId = e.EmployeeID;
                        r.LeaveEmployeeName = e.FullName;
                        r.DepartmentId = e.DepartmentID ?? 0;
                        r.UnitId = e.UnitID ?? 0;
                        r.LocationId = e.LocationID ?? 0;
                    }

                    if (!_validateEndDate(r.RequestedStartDate, r.RequestedEndDate)) { throw new Exception("Error: Invalid Start Date or End Date."); }
                    if (!_validateResumptionDate(r.RequestedResumptionDate.Value, r.RequestedEndDate)) { throw new Exception("Error: Invalid Resumption Date."); }

                    long LeaveRequestId = await _leaveService.CreateLeaveRequestAsync(r);
                    if (LeaveRequestId > 0)
                    {
                        return RedirectToAction("MyLeaveRecords", new { yr = model.RequestedStartDate.Year });
                    }
                    else { throw new Exception("An error was encountered. New Leave Request could not be added."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        #endregion

        #region Leave Utilities Controller Methods
        public async Task<IActionResult> LeaveNotes(string sp, int yr, long? pd = null, long? rd = null )
        {
            LeaveNoteListViewModel model = new LeaveNoteListViewModel();
            model.LeavePlanId = pd;
            model.LeaveRequestId = rd;
            model.LeaveYear = yr;
            model.SourcePage = sp;
            if (model.LeavePlanId > 0)
            {
                LeavePlan p = await _leaveService.GetLeavePlanAsync(model.LeavePlanId.Value);
                if (p != null)
                {
                    model.ApplicantID = p.LeaveEmployeeId;
                    model.ApplicantName = p.LeaveEmployeeName;
                    model.LoggedInEmployeeName = HttpContext.User.Identity.Name;
                    model.LoggedInEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

                    if (string.IsNullOrWhiteSpace(model.LoggedInEmployeeID))
                    {
                        await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                        return LocalRedirect("/Home/Login");
                    }
                    var plan_notes = await _leaveService.GetLeavePlanNotesAsync(model.LeavePlanId.Value);
                    if (plan_notes != null && plan_notes.Count > 0)
                    {
                        model.LeaveNoteList = plan_notes.ToList();
                    }
                }
                else if (p != null)
                {
                    //LeaveRequest r  = await _leaveService.GetLeaveRequestAsync(model.LeavePlanId.Value);
                    //if (p != null)
                    //{
                    //    model.ApplicantID = p.LeaveEmployeeId;
                    //    model.ApplicantName = p.LeaveEmployeeName;
                    //    model.LoggedInEmployeeName = HttpContext.User.Identity.Name;
                    //    model.LoggedInEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

                    //    if (string.IsNullOrWhiteSpace(model.LoggedInEmployeeID))
                    //    {
                    //        await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    //        return LocalRedirect("/Home/Login");
                    //    }
                    //    var plan_notes = await _leaveService.GetLeavePlanNotesAsync(model.LeavePlanId.Value);
                    //    if (plan_notes != null && plan_notes.Count > 0)
                    //    {
                    //        model.LeaveNoteList = plan_notes.ToList();
                    //    }

                    }

                }
            return View(model);
        }

        public async Task<IActionResult> LeaveActivities(long? pd = null, long? rd = null)
        {
            LeaveActivitiesViewModel model = new LeaveActivitiesViewModel();
            model.LeavePlanId = pd;
            model.LeaveRequestId = rd;

            var entities = await _leaveService.GetLeaveActivitiesAsync(model.LeavePlanId, model.LeaveRequestId);
            if (entities != null && entities.Count > 0)
            {
                model.LeaveActivityList = entities.ToList();
            }
            return View(model);
        }

        public async Task<IActionResult> LeaveApprovals(string sp, int yr, long? pd = null, long? rd = null )
        {
            LeaveApprovalListViewModel model = new LeaveApprovalListViewModel();
            model.LeavePlanId = pd;
            model.LeaveRequestId = rd;
            model.LeaveYear = yr;
            model.SourcePage = sp;
            //if (model.LeavePlanId > 0)
            //{
            //    var entities = await _leaveService.GetLeaveApprovalsAsync(id);
            //    if (entities != null && entities.Count > 0)
            //    {
            //        model.LeaveApprovalList = entities.ToList();
            //    }
            //}
            return View(model);
        }



        //public async Task<IActionResult> LeaveApprovals(long id, string sp, int yr)
        //{
        //    LeaveApprovalListViewModel model = new LeaveApprovalListViewModel();
        //    model.LeaveID = id;
        //    model.LeaveYear = yr;
        //    model.SourcePage = sp;
        //    if (id > 0)
        //    {
        //        var entities = await _lmsService.GetLeaveApprovalsAsync(id);
        //        if (entities != null && entities.Count > 0)
        //        {
        //            model.LeaveApprovalList = entities.ToList();
        //        }
        //    }
        //    return View(model);
        //}
        #endregion


        #region  Controller Helper Methods
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
                    leaveEndDate = _leaveService.GenerateLeaveEndDate(convertedStartDate, dt, dr).ToString("yyyy-MM-dd");
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
        public string SaveLeaveNote(string nm, string msg, long? pd = null, long? rd = null )
        {
            LeaveNote note = new LeaveNote()
            {
                TimeAdded = DateTime.Now,
                FromEmployeeName = nm,
                LeavePlanId = pd,
                LeaveRequestId = rd,
                NoteContent = msg
            };

            if (string.IsNullOrWhiteSpace(nm) || string.IsNullOrWhiteSpace(msg)) { return "parameter"; }
            //string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_leaveService.AddLeaveNoteAsync(note).Result)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }
        public string SaveLeaveRequestNote(long id, string nm, string msg)
        {
            LeaveNote note = new LeaveNote()
            {
                TimeAdded = DateTime.Now,
                FromEmployeeName = nm,
                LeavePlanId = null,
                LeaveRequestId = id,
                NoteContent = msg
            };

            if (id < 1 || string.IsNullOrWhiteSpace(nm) || string.IsNullOrWhiteSpace(msg)) { return "parameter"; }
            //string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_leaveService.AddLeaveNoteAsync(note).Result)
                {
                    return "saved";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }
        public string DeleteLeaveSubmission(int sd)
        {
            if (sd < 1) { return "parameter"; }
            //string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_leaveService.DeleteLeaveSubmissionAsync(sd).Result)
                {
                    return "deleted";
                }
                else
                {
                    return "failed";
                }
            }
            catch
            {
                return "failed";
            }
        }

        public JsonResult GetResumptionDate(string ed)
        {
            ResultObject returnObj = new ResultObject();
            DateTime resumptionDate = DateTime.Today.Date;
            DateTime leaveEndDate = DateTime.Today;
            string errorMessage = string.Empty;
            try
            {
                if (DateTime.TryParse(ed, out leaveEndDate))
                    switch (leaveEndDate.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            resumptionDate = leaveEndDate.AddDays(3);
                            break;
                        case DayOfWeek.Saturday:
                            resumptionDate = leaveEndDate.AddDays(2);
                            break;
                        default:
                            resumptionDate = leaveEndDate.AddDays(1);
                            break;
                    }
            }
            catch (Exception ex)
            {
                returnObj.errorMessage = ex.Message;
            }

            returnObj.errorMessage = errorMessage;
            returnObj.result = resumptionDate.ToString("yyyy-MM-dd");
            var jsonObj = System.Text.Json.JsonSerializer.Serialize(returnObj);
            return Json(jsonObj);
        }
        private bool _validateEndDate(DateTime leaveStartDate, DateTime leaveEndDate)
        {
            if (leaveEndDate <= leaveStartDate) { return false; }
            else return true;
        }
        private bool _validateResumptionDate(DateTime leaveResumptionDate, DateTime leaveEndDate)
        {
            if (leaveResumptionDate <= leaveEndDate) { return false; }
            else return true;
        }
        #endregion
    }
}
