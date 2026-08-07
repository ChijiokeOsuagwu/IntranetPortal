using ClosedXML.Excel;
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
using System.Data;
using System.IO;
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

            model.sn = HttpContext.User.Identity.Name;
            model.ei = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            try
            {
                if (string.IsNullOrWhiteSpace(model.ei))
                {
                    await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    return LocalRedirect("/Home/Login");
                }
                Employee employee = await _ermService.GetEmployeeByIdAsync(model.ei);
                if (employee == null || string.IsNullOrWhiteSpace(employee.FullName)) { throw new Exception("Sorry, no employee record was found for this user."); }
                model.sn = employee.FullName;
                model.ei = employee.EmployeeID;
                model.LeavePlanList = await _leaveService.GetLeavePlansAsync(employee.EmployeeID, model.yr);
                model.LeaveRequestList = await _leaveService.GetLeaveRequestsAsync(employee.EmployeeID, model.yr);
                model.CurrentLeaveRollingBalances = await _leaveService.GetRefreshedLeaveBalancesAsync("ANL", DateTime.Now.Year, employee.EmployeeID, employee.FullName);

                //model.CurrentLeaveBalances = await _leaveService.RefreshAndRetrieveLeaveBalancesAsync("ANL", DateTime.Now.Year, employee.EmployeeID, employee.FullName);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }


        #region Leave Plans
        public async Task<IActionResult> NewLeavePlan()
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            try
            {
                model.LeaveYear = DateTime.Today.Year;
                model.LeaveEmployeeName = HttpContext.User.Identity.Name;
                model.LeaveEmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                model.RollingBalances = await _leaveService.GetRefreshedLeaveBalancesAsync("ANL", model.LeaveYear, model.LeaveEmployeeId, model.LeaveEmployeeName);
                if (model.RollingBalances.PreviousBalanceExpiryMonth > DateTime.Now.Month)
                {
                    model.CurrentLeaveBalance = model.RollingBalances.TotalOutstandingLeaveDaysBeforeExpiry;
                    model.CurrentLeaveBalanceDescription = $"{model.RollingBalances.TotalOutstandingLeaveDaysBeforeExpiry} Working Day(s)";
                }
                else
                {
                    model.CurrentLeaveBalance = model.RollingBalances.TotalOutstandingLeaveDaysAfterExpiry;
                    model.CurrentLeaveBalanceDescription = $"{model.RollingBalances.TotalOutstandingLeaveDaysAfterExpiry} Working Day(s)";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
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
                    if (model.LeaveTypeCode == "ANL" & model.LeavePlanDuration > model.CurrentLeaveBalance)
                    {
                        throw new Exception("Sorry, your Leave Duration cannot exceed your Current Leave Balance.");
                    }

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
            try
            {
                var entity = await _leaveService.GetLeavePlanAsync(id);
                if (entity != null) { plan = entity; }
                model = model.Extract(plan);

                model.RollingBalances = await _leaveService.GetRefreshedLeaveBalancesAsync("ANL", model.LeaveYear, model.LeaveEmployeeId, model.LeaveEmployeeName);
                if (model.RollingBalances.PreviousBalanceExpiryMonth > DateTime.Now.Month)
                {
                    model.CurrentLeaveBalance = model.RollingBalances.TotalOutstandingLeaveDaysBeforeExpiry;
                    model.CurrentLeaveBalanceDescription = $"{model.RollingBalances.TotalOutstandingLeaveDaysBeforeExpiry} Working Day(s)";
                }
                else
                {
                    model.CurrentLeaveBalance = model.RollingBalances.TotalOutstandingLeaveDaysAfterExpiry;
                    model.CurrentLeaveBalanceDescription = $"{model.RollingBalances.TotalOutstandingLeaveDaysAfterExpiry} Working Day(s)";
                }

            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

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
                    if (model.LeaveTypeCode == "ANL" & model.LeavePlanDuration > model.CurrentLeaveBalance)
                    {
                        throw new Exception("Sorry, your Leave Duration cannot exceed your Current Leave Balance.");
                    }

                    d = model.Convert();

                    if (!_validateEndDate(d.LeavePlanStartDate.Value, d.LeavePlanEndDate.Value)) { throw new Exception("Error: Invalid Leave Start Date or End Date."); }
                    if (!_validateResumptionDate(d.LeavePlanResumptionDate.Value, d.LeavePlanEndDate.Value)) { throw new Exception("Error: Invalid Resumption Date."); }

                    if (await _leaveService.UpdateLeavePlanAsync(d))
                    {
                        return RedirectToAction("MyLeaveRecords", new { yr = model.LeavePlanStartDate?.Year });
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
                        return RedirectToAction("MyLeaveRecords", new { yr = model.LeavePlanStartDate.Value.Year });
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

        public async Task<IActionResult> LeavePendingApproval(int? yr = null)
        {
            LeavePendingApprovalListViewModel model = new LeavePendingApprovalListViewModel();
            if (yr == null || yr < 2020) { model.yr = DateTime.Now.Year; }
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

            if (User.Identity is { IsAuthenticated: true })
            {
                if (User.IsInRole("GBSHRDAPV"))
                {
                    model.UserIsSeniorHr = true;
                }
            }


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

        public async Task<IActionResult> LeavePlanApproval(long id, long sd, string sp = null)
        {
            LeavePlanApprovalViewModel model = new LeavePlanApprovalViewModel();
            model.LeaveSubmissionId = sd;
            model.LeavePlanId = id;
            model.SourcePage = model.src = sp;

            LeavePlan plan = new LeavePlan();
            if (model.LeavePlanId < 1)
            {
                if (model.SourcePage == "lpa")
                {
                    return RedirectToAction("LeavePendingApproval");
                }
                else if (model.SourcePage == "lsh")
                {
                    return RedirectToAction("LeaveSubmittedToHr");
                }
                else
                {
                    return RedirectToAction("Index");
                }
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
                    if (leaveSubmission == null) { throw new Exception("Error! This submission record was not found. Please try again."); }
                    leaveApproval.ApproverName = HttpContext.User.Identity.Name;
                    leaveApproval.ApproverRole = leaveSubmission.ToEmployeeRole;
                    leaveApproval.IsApproved = true;
                    leaveApproval.LeavePlanId = model.LeavePlanId;
                    leaveApproval.TimeApproved = DateTime.Now;
                    leaveApproval.ApplicantName = leaveSubmission.FromEmployeeName;

                    bool IsApproved = await _leaveService.ApproveLeaveAsync(leaveApproval, leaveSubmission, DocumentType.LeavePlan);
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

                        if (model.SourcePage == "lpa" || model.src == "lpa")
                        {
                            return RedirectToAction("LeavePendingApproval");
                        }
                        else if (model.SourcePage == "lsh" || model.src == "lsh")
                        {
                            return RedirectToAction("LeaveSubmittedToHr");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
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
                    leaveApproval.ApproverComments = model.DeclineReason;

                    bool IsDeclined = await _leaveService.DeclineLeaveAsync(leaveApproval, leaveSubmission, DocumentType.LeavePlan);
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

        #endregion

        #region Leave Requests
        public async Task<IActionResult> NewLeaveRequest(long? pd = null)
        {
            LeaveRequestViewModel model = new LeaveRequestViewModel();
            try
            {
                if (pd > 0)
                {
                    LeavePlan plan = await _leaveService.GetLeavePlanAsync(pd.Value);
                    if (plan != null) { model = model.ExtractFromLeavePlan(plan); }
                }
                else
                {
                    model.RequestedStartDate = DateTime.Now.Date;
                    model.LeaveYear = DateTime.Today.Year;
                    model.LeaveEmployeeName = HttpContext.User.Identity.Name;
                    model.LeaveEmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                }
                model.LeaveRequestStatusId = 0;
                model.RollingBalances = await _leaveService.GetRefreshedLeaveBalancesAsync("ANL", model.LeaveYear, model.LeaveEmployeeId, model.LeaveEmployeeName);
                if (model.RollingBalances.PreviousBalanceExpiryMonth > DateTime.Now.Month)
                {
                    model.CurrentLeaveBalance = model.RollingBalances.TotalOutstandingLeaveDaysBeforeExpiry;
                    model.CurrentLeaveBalanceDescription = $"{model.RollingBalances.TotalOutstandingLeaveDaysBeforeExpiry} Working Day(s)";
                }
                else
                {
                    model.CurrentLeaveBalance = model.RollingBalances.TotalOutstandingLeaveDaysAfterExpiry;
                    model.CurrentLeaveBalanceDescription = $"{model.RollingBalances.TotalOutstandingLeaveDaysAfterExpiry} Working Day(s)";
                }
                var employee = await _ermService.GetEmployeeByIdAsync(model.LeaveEmployeeId);
                if (employee != null)
                {
                    model.EligibleForLeaveAllowance = employee.IsEligibleForLeaveAllowance;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
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
                LeaveAllowance leaveAllowance = new LeaveAllowance();
                if (ModelState.IsValid)
                {
                    if (model.LeaveTypeCode == "ANL" & model.RequestedDuration > model.CurrentLeaveBalance)
                    {
                        throw new Exception("Sorry, your Leave Duration cannot exceed your Current Leave Balance.");
                    }
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

                        leaveAllowance.LeaveEmployeeId = e.EmployeeID;
                        leaveAllowance.LeaveDepartmentId = e.DepartmentID ?? 0;
                        leaveAllowance.LeaveYear = r.LeaveYear;
                        leaveAllowance.PaymentYear = r.LeaveYear;
                        leaveAllowance.LeaveLocationId = e.LocationID ?? 0;
                        leaveAllowance.LeaveRequestId = r.LeaveRequestId;
                        leaveAllowance.LeaveUnitId = e.UnitID ?? 0;
                        leaveAllowance.PaymentMonth = r.RequestedStartDate.Month;
                        leaveAllowance.RecordedBy = HttpContext.User.Identity.Name;
                        leaveAllowance.RecordedTime = DateTime.UtcNow;
                        leaveAllowance.RequestedTime = DateTime.UtcNow;

                        r.LeaveAllowance = leaveAllowance;
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

        public async Task<IActionResult> EditLeaveRequest(long id)
        {
            if (id < 1) { return RedirectToAction("NewLeaveRequest"); }
            LeaveRequestViewModel model = new LeaveRequestViewModel();
            LeaveRequest request = new LeaveRequest();

            try
            {
                var entity = await _leaveService.GetLeaveRequestAsync(id);
                if (entity != null) { request = entity; }
                model = model.ExtractFromLeaveRequest(request);
                model.RollingBalances = await _leaveService.GetRefreshedLeaveBalancesAsync("ANL", model.LeaveYear, model.LeaveEmployeeId, model.LeaveEmployeeName);
                if (model.RollingBalances.PreviousBalanceExpiryMonth > DateTime.Now.Month)
                {
                    model.CurrentLeaveBalance = model.RollingBalances.TotalOutstandingLeaveDaysBeforeExpiry;
                    model.CurrentLeaveBalanceDescription = $"{model.RollingBalances.TotalOutstandingLeaveDaysBeforeExpiry} Working Day(s)";
                }
                else
                {
                    model.CurrentLeaveBalance = model.RollingBalances.TotalOutstandingLeaveDaysAfterExpiry;
                    model.CurrentLeaveBalanceDescription = $"{model.RollingBalances.TotalOutstandingLeaveDaysAfterExpiry} Working Day(s)";
                }

                var employee = await _ermService.GetEmployeeByIdAsync(model.LeaveEmployeeId);
                if (employee != null)
                {
                    model.EligibleForLeaveAllowance = employee.IsEligibleForLeaveAllowance;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeaveRequest(LeaveRequestViewModel model)
        {
            try
            {
                LeaveRequest request = new LeaveRequest();
                LeaveAllowance leaveAllowance = new LeaveAllowance();
                if (ModelState.IsValid)
                {
                    if (model.LeaveTypeCode == "ANL" & model.RequestedDuration > model.CurrentLeaveBalance)
                    {
                        throw new Exception("Sorry, your Leave Duration cannot exceed your Current Leave Balance.");
                    }
                    request = model.Convert();

                    if (!_validateEndDate(request.RequestedStartDate, request.RequestedEndDate)) { throw new Exception("Error: Invalid Start Date and/or End Date."); }
                    if (!_validateResumptionDate(request.RequestedResumptionDate.Value, request.RequestedEndDate)) { throw new Exception("Error: Invalid Resumption Date."); }

                    Employee e = await _ermService.GetEmployeeByIdAsync(request.LeaveEmployeeId);
                    if (e == null || string.IsNullOrWhiteSpace(e.FullName)) { throw new Exception("Sorry, no record was found for this staff."); }
                    else
                    {
                        leaveAllowance.LeaveEmployeeId = e.EmployeeID;
                        leaveAllowance.LeaveDepartmentId = e.DepartmentID ?? 0;
                        leaveAllowance.LeaveYear = request.LeaveYear;
                        leaveAllowance.PaymentYear = request.LeaveYear;
                        leaveAllowance.LeaveLocationId = e.LocationID ?? 0;
                        leaveAllowance.LeaveRequestId = request.LeaveRequestId;
                        leaveAllowance.LeaveUnitId = e.UnitID ?? 0;
                        leaveAllowance.PaymentMonth = request.RequestedStartDate.Month;
                        leaveAllowance.RecordedBy = HttpContext.User.Identity.Name;
                        leaveAllowance.RecordedTime = DateTime.UtcNow;
                        leaveAllowance.RequestedTime = DateTime.UtcNow;

                        request.LeaveAllowance = leaveAllowance;
                    }

                    if (await _leaveService.UpdateLeaveRequestAsync(request))
                    {
                        return RedirectToAction("MyLeaveRecords", new { yr = model.RequestedStartDate.Year });
                    }
                    else { throw new Exception("An error was encountered. Attempt to update Leave Request was not successful."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> DeleteLeaveRequest(long id)
        {
            LeaveRequestViewModel model = new LeaveRequestViewModel();
            var leaveRequest = await _leaveService.GetLeaveRequestAsync(id);
            if (leaveRequest != null)
            {
                model = model.ExtractFromLeaveRequest(leaveRequest);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLeaveRequest(LeaveRequestViewModel model)
        {
            try
            {
                if (model.LeaveRequestId > 0)
                {
                    bool IsDeleted = await _leaveService.DeleteLeaveRequestAsync(model.LeaveRequestId);
                    if (IsDeleted)
                    {
                        return RedirectToAction("MyLeaveRecords", new { yr = model.RequestedStartDate.Year });
                    }
                    else { throw new Exception("An error was encountered. Leave Request could not be deleted."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ViewLeaveRequest(long id)
        {
            LeaveRequestViewModel model = new LeaveRequestViewModel();
            LeaveRequest request = new LeaveRequest();
            if (id < 1)
            {
                return RedirectToAction("NewLeaveRequest");
            }
            var entity = await _leaveService.GetLeaveRequestAsync(id);
            if (entity != null) { request = entity; }
            model = model.ExtractFromLeaveRequest(request);

            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> SubmitLeave(long? pd = null, long? rd = null)
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
        public async Task<IActionResult> SubmitLeave(SubmitLeaveViewModel model)
        {
            if (ModelState.IsValid)
            {
                LeaveSubmission leaveSubmission = new LeaveSubmission();
                leaveSubmission = model.Convert();
                leaveSubmission.TimeSubmitted = DateTime.Now;

                string documentType = string.Empty;
                if (model.LeavePlanId > 0) { documentType = "Leave Plan"; }
                else if (model.LeaveRequestId > 0) { documentType = "Leave Request"; }

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

                        if(model.LeaveRequestId > 0)
                        {
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
                        }
                        else if(model.LeaveRequestId > 0)
                        {
                            switch (leaveSubmission.Purpose)
                            {
                                case "Approval":
                                    recipientEmailCopy.Subject = "Request for Leave Application Approval";
                                    recipientEmailCopy.HtmlContent = UtilityHelper.GetLeaveRequestApprovalEmailHtmlContent(approver.FullName, sender.FullName);
                                    recipientEmailCopy.PlainContent = UtilityHelper.GetLeaveRequestApprovalEmailPlainContent(approver.FullName, sender.FullName);

                                    message.Subject = "Request for Leave Application Approval";
                                    message.MessageBody = UtilityHelper.GetLeavePlanApprovalMessageContent(sender.FullName);
                                    break;
                                case "Notification":
                                    recipientEmailCopy.Subject = "Notice of Leave Application";
                                    recipientEmailCopy.HtmlContent = UtilityHelper.GetLeaveRequestNoticeEmailHtmlContent(approver.FullName, sender.FullName);
                                    recipientEmailCopy.PlainContent = UtilityHelper.GetLeaveRequestNoticeEmailPlainContent(approver.FullName, sender.FullName);

                                    message.Subject = "Notice of Leave Application";
                                    message.MessageBody = UtilityHelper.GetLeaveRequestNoticeMessageContent(sender.FullName);
                                    break;
                                default:
                                    break;
                            }
                        }

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                        {
                            // approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
                        }

                        return RedirectToAction("MyLeaveRecords", new { yr = DateTime.Now.Year });
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

        public async Task<IActionResult> LeaveRequestApproval(long id, long sd)
        {
            LeaveRequestApprovalViewModel model = new LeaveRequestApprovalViewModel();
            model.LeaveSubmissionId = sd;
            model.LeaveRequestId = id;

            LeaveRequest request = new LeaveRequest();
            if (model.LeaveRequestId < 1)
            {
                return RedirectToAction("LeavePendingApproval");
            }
            var entity = await _leaveService.GetLeaveRequestAsync(model.LeaveRequestId);
            if (entity != null) { request = entity; }
            model.LeaveEmployeeId = request.LeaveEmployeeId;
            model.LeaveEmployeeName = request.LeaveEmployeeName;
            model.RequestedDurationDescription = request.RequestedDurationDescription;
            model.RequestedEndDate = request.RequestedEndDate;
            model.LeaveRequestId = request.LeaveRequestId;
            model.RequestedResumptionDate = request.RequestedResumptionDate;
            model.RequestedStartDate = request.RequestedStartDate;
            model.LeaveRequestStatusDescription = request.LeaveRequestStatusDescription;
            model.LeaveRequestStatusId = request.LeaveRequestStatusId;
            model.LeaveTypeName = request.LeaveTypeName;
            model.LeaveTypeCode = request.LeaveTypeCode;
            model.LeaveYear = request.LeaveYear;
            model.RequestedDuration = request.RequestedDuration;
            model.RequestedDurationTypeId = request.RequestedDurationTypeId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LeaveRequestApproval(LeaveRequestApprovalViewModel model)
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
                    leaveApproval.IsApproved = true;
                    leaveApproval.LeaveRequestId = model.LeaveRequestId;
                    leaveApproval.TimeApproved = DateTime.Now;
                    leaveApproval.ApplicantName = leaveSubmission.FromEmployeeName;

                    bool IsApproved = await _leaveService.ApproveLeaveAsync(leaveApproval, leaveSubmission, DocumentType.LeaveRequest);
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

        public async Task<IActionResult> LeaveRequestDecline(long id, long sd, string sp = null)
        {
            LeaveRequestApprovalViewModel model = new LeaveRequestApprovalViewModel();
            model.LeaveSubmissionId = sd;
            model.LeaveRequestId = id;
            model.SourcePage = model.src = sp;

            LeaveRequest request = new LeaveRequest();
            if (model.LeaveRequestId < 1)
            {
                return RedirectToAction("LeavePendingApproval");
            }
            var entity = await _leaveService.GetLeaveRequestAsync(model.LeaveRequestId);
            if (entity != null) { request = entity; }
            model.LeaveEmployeeId = request.LeaveEmployeeId;
            model.LeaveEmployeeName = request.LeaveEmployeeName;
            model.RequestedDurationDescription = request.RequestedDurationDescription;
            model.RequestedEndDate = request.RequestedEndDate;
            model.LeaveRequestId = request.LeaveRequestId;
            model.RequestedResumptionDate = request.RequestedResumptionDate;
            model.RequestedStartDate = request.RequestedStartDate;
            model.LeaveRequestStatusDescription = request.LeaveRequestStatusDescription;
            model.LeaveRequestStatusId = request.LeaveRequestStatusId;
            model.LeaveTypeName = request.LeaveTypeName;
            model.LeaveTypeCode = request.LeaveTypeCode;
            model.LeaveYear = request.LeaveYear;
            model.RequestedDuration = request.RequestedDuration;
            model.RequestedDurationTypeId = request.RequestedDurationTypeId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LeaveRequestDecline(LeaveRequestApprovalViewModel model)
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
                    leaveApproval.LeaveRequestId = model.LeaveRequestId;
                    leaveApproval.TimeApproved = DateTime.Now;
                    leaveApproval.ApplicantName = leaveSubmission.FromEmployeeName;
                    leaveApproval.ApproverComments = model.DeclineReason;

                    bool IsDeclined = await _leaveService.DeclineLeaveAsync(leaveApproval, leaveSubmission, DocumentType.LeaveRequest);
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
                                recipientEmailCopy.Subject = "Request for Leave Approval";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetLeavePlanApprovalEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetLeavePlanApprovalEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Request for Leave Approval";
                                message.MessageBody = UtilityHelper.GetLeavePlanApprovalMessageContent(sender.FullName);
                                break;
                            case "Notification":
                                recipientEmailCopy.Subject = "Notice of Leave Request";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetLeavePlanNoticeEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetLeavePlanNoticeEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Notice of Leave Request";
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

        public async Task<IActionResult> SendResumptionNotice(long id)
        {
            SendResumptionNoticeViewModel model = new SendResumptionNoticeViewModel();
            try
            {
                if (id < 1) { throw new ArgumentNullException("id"); }
                var entity = await _leaveService.GetLeaveRequestAsync(id);
                model.LeaveRequest = entity ?? throw new ArgumentNullException(nameof(entity));
                model.LeaveRequestId = entity.LeaveRequestId;
                model.LeaveEmployeeName = entity.LeaveEmployeeName;
                model.ApprovedResumptionDate = entity.RequestedResumptionDate.Value;
                model.ResumptionDateByEmployee = entity.RequestedResumptionDate.Value.Date;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendResumptionNotice(SendResumptionNoticeViewModel model)
        {
            if (ModelState.IsValid)
            {
                LeaveResumption leaveResumption = new LeaveResumption();

                try
                {
                    leaveResumption.LeaveEmployeeName = HttpContext.User.Identity.Name;
                    leaveResumption.NoOfExtraDaysByEmployee = model.NoOfExtraDaysByEmployee;
                    leaveResumption.NoOfUnusedDaysByEmployee = model.NoOfUnusedDaysByEmployee;
                    leaveResumption.ResumptionDateByEmployee = model.ResumptionDateByEmployee;
                    leaveResumption.ReasonByEmployee = model.ReasonByEmployee;
                    leaveResumption.LeaveRequestId = model.LeaveRequestId;
                    leaveResumption.ApprovedResumptionDate = model.ApprovedResumptionDate;
                    leaveResumption.DateRecordedByEmployee = DateTime.UtcNow;
                    leaveResumption.EmployeeRequestAdjustment = model.EmployeeRequestAdjustment;
                    leaveResumption.RequestedAdjustmentType = model.RequestedAdjustmentType;

                    bool IsSent = await _leaveService.SubmitLeaveResumptionNoticeAsync(leaveResumption, model.SendToEmployeeName, model.SendToEmployeeRole);
                    if (!IsSent)
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The operation failed.";
                    }
                    else
                    {
                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByNameAsync(leaveResumption.LeaveEmployeeName);
                        Employee approver = new Employee();
                        approver = await _ermService.GetEmployeeByNameAsync(model.SendToEmployeeName);

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

                        recipientEmailCopy.Subject = "Notice of Resumption from Leave";
                        recipientEmailCopy.HtmlContent = UtilityHelper.GetLeaveResumptionNoticeEmailHtmlContent(approver.FullName, sender.FullName);
                        recipientEmailCopy.PlainContent = UtilityHelper.GetLeaveResumptionNoticeEmailPlainContent(approver.FullName, sender.FullName);

                        message.Subject = "Notice of Resumption from Leave";
                        message.MessageBody = UtilityHelper.GetLeaveResumptionNoticeMessageContent(sender.FullName);

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                        {
                            // approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
                        }
                        return RedirectToAction("MyLeaveRecords");
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> MyLeaveAdjustments(long id)
        {
            LeaveAdjustmentListViewModel model = new LeaveAdjustmentListViewModel();
            model.LeaveRequestDetail = new LeaveRequest();

            try
            {
                if (id > 0)
                {
                    var entity = await _leaveService.GetLeaveRequestAsync(id);
                    if (entity != null)
                    {
                        model.LeaveRequestDetail = entity;
                    }

                    var entities = await _leaveService.GetLeaveAdjustmentsAsync(id);
                    if (entities != null)
                    {
                        model.LeaveAdjustmentList = entities;
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

        #region HR Actions

        public async Task<IActionResult> LeavePlans(int? l = null, int? u = null, int? y = null, int? m = null, string n = null)
        {
            LeavePlansListViewModel model = new LeavePlansListViewModel();
            model.l = l;
            model.u = u;
            model.y = y;
            model.m = m ?? 0;
            model.n = n;
            if (model.y == null || model.y < 2020)
            {
                model.y = DateTime.Now.Year;
            }

            try
            {
                model.LeavePlanList = await _leaveService.SearchLeavePlansAsync(model.y.Value, model.m ?? 0, model.n, model.l, model.u);
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

        public IActionResult FlagLeavePlan(int id)
        {
            FlagLeaveViewModel model = new FlagLeaveViewModel();
            model.LeavePlanId = id;
            model.FlaggedByEmployeeName = HttpContext.User.Identity.Name;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FlagLeavePlan(FlagLeaveViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _leaveService.UpdateLeavePlanFlagAsync(model.LeavePlanId, true, model.FlagReason, model.FlaggedByEmployeeName);
                    model.OperationIsSuccessful = true;
                    model.ViewModelSuccessMessage = "Leave Plan was flagged successfully! Please close this tab.";
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LeaveRecords(string sn, int yr)
        {
            MyLeaveRecordsViewModel model = new MyLeaveRecordsViewModel();
            model.sn = sn;
            if (yr < 2020)
            {
                model.yr = DateTime.Now.Year;
            }
            else { model.yr = yr; }

            try
            {
                if (!string.IsNullOrWhiteSpace(model.sn))
                {
                    Employee employee = await _ermService.GetEmployeeByNameAsync(model.sn);
                    if (employee == null || string.IsNullOrWhiteSpace(employee.EmployeeID)) { throw new Exception("Sorry, no employee record was found for this staff."); }
                    model.ei = employee.EmployeeID;
                    model.sn = employee.FullName;
                    model.LeavePlanList = await _leaveService.GetLeavePlansAsync(model.ei, model.yr);
                    model.LeaveRequestList = await _leaveService.GetLeaveRequestsAsync(model.ei, model.yr);
                    model.CurrentLeaveRollingBalances = await _leaveService.GetRefreshedLeaveBalancesAsync("ANL", model.yr, model.ei, model.sn);
                }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> LeaveSubmittedToHr(int? yr = null)
        {
            LeaveSubmittedToHrListViewModel model = new LeaveSubmittedToHrListViewModel();
            if (yr == null || yr < 2020) { model.yr = DateTime.Now.Year; }
            //string userFullName = string.Empty;
            try
            {
                string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    return LocalRedirect("/Home/Login");
                }

                if (User.Identity is { IsAuthenticated: true })
                {
                    if (User.IsInRole("GBSHRDAPV"))
                    {
                        model.UserIsSeniorHr = true;
                    }
                }

                var entities = await _leaveService.GetLeaveSubmissionsByApproverRoleAsync("HR Department", model.yr);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveSubmissionList = entities.Where(x => x.IsActioned == false).ToList();
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ApprovedLeaveRequests(int? l = null, int? u = null, int? y = null, int? m = null)
        {
            LeaveRequestsListViewModel model = new LeaveRequestsListViewModel();
            model.l = l;
            model.u = u;
            model.y = y;
            model.m = m ?? 0;
            if (model.y == null || model.y < 2020)
            {
                model.y = DateTime.Now.Year;
            }

            try
            {
                model.LeaveRequestList = await _leaveService.SearchApprovedLeaveRequestsAsync(model.y.Value, model.m ?? 0, model.l, model.u);
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

        public async Task<IActionResult> LeaveRequestsDueResumption(int? l = null, int? u = null, int? y = null, int? m = null)
        {
            LeaveRequestsListViewModel model = new LeaveRequestsListViewModel();
            model.l = l;
            model.u = u;
            model.y = y;
            model.m = m;
            if (model.y == null || model.y < 2026)
            {
                model.y = DateTime.Now.Year;
            }

            try
            {
                model.LeaveRequestList = await _leaveService.GetLeaveRequestsDueResumptionAsync(model.y.Value, model.m ?? 0, model.l, model.u);
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

        public async Task<IActionResult> ViewResumptionNotice(long rd)
        {
            ConfirmResumptionViewModel model = new ConfirmResumptionViewModel();
            try
            {
                if (rd < 1) { throw new ArgumentNullException("Required parameter Leave Request ID has an invalid value."); }
                var leaveResumption = await _leaveService.GetLeaveResumptionAsync(rd, 0);
                if (leaveResumption != null) { model = model.Convert(leaveResumption); }

                var leaveRequest = await _leaveService.GetLeaveRequestAsync(rd);
                if (leaveRequest != null)
                {
                    model.LeaveYear = leaveRequest.LeaveYear;
                    model.LeaveTypeName = leaveRequest.LeaveTypeName;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> LeaveAdjustments(long id, string sp)
        {
            LeaveAdjustmentListViewModel model = new LeaveAdjustmentListViewModel();
            model.LeaveRequestDetail = new LeaveRequest();
            model.SourcePage = model.src = sp;

            try
            {
                if (id > 0)
                {
                    var entity = await _leaveService.GetLeaveRequestAsync(id);
                    if (entity != null)
                    {
                        model.LeaveRequestDetail = entity;
                    }

                    var entities = await _leaveService.GetLeaveAdjustmentsAsync(id);
                    if (entities != null)
                    {
                        model.LeaveAdjustmentList = entities;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> AddLeaveAdjustment(long id, string sp = null)
        {
            AddLeaveAdjustmentViewModel model = new AddLeaveAdjustmentViewModel();
            model.SourcePage = model.src = sp;
            model.LeaveRequestId = id;
            try
            {
                LeaveRequest request = new LeaveRequest();

                if (id < 1)
                {
                    throw new Exception("No record was found for this Leave.");
                }
                var entity = await _leaveService.GetLeaveRequestAsync(id);
                if (entity != null) { request = entity; }
                model.LeaveEmployeeId = request.LeaveEmployeeId;
                model.LeaveEmployeeName = request.LeaveEmployeeName;
                model.LeaveTypeCode = request.LeaveTypeCode;
                model.LeaveTypeName = request.LeaveTypeName;
                model.LeaveYear = request.LeaveYear;
                model.LeaveUnitId = request.UnitId;
                model.LeaveDepartmentId = request.DepartmentId;
                model.LeaveLocationId = request.LocationId;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddLeaveAdjustment(AddLeaveAdjustmentViewModel model)
        {
            try
            {
                LeaveAdjustment adjustment = new LeaveAdjustment();
                if (ModelState.IsValid)
                {
                    adjustment = model.Convert();
                    adjustment.AdjustmentAddedBy = HttpContext.User.Identity.Name;
                    adjustment.AdjustmentDate = DateTime.UtcNow;
                    adjustment.DurationDescription = $"{model.NumberOfDays} Working Day(s)";

                    if (await _leaveService.AddLeaveAdjustmentAsync(adjustment))
                    {
                        return RedirectToAction("LeaveAdjustments", new { id = model.LeaveRequestId });
                    }
                    else { throw new Exception("An error was encountered. Attempt to add Leave Adjustment was not successful."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> CloseLeaveRequest(long id, string sp)
        {
            CloseLeaveRequestViewModel model = new CloseLeaveRequestViewModel();
            model.SourcePage = model.src = sp;

            LeaveRequest request = new LeaveRequest();
            try
            {
                if (id < 1)
                {
                    throw new Exception("No record was found for this Leave Request.");
                }
                var entity = await _leaveService.GetLeaveRequestAsync(id);
                if (entity != null) { request = entity; }
                model = model.Convert(request);
                model.ActualLeaveDuration = request.RequestedDuration;
                model.ActualLeaveDurationTypeId = request.RequestedDurationTypeId;
                model.ActualLeaveEndDate = request.RequestedEndDate;
                model.ActualLeaveStartDate = request.RequestedStartDate;
                model.HrResumptionDate = request.RequestedResumptionDate;
                model.LeaveUnitId = request.UnitId;
                model.LeaveDepartmentId = request.DepartmentId;
                model.LeaveLocationId = request.LocationId;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            List<LeaveType> entities = await _leaveService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CloseLeaveRequest(CloseLeaveRequestViewModel model)
        {
            try
            {
                LeaveRequest request = new LeaveRequest();
                if (ModelState.IsValid)
                {
                    request = model.Convert();

                    if (!_validateEndDate(request.ActualLeaveStartDate.Value, request.ActualLeaveEndDate.Value)) { throw new Exception("Error: Invalid Start Date and/or End Date."); }
                    if (!_validateResumptionDate(request.HrResumptionDate.Value, request.ActualLeaveEndDate.Value)) { throw new Exception("Error: Invalid Resumption Date."); }
                    request.ActualLeaveDuration = _leaveService.GetLeaveDuration(request.ActualLeaveStartDate.Value, request.HrResumptionDate.Value);
                    request.ActualLeaveDurationTypeId = (int)DurationTypeEnum.WorkingDays;
                    request.ActualLeaveDurationDescription = $"{request.ActualLeaveDuration} Working Day(s)";

                    if (await _leaveService.CloseLeaveRequestAsync(request, HttpContext.User.Identity.Name))
                    {
                        return RedirectToAction("LeaveRequestsDueResumption");
                    }
                    else { throw new Exception("An error was encountered. Attempt to update Leave Request was not successful."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        #endregion

        #region Teams Leave Plans
        public async Task<IActionResult> MyTeamsLeavePlans(int yr, int mm, string ed)
        {
            MyTeamsLeavePlansListViewModel model = new MyTeamsLeavePlansListViewModel();
            model.yr = yr < 1 ? DateTime.Now.Year : yr;
            model.mm = mm;
            model.ed = ed;
            model.td = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            try
            {
                model.LeavePlanList = await _leaveService.SearchMyTeamsLeavePlansAsync(model.td, model.yr, model.mm, model.ed);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }

            var reports_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(model.td);
            if (reports_entities != null && reports_entities.Count > 0)
            {
                ViewBag.ReportsList = new SelectList(reports_entities, "EmployeeID", "EmployeeName", model.ed);
            }

            return View(model);
        }

        public async Task<IActionResult> MyTeamsLeaveRequests(int yr, int mm, string ed = null, int? st = null)
        {
            MyTeamsLeaveRequestsListViewModel model = new MyTeamsLeaveRequestsListViewModel();
            model.yr = yr;
            model.mm = mm;
            model.ed = ed;
            model.st = st;
            model.td = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

            if (model.yr < 2020)
            {
                model.yr = DateTime.Now.Year;
            }

            try
            {
                model.LeaveRequestList = await _leaveService.SearchMyTeamsLeaveRequestsAsync(model.td, model.yr, model.mm, model.ed, model.st);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }

            var reports_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(model.td);
            if (reports_entities != null && reports_entities.Count > 0)
            {
                ViewBag.ReportsList = new SelectList(reports_entities, "EmployeeID", "EmployeeName", model.ed);
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmResumption(long rd, long sd)
        {
            ConfirmResumptionViewModel model = new ConfirmResumptionViewModel();
           try
            {
                if (rd < 1) { throw new ArgumentNullException("Required parameter Leave Request ID has an invalid value."); }
                var leaveResumption = await _leaveService.GetLeaveResumptionAsync(rd, 0);
                if(leaveResumption != null) { model = model.Convert(leaveResumption); }
                model.LineManagerName = HttpContext.User.Identity.Name;

                var leaveRequest = await _leaveService.GetLeaveRequestAsync(rd);
                if (leaveRequest != null)
                {
                    model.LeaveYear = leaveRequest.LeaveYear;
                    model.LeaveTypeName = leaveRequest.LeaveTypeName;
                    model.ResumptionDateByLineManager = leaveRequest.RequestedResumptionDate;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            model.LeaveSubmissionId = sd;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmResumption(ConfirmResumptionViewModel model)
        {
            if(!ModelState.IsValid) return View(model);
            try
            {
                var leaveResumption = await _leaveService.GetLeaveResumptionAsync(model.LeaveRequestId, model.LeaveResumptionId);
                if (leaveResumption != null) 
                { 
                    leaveResumption.DateRecordedByLineManager = DateTime.Today;
                    leaveResumption.LineManagerName= HttpContext.User.Identity.Name;
                    leaveResumption.NoOfExtraDaysByLineManager = model.NoOfExtraDaysByLineManager;
                    leaveResumption.NoOfUnusedDaysByLineManager = model.NoOfUnusedDaysByLineManager;
                    leaveResumption.ResumptionDateByLineManager = model.ResumptionDateByLineManager;
                    leaveResumption.ReasonByLineManager = model.ReasonByLineManager;
                    leaveResumption.LineManagerApprovesAdjustment = model.LineManagerApprovesAdjustment;

                    if(await _leaveService.ConfirmLeaveResumptionAsync(leaveResumption, model.LeaveSubmissionId))
                    {
                        return RedirectToAction("LeavePendingApproval");
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        //[HttpPost]
        //public async Task<IActionResult> ReturnLeave(ReturnLeaveViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        bool IsReturned = false;
        //        string DocumentTypeDescription;
        //        if (model.DocumentType == "P") { DocumentTypeDescription = "Leave Plan"; }
        //        else { DocumentTypeDescription = "Leave Request"; }
        //        try
        //        {
        //            IsReturned = await _lmsService.UpdateLeaveStatusAsync(model.LeaveId, LeaveStatus.Draft.ToString());
        //            if (IsReturned)
        //            {
        //                LeaveNote note = new LeaveNote();

        //                note.NoteContent = model.ReturnNote;
        //                note.LeaveId = model.LeaveId;
        //                note.TimeAdded = DateTime.Now;
        //                note.FromEmployeeName = model.ApproverName;
        //                if (await _lmsService.AddLeaveNoteAsync(note))
        //                {
        //                    LeaveActivityLog history = new LeaveActivityLog();
        //                    history.ActivityDescription = $"{DocumentTypeDescription} was declined approval by {model.ApproverName}. It was returned to applicant on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT";
        //                    history.ActivityTime = DateTime.Now;
        //                    history.LeaveId = model.LeaveId;
        //                    await _lmsService.AddActivityLogAsync(history);
        //                }
        //                else { model.ViewModelErrorMessage = "Sorry, return note was not added due to an error."; }
        //                model.ViewModelSuccessMessage = "Returned to Applicant successfully!";
        //                model.OperationIsSuccessful = true;
        //            }
        //            else
        //            {
        //                model.ViewModelErrorMessage = "Sorry, an error was encountered. Operation cound not be completed.";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            model.ViewModelErrorMessage = ex.Message;
        //        }
        //    }
        //    return View(model);
        //}

        //public IActionResult ApproveLeave(int id, string nm, string tp)
        //{
        //    ApproveLeaveViewModel model = new ApproveLeaveViewModel();
        //    model.LeaveId = id;
        //    model.ApplicantName = nm;
        //    model.DocumentType = tp;
        //    model.ApproverName = HttpContext.User.Identity.Name;
        //    model.IsApproved = true;
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ApproveLeave(ApproveLeaveViewModel model)
        //{
        //    model.TimeApproved = DateTime.Now;
        //    if (ModelState.IsValid)
        //    {

        //        string DocumentTypeDescription;
        //        if (model.DocumentType == "P") { DocumentTypeDescription = "Leave Plan"; }
        //        else { DocumentTypeDescription = "Leave Request"; }
        //        try
        //        {
        //            if (await _lmsService.ApproveLeaveAsync(model.Convert(), DocumentTypeDescription))
        //            {
        //                model.ViewModelSuccessMessage = "Leave approval completed successfully!";
        //                model.OperationIsSuccessful = true;
        //            }
        //            else
        //            {
        //                model.ViewModelErrorMessage = "Sorry, an error was encountered. Operation cound not be completed.";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            model.ViewModelErrorMessage = ex.Message;
        //        }
        //    }
        //    return View(model);
        //}


        #endregion

        #region Leave Utilities Controller Methods
        public async Task<IActionResult> LeaveNotes(string sp, int yr, long? pd = null, long? rd = null, long? sd = null)
        {
            LeaveNoteListViewModel model = new LeaveNoteListViewModel();
            model.LeavePlanId = pd;
            model.LeaveRequestId = rd;
            model.LeaveYear = yr;
            model.SourcePage = model.src = sp;
            model.LeaveSubmissionId = sd;
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
            }
            else if (model.LeaveRequestId > 0)
            {
                LeaveRequest request = await _leaveService.GetLeaveRequestAsync(model.LeaveRequestId.Value);
                if (request != null)
                {
                    model.ApplicantID = request.LeaveEmployeeId;
                    model.ApplicantName = request.LeaveEmployeeName;
                    model.LoggedInEmployeeName = HttpContext.User.Identity.Name;
                    model.LoggedInEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

                    if (string.IsNullOrWhiteSpace(model.LoggedInEmployeeID))
                    {
                        await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                        return LocalRedirect("/Home/Login");
                    }
                    var request_notes = await _leaveService.GetLeaveRequestNotesAsync(model.LeaveRequestId.Value);
                    if (request_notes != null && request_notes.Count > 0)
                    {
                        model.LeaveNoteList = request_notes.ToList();
                    }
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

        public async Task<IActionResult> LeaveApprovals(string sp, int yr, long? pd = null, long? rd = null)
        {
            LeaveApprovalListViewModel model = new LeaveApprovalListViewModel();
            model.LeavePlanId = pd;
            model.LeaveRequestId = rd;
            model.LeaveYear = yr;
            model.SourcePage = sp;

            var entities = await _leaveService.GetLeaveApprovalsAsync(model.LeavePlanId, model.LeaveRequestId);
            if (entities != null && entities.Count > 0)
            {
                model.LeaveApprovalList = entities.ToList();
            }
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

        #region Leave Documents Controller Methods

        public async Task<IActionResult> LeaveDocuments(long id, string sp = null, long? sd = null)
        {
            LeaveDocumentListViewModel model = new LeaveDocumentListViewModel();
            model.LeaveRequestId = id;
            model.src = model.SourcePage = sp;
            model.SubmissionId = sd;
            var entities = await _leaveService.GetLeaveDocumentsAsync(model.LeaveRequestId);
            if (entities != null)
            {
                model.LeaveDocumentList = entities;
            }

            if (TempData["SuccessMessage"] != null)
            {
                model.ViewModelSuccessMessage = TempData["SuccessMessage"].ToString();
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }

        public async Task<IActionResult> ViewAttachments(long id)
        {
            LeaveDocumentListViewModel model = new LeaveDocumentListViewModel();
            model.LeaveRequestId = id;
            var entities = await _leaveService.GetLeaveDocumentsAsync(model.LeaveRequestId);
            if (entities != null)
            {
                model.LeaveDocumentList = entities;
            }

            if (TempData["SuccessMessage"] != null)
            {
                model.ViewModelSuccessMessage = TempData["SuccessMessage"].ToString();
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }


        public IActionResult UploadDocument(long id)
        {
            LeaveDocumentViewModel model = new LeaveDocumentViewModel();
            model.LeaveRequestId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(LeaveDocumentViewModel model)
        {
            string uploadsFolder = string.Empty;
            string absoluteFilePath = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.MediaFile != null && model.MediaFile.Length > 0)
                    {
                        var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
                        FileInfo fileInfo = new FileInfo(model.MediaFile.FileName);
                        var fileExt = fileInfo.Extension;
                        if (!supportedTypes.Contains(fileExt))
                        {
                            model.ViewModelErrorMessage = "Sorry, invalid file format. Only files of type jpg, jpeg, png, gif and pdf are permitted.";
                            return View(model);
                        }

                        //if(fileInfo.Length / (1048576) > 1)
                        //{
                        //    model.ViewModelErrorMessage = "Sorry, this image is too large. Image size must not exceed 1MB.";
                        //    return View(model);
                        //}

                        uploadsFolder = "uploads/lvm/" + Guid.NewGuid().ToString() + "_" + model.MediaFile.FileName;
                        absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                        using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create))
                        {
                            await model.MediaFile.CopyToAsync(fileStream);
                        }
                    }

                    LeaveDocument document = new LeaveDocument
                    {
                        LeaveRequestId = model.LeaveRequestId,
                        DocumentTitle = model.DocumentTitle,
                        DocumentDescription = model.DocumentDescription,
                        DocumentFullPath = absoluteFilePath,
                        DocumentReferencePath = "/" + uploadsFolder,
                        TimeUploaded = DateTime.UtcNow
                    };

                    if (await _leaveService.AddLeaveDocumentAsync(document))
                    {
                        model.ViewModelSuccessMessage = "New Document was uploaded successfully!";
                    }
                    else
                    {
                        FileInfo file = new FileInfo(absoluteFilePath);
                        if (file.Exists)
                        {
                            if (!file.IsFileOpen())
                            {
                                await Task.Run(() =>
                                {
                                    file.Delete();
                                });
                            }
                        }
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. New Post could not be added.";
                    }
                }
            }
            catch (Exception ex)
            {
                FileInfo file = new FileInfo(absoluteFilePath);
                if (file.Exists)
                {
                    if (!file.IsFileOpen())
                    {
                        await Task.Run(() =>
                        {
                            file.Delete();
                        });
                    }
                }
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocument(long id, long rd)
        {
            long LeaveDocumentId = id;
            long LeaveRequestId = rd;
            string mediaFileFullPath = null;
            try
            {
                LeaveDocument document = await _leaveService.GetLeaveDocumentAsync(LeaveDocumentId);
                if (document != null)
                {
                    mediaFileFullPath = document.DocumentFullPath;
                    if (await _leaveService.DeleteLeaveDocumentAsync(LeaveDocumentId))
                    {
                        if (!string.IsNullOrWhiteSpace(mediaFileFullPath))
                        {
                            FileInfo file = new FileInfo(mediaFileFullPath);
                            if (file.Exists)
                            {
                                if (!file.IsFileOpen())
                                {
                                    await Task.Run(() =>
                                    {
                                        file.Delete();
                                    });
                                }
                            }
                        }
                        TempData["SuccessMessage"] = "Delete operation completed successfully!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("LeaveDocuments", new { id = LeaveRequestId });
        }
        #endregion

        #region Leave Reports Controller Methods

        //Leave Plan Reports
        public async Task<IActionResult> LeavePlanReport(int yr, int mn = 0, int ld = 0, int ud = 0)
        {
            LeavePlanReportViewModel model = new LeavePlanReportViewModel();
            model.LeavePlanList = new List<LeavePlan>();
            model.yr = yr;
            model.mn = mn;
            model.ld = ld;
            model.ud = ud;
            if(yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Leave Plan Report ({model.yr})";

            try
            {
                var entities = await _leaveService.SearchLeavePlansAsync(model.yr,model.mn,null,model.ld,model.ud,null);
                if (entities != null && entities.Count > 0)
                {
                    model.LeavePlanList = entities;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", ld);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", ud);
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }
        public async Task<IActionResult> LeavePlanComplianceReport(int yr, string pm)
        {
            LeavePlanComplianceReportViewModel model = new LeavePlanComplianceReportViewModel();
            model.LeavePlanComplianceList = new List<LeavePlanCompliance>();
            model.yr = yr;
            model.pm = pm;
            if (yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Leave Plan Compliance Report ({model.yr})";

            try
            {
                ReportParameter reportParameter = new ReportParameter();
                switch (pm)
                {
                    case "U":
                        reportParameter = ReportParameter.Unit;
                        break;
                    case "D":
                        reportParameter = ReportParameter.Department; 
                        break;
                    case "L":
                        reportParameter = ReportParameter.Location;
                        break;
                }

                var entities = await _leaveService.GetLeavePlanComplianceAsync(model.yr, reportParameter);
                if (entities != null && entities.Count > 0)
                {
                    model.LeavePlanComplianceList = entities;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }


            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }


        //Leave Request Reports
        public async Task<IActionResult> LeaveRequestReport(int yr, int mn = 0, int ld = 0, int ud = 0)
        {
            LeaveRequestReportViewModel model = new LeaveRequestReportViewModel();
            model.LeaveRequestList = new List<LeaveRequest>();
            model.yr = yr;
            model.mn = mn;
            model.ld = ld;
            model.ud = ud;
            if (yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Employee Leave Report ({model.yr})";

            try
            {
                var entities = await _leaveService.SearchLeaveRequestsAsync(model.yr, model.mn, null, model.ld, model.ud);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveRequestList = entities;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", ld);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", ud);
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }
        public async Task<IActionResult> LeaveRequestComplianceReport(int yr, string pm)
        {
            LeaveRequestComplianceReportViewModel model = new LeaveRequestComplianceReportViewModel();
            model.LeaveRequestComplianceList = new List<LeaveRequestCompliance>();
            model.yr = yr;
            model.pm = pm;
            if (yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Employee Leave Compliance Report ({model.yr})";

            try
            {
                ReportParameter reportParameter = new ReportParameter();
                switch (pm)
                {
                    case "U":
                        reportParameter = ReportParameter.Unit;
                        break;
                    case "D":
                        reportParameter = ReportParameter.Department;
                        break;
                    case "L":
                        reportParameter = ReportParameter.Location;
                        break;
                }

                var entities = await _leaveService.GetLeaveRequestComplianceAsync(model.yr, reportParameter);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveRequestComplianceList = entities;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }


            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }


        //Annual Leave Report
        public async Task<IActionResult> AnnualLeaveSummaryReport(int yr, int ld = 0,  int dd = 0, int ud = 0, string sn = null)
        {
            AnnualLeaveSummaryReportViewModel model = new AnnualLeaveSummaryReportViewModel();
            model.AnnualLeaveSummaryList = new List<AnnualLeaveSummary>();
            model.yr = yr;
            model.ld = ld;
            model.ud = ud;
            model.sn = sn;
            if (yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Annual Leave Summary Report ({model.yr})";

            try
            {
                    var entities = await _leaveService.SearchAnnualLeaveSummaryAsync(model.yr, model.ud, model.dd, model.ld, model.sn);
                    if (entities != null && entities.Count > 0)
                    {
                        model.AnnualLeaveSummaryList = entities;
                    }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", ld);
            }

            var dept_entities = await _globalSettingsService.GetDepartmentsAsync();
            if (dept_entities != null && dept_entities.Count > 0)
            {
                ViewBag.DepartmentList = new SelectList(dept_entities, "DepartmentID", "DepartmentName", dd);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", ud);
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }

        #endregion

        #region Download Controller Methods
        public async Task<FileResult> DownloadLeavePlanReport(int yr, int mn = 0, int ld = 0, int ud = 0)
        {
            LeavePlanReportViewModel model = new LeavePlanReportViewModel();
            model.LeavePlanList = new List<LeavePlan>();
            model.yr = yr;
            model.mn = mn;
            model.ld = ld;
            model.ud = ud;
            if (yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Leave Plan Report ({model.yr})";
            string fileName =  $"{model.ReportHeaderTitle}_vs{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";

            try
            {
                var entities = await _leaveService.SearchLeavePlansAsync(model.yr, model.mn, null, model.ld, model.ud, null);
                if (entities != null && entities.Count > 0)
                {
                    model.LeavePlanList = entities;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return GenerateLeavePlanReportExcel(fileName, model.LeavePlanList);
        }
        public async Task<FileResult> DownloadLeaveRequestReport(int yr, int mn = 0, int ld = 0, int ud = 0)
        {
            LeaveRequestReportViewModel model = new LeaveRequestReportViewModel();
            model.LeaveRequestList = new List<LeaveRequest>();
            model.yr = yr;
            model.mn = mn;
            model.ld = ld;
            model.ud = ud;
            if (yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Employee Leave Report ({model.yr})";
            string fileName = $"{model.ReportHeaderTitle} vs {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";

            try
            {
                var entities = await _leaveService.SearchLeaveRequestsAsync(model.yr, model.mn, null, model.ld, model.ud);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveRequestList = entities;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return GenerateLeaveRequestReportExcel(fileName, model.LeaveRequestList);
        }
        public async Task<IActionResult> DownloadLeavePlanComplianceReport(int yr, string pm)
        {
            LeavePlanComplianceReportViewModel model = new LeavePlanComplianceReportViewModel();
            model.LeavePlanComplianceList = new List<LeavePlanCompliance>();
            model.yr = yr;
            model.pm = pm;
            if (yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Leave Plan Compliance Report ({model.yr})";
            string fileName = $"{model.ReportHeaderTitle}_vs{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";

            try
            {
                ReportParameter reportParameter = new ReportParameter();
                switch (pm)
                {
                    case "U":
                        reportParameter = ReportParameter.Unit;
                        break;
                    case "D":
                        reportParameter = ReportParameter.Department;
                        break;
                    case "L":
                        reportParameter = ReportParameter.Location;
                        break;
                }

                var entities = await _leaveService.GetLeavePlanComplianceAsync(model.yr, reportParameter);
                if (entities != null && entities.Count > 0)
                {
                    model.LeavePlanComplianceList = entities;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return GenerateLeavePlanComplianceReportExcel(fileName, model.LeavePlanComplianceList);
        }
        public async Task<IActionResult> DownloadLeaveRequestComplianceReport(int yr, string pm)
        {
            LeaveRequestComplianceReportViewModel model = new LeaveRequestComplianceReportViewModel();
            model.LeaveRequestComplianceList = new List<LeaveRequestCompliance>();
            model.yr = yr;
            model.pm = pm;
            if (yr < 2020) { model.yr = DateTime.Now.Year; }
            model.ReportHeaderTitle = $"Employee Leave Compliance Report ({model.yr})";
            string fileName = $"{model.ReportHeaderTitle}_vs{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";

            try
            {
                ReportParameter reportParameter = new ReportParameter();
                switch (pm)
                {
                    case "U":
                        reportParameter = ReportParameter.Unit;
                        break;
                    case "D":
                        reportParameter = ReportParameter.Department;
                        break;
                    case "L":
                        reportParameter = ReportParameter.Location;
                        break;
                }

                var entities = await _leaveService.GetLeaveRequestComplianceAsync(model.yr, reportParameter);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveRequestComplianceList = entities;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return GenerateLeaveRequestComplianceReportExcel(fileName, model.LeaveRequestComplianceList);
        }


        #endregion

        #region Controller Download Helper Methods
        private FileResult GenerateLeavePlanReportExcel(string fileName, IEnumerable<LeavePlan> results)
        {
            int row_no = 0;
            DataTable dataTable = new DataTable("results");
            dataTable.Columns.AddRange(new DataColumn[]
            {
new DataColumn("#"),
new DataColumn("Name"),
new DataColumn("Leave Type"),
new DataColumn("Duration"),
new DataColumn("Start Date"),
new DataColumn("End Date"),
new DataColumn("Resumption Date"),
new DataColumn("Unit"),
new DataColumn("Department"),
new DataColumn("Location"),
});
            
            foreach (var result in results)
            {
                row_no++;
                dataTable.Rows.Add(
                    row_no.ToString(),
                    result.LeaveEmployeeName,
                    result.LeaveTypeName,
                    result.LeavePlanDurationDescription,
                    result.LeavePlanStartDate.Value.ToLongDateString(),
                    result.LeavePlanEndDate.Value.ToLongDateString(),
                    result.LeavePlanResumptionDate.Value.ToLongDateString(),
                    result.LeaveUnitName,
                    result.LeaveDepartmentName,
                    result.LeaveLocationName
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
        private FileResult GenerateLeaveRequestReportExcel(string fileName, IEnumerable<LeaveRequest> results)
        {
            int row_no = 0;
            DataTable dataTable = new DataTable("results");
            dataTable.Columns.AddRange(new DataColumn[]
            {
new DataColumn("#"),
new DataColumn("Name"),
new DataColumn("Leave Type"),
new DataColumn("Duration"),
new DataColumn("Start Date"),
new DataColumn("End Date"),
new DataColumn("Resumption Date"),
new DataColumn("Unit"),
new DataColumn("Department"),
new DataColumn("Location"),
new DataColumn("Allowance"),
});

            foreach (var result in results)
            {
                row_no++;
                dataTable.Rows.Add(
                    row_no.ToString(),
                    result.LeaveEmployeeName,
                    result.LeaveTypeName,
                    result.ActualLeaveDurationDescription,
                    result.ActualLeaveStartDate.Value.ToLongDateString(),
                    result.ActualLeaveEndDate.Value.ToLongDateString(),
                    result.HrResumptionDate.Value.ToLongDateString(),
                    result.UnitName,
                    result.DepartmentName,
                    result.LocationName,
                    result.RequestLeaveAllowance ? "Yes" : "No"
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

        private FileResult GenerateLeavePlanComplianceReportExcel(string fileName, IEnumerable<LeavePlanCompliance> results)
        {
            int row_no = 0;
            DataTable dataTable = new DataTable("results");
            dataTable.Columns.AddRange(new DataColumn[]
            {
new DataColumn("#"),
new DataColumn("Name"),
new DataColumn("Total Staff"),
new DataColumn("Staff With Leave Plans"),
new DataColumn("Staff Without Leave Plans"),
new DataColumn("% Compliance"),
});

            foreach (var result in results)
            {
                row_no++;
                string Name = string.Empty;
                if (!string.IsNullOrWhiteSpace(result.UnitName)) { Name = result.UnitName; }
                else if (!string.IsNullOrWhiteSpace(result.DepartmentName)){ Name = result.DepartmentName; }
                else if(!string.IsNullOrWhiteSpace(result.LocationName)){ Name = result.LocationName; }

                dataTable.Rows.Add(
                    row_no.ToString(),
                    Name,
                    result.TotalNumberOfStaff,
                    result.NumberWithLeavePlans,
                    result.NumberWithoutLeavePlans,
                    result.PercentageCompliance.ToString("F2")
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

        private FileResult GenerateLeaveRequestComplianceReportExcel(string fileName, IEnumerable<LeaveRequestCompliance> results)
        {
            int row_no = 0;
            DataTable dataTable = new DataTable("results");
            dataTable.Columns.AddRange(new DataColumn[]
            {
new DataColumn("#"),
new DataColumn("Name"),
new DataColumn("Total Staff"),
new DataColumn("Staff With Leave Plans"),
new DataColumn("Staff Without Leave Plans"),
new DataColumn("% Compliance"),
});

            foreach (var result in results)
            {
                row_no++;
                string Name = string.Empty;
                if (!string.IsNullOrWhiteSpace(result.UnitName)) { Name = result.UnitName; }
                else if (!string.IsNullOrWhiteSpace(result.DepartmentName)) { Name = result.DepartmentName; }
                else if (!string.IsNullOrWhiteSpace(result.LocationName)) { Name = result.LocationName; }

                dataTable.Rows.Add(
                    row_no.ToString(),
                    Name,
                    result.TotalNumberOfStaff,
                    result.NumberWithLeaveRequests,
                    result.NumberWithoutLeaveRequests,
                    result.PercentageCompliance.ToString("F2")
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
        public string SaveLeaveNote(string nm, string msg, long? pd = null, long? rd = null)
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
        public string ApproveLeaveRequest(long rd, long sd)
        {
            LeaveSubmission leaveSubmission = new LeaveSubmission();
            LeaveApproval leaveApproval = new LeaveApproval();
            try
            {
                leaveSubmission = _leaveService.GetLeaveSubmissionByIdAsync(sd).Result;
                if (leaveSubmission == null) { throw new Exception("Error! This submission record was not found. Please try again."); }
                leaveApproval.ApproverName = HttpContext.User.Identity.Name;
                leaveApproval.ApproverRole = leaveSubmission.ToEmployeeRole;
                leaveApproval.IsApproved = true;
                leaveApproval.LeaveRequestId = rd;
                leaveApproval.TimeApproved = DateTime.Now;
                leaveApproval.ApplicantName = leaveSubmission.FromEmployeeName;

                bool IsApproved = _leaveService.ApproveLeaveAsync(leaveApproval, leaveSubmission, DocumentType.LeaveRequest).Result;
                if (!IsApproved)
                {
                    return "An error was encountered. Operation failed!";
                }
                else
                {
                    Employee sender = new Employee();
                    sender = _ermService.GetEmployeeByNameAsync(leaveSubmission.FromEmployeeName).Result;
                    Employee approver = new Employee();
                    approver = _ermService.GetEmployeeByNameAsync(leaveSubmission.ToEmployeeName).Result;

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

                    bool messageSent = _baseModelService.SendMessageAsync(message).Result;
                    if (!string.IsNullOrWhiteSpace(recipientEmailCopy.RecipientEmail))
                    {
                        // approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "approved";
        }
        public string ConfirmLeaveRequest(long rd)
        {
            long LeaveRequestId = rd;
            string ConfirmedBy = HttpContext.User.Identity.Name;
            DateTime ConfirmedTime = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(ConfirmedBy) || string.IsNullOrWhiteSpace(ConfirmedBy)) { return "parameter"; }
            if (LeaveRequestId < 1) { return "parameter"; }
            try
            {
                if (_leaveService.HrConfirmLeaveRequestAsync(LeaveRequestId, ConfirmedBy, ConfirmedTime).Result)
                {
                    return "confirmed";
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
        public string DeleteLeaveAdjustment(long id)
        {
            if (id < 1) { return "parameter"; }
            string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_leaveService.DeleteLeaveAdjustmentAsync(id, actionBy).Result)
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

        public string UnflagLeavePlan(long pd)
        {
            long LeavePlanId = pd;
            string UnflaggedBy = HttpContext.User.Identity.Name;
            DateTime UnflaggedTime = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(UnflaggedBy)) { return "parameter"; }
            if (LeavePlanId < 1) { return "parameter"; }
            try
            {
                if (_leaveService.UpdateLeavePlanFlagAsync(LeavePlanId, false, null, UnflaggedBy).Result)
                {
                    return "unflagged";
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
