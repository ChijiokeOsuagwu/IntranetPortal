using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.LMS.Models;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Helpers;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace IntranetPortal.Areas.LMS.Controllers
{
    [Area("LMS")]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IBaseModelService _baseModelService;
        private readonly ILmsService _lmsService;
        private readonly IErmService _ermService;

        public HomeController(IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration, IBaseModelService baseModelService,
            ILmsService lmsService, IErmService ermService)
        {
            _configuration = configuration;
            _baseModelService = baseModelService;
            _lmsService = lmsService;
            _ermService = ermService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LeaveNotes(long id, string sp, int yr)
        {
            LeaveNoteListViewModel model = new LeaveNoteListViewModel();
            model.LeaveID = id;
            model.LeaveYear = yr;
            model.SourcePage = sp;
            if (id > 0)
            {
                EmployeeLeave e = await _lmsService.GetEmployeeLeaveAsync(id);
                if (e != null)
                {
                    model.ApplicantID = e.EmployeeId;
                    model.ApplicantName = e.EmployeeFullName;
                    model.LoggedInEmployeeName = HttpContext.User.Identity.Name;
                    model.LoggedInEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

                    if (string.IsNullOrWhiteSpace(model.LoggedInEmployeeID))
                    {
                        await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                        return LocalRedirect("/Home/Login");
                    }
                }
                var entities = await _lmsService.GetLeaveNotesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveNoteList = entities.ToList();
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LeaveActivities(long id)
        {
            LeaveActivitiesViewModel model = new LeaveActivitiesViewModel();
            model.LeaveId = id;
            if (id > 0)
            {
                var entities = await _lmsService.GetLeaveActivitiesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveActivityList = entities.ToList();
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LeaveApprovals(long id, string sp, int yr)
        {
            LeaveApprovalListViewModel model = new LeaveApprovalListViewModel();
            model.LeaveID = id;
            model.LeaveYear = yr;
            model.SourcePage = sp;
            if (id > 0)
            {
                var entities = await _lmsService.GetLeaveApprovalsAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.LeaveApprovalList = entities.ToList();
                }
            }
            return View(model);
        }


        #region Leave Plan Controller Action Methods
        public async Task<IActionResult> MyLeavePlans(int yr)
        {
            LeavePlanListViewModel model = new LeavePlanListViewModel();
            if (yr < 1)
            {
                model.yr = DateTime.Now.Year;
            }
            else { model.yr = yr; }

            model.nm = HttpContext.User.Identity.Name;
            model.ei = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            try
            {
                model.LeavePlanList = await _lmsService.GetEmployeeLeavesAsync(model.ei, model.yr, true);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }

        public async Task<IActionResult> NewLeavePlan()
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            model.LeaveStartDate = DateTime.Today;
            model.LeaveEndDate = null;
            model.LeaveYear = DateTime.Today.Year;
            model.LeaveStatus = LeaveStatus.Draft.ToString();
            model.IsPlan = true;
            //model.EmployeeFullName = HttpContext.User.Identity.Name;
            model.EmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            Employee e = await _ermService.GetEmployeeByIdAsync(model.EmployeeId);
            if (e == null || string.IsNullOrWhiteSpace(e.FullName)) { throw new Exception("Sorry, no record was found for this staff."); }
            else
            {
                model.EmployeeId = e.EmployeeID;
                model.EmployeeFullName = e.FullName;
                model.DepartmentId = e.DepartmentID ?? 0;
                model.UnitId = e.UnitID ?? 0;
                model.LocationId = e.LocationID ?? 0;
            }
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
                    d = model.Convert();
                    long LeaveId = await _lmsService.CreateLeaveAsync(d);
                    if (LeaveId > 0)
                    {
                        return RedirectToAction("MyLeavePlans", new { yr = model.LeaveStartDate.Year });
                    }
                    else { throw new Exception("An error was encountered. New Leave Plan could not be added."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> EditLeavePlan(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            var leavePlan = await _lmsService.GetEmployeeLeaveAsync(id);
            if (leavePlan != null)
            {
                model = model.Extract(leavePlan);
            }
            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name"); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeavePlan(LeavePlanViewModel model)
        {
            try
            {
                EmployeeLeave d = new EmployeeLeave();
                if (ModelState.IsValid)
                {
                    d = model.Convert();
                    bool IsUpdated = await _lmsService.UpdateLeaveAsync(d);
                    if (IsUpdated)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Leave Request was updated successfully!";
                    }
                    else { throw new Exception("An error was encountered. Leave Request could not be updated."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> LeavePlanDetails(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            var leavePlan = await _lmsService.GetEmployeeLeaveAsync(id);
            if (leavePlan != null)
            {
                model = model.Extract(leavePlan);
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteLeavePlan(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            var leavePlan = await _lmsService.GetEmployeeLeaveAsync(id);
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
                if (model.Id > 0)
                {
                    bool IsDeleted = await _lmsService.DeleteLeaveAsync(model.Id);
                    if (IsDeleted)
                    {
                        return RedirectToAction("MyLeavePlans", new { yr = model.LeaveStartDate.Year });
                    }
                    else { throw new Exception("An error was encountered. New Leave Plan could not be added."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> SubmitLeavePlan(long id)
        {
            SubmitLeaveViewModel model = new SubmitLeaveViewModel();
            try
            {
                model.LeaveId = id;
                model.FromEmployeeName = HttpContext.User.Identity.Name;
                model.FromEmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                var entities = await _ermService.GetActiveEmployeeReportLinesByEmployeeIdAsync(model.FromEmployeeId);
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
                leaveSubmission.TimeSubmitted = DateTime.UtcNow;
                try
                {
                    bool IsSubmitted = await _lmsService.SubmitLeaveAsync(leaveSubmission, "Leave Plan");
                    if (IsSubmitted)
                    {
                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByIdAsync(model.FromEmployeeId);
                        Employee approver = new Employee();
                        approver = await _ermService.GetEmployeeByIdAsync(model.ToEmployeeId);

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
                            approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
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

        #endregion

        #region My Team's Leave Plan Controller Action Methods
        public async Task<IActionResult> MyTeamsLeavePlans(int yr, int mm, string ed, string st)
        {
            MyTeamsLeavePlansListViewModel model = new MyTeamsLeavePlansListViewModel();
            model.yr = yr < 1 ? DateTime.Now.Year : yr;
            model.mm = mm;
            model.ed = ed;
            model.st = st;
            model.td = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            try
            {
                model.LeavePlanList = await _lmsService.SearchMyTeamsEmployeeLeavesAsync(model.td, model.yr, model.mm, model.ed, model.st, true);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }

            var reports_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(model.td);
            if (reports_entities != null && reports_entities.Count > 0)
            {
                ViewBag.ReportsList = new SelectList(reports_entities, "EmployeeID", "EmployeeName", model.ed);
            }

            return View(model);
        }

        public IActionResult ReturnLeave(int id, string tp)
        {
            ReturnLeaveViewModel model = new ReturnLeaveViewModel();
            model.LeaveId = id;
            model.DocumentType = tp;
            model.ApproverName = HttpContext.User.Identity.Name;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnLeave(ReturnLeaveViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool IsReturned = false;
                string DocumentTypeDescription;
                if (model.DocumentType == "P") { DocumentTypeDescription = "Leave Plan"; }
                else { DocumentTypeDescription = "Leave Request"; }
                try
                {
                    IsReturned = await _lmsService.UpdateLeaveStatusAsync(model.LeaveId, LeaveStatus.Draft.ToString());
                    if (IsReturned)
                    {
                        LeaveNote note = new LeaveNote();

                        note.NoteContent = model.ReturnNote;
                        note.LeaveId = model.LeaveId;
                        note.TimeAdded = DateTime.Now;
                        note.FromEmployeeName = model.ApproverName;
                        if (await _lmsService.AddLeaveNoteAsync(note))
                        {
                            LeaveActivityLog history = new LeaveActivityLog();
                            history.ActivityDescription = $"{DocumentTypeDescription} was declined approval by {model.ApproverName}. It was returned to applicant on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT";
                            history.ActivityTime = DateTime.Now;
                            history.LeaveId = model.LeaveId;
                            await _lmsService.AddActivityLogAsync(history);
                        }
                        else { model.ViewModelErrorMessage = "Sorry, return note was not added due to an error."; }
                        model.ViewModelSuccessMessage = "Returned to Applicant successfully!";
                        model.OperationIsSuccessful = true;
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Operation cound not be completed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public IActionResult ApproveLeave(int id, string nm, string tp)
        {
            ApproveLeaveViewModel model = new ApproveLeaveViewModel();
            model.LeaveId = id;
            model.ApplicantName = nm;
            model.DocumentType = tp;
            model.ApproverName = HttpContext.User.Identity.Name;
            model.IsApproved = true;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveLeave(ApproveLeaveViewModel model)
        {
            model.TimeApproved = DateTime.Now;
            if (ModelState.IsValid)
            {

                string DocumentTypeDescription;
                if (model.DocumentType == "P") { DocumentTypeDescription = "Leave Plan"; }
                else { DocumentTypeDescription = "Leave Request"; }
                try
                {
                    if (await _lmsService.ApproveLeaveAsync(model.Convert(), DocumentTypeDescription))
                    {
                        model.ViewModelSuccessMessage = "Leave approval completed successfully!";
                        model.OperationIsSuccessful = true;
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Operation cound not be completed.";
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

        #region HR Dashboard Controller Action Methods
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> LeavePlans(int yr, int mm, string sn, int ld, int ud, string st)
        {
            LeavePlanReportListViewModel model = new LeavePlanReportListViewModel();
            model.yr = yr < 1 ? DateTime.Now.Year : yr;
            model.mm = mm;
            model.sn = sn;
            model.ld = ld;
            model.ud = ud;
            model.st = st;
            try
            {
                model.LeavePlanList = await _lmsService.SearchAllEmployeeLeavesAsync(model.yr, model.mm, model.sn, model.st, true);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }


        #endregion

        #region Leave Request Controller Action Methods
        public async Task<IActionResult> MyLeaveRequests(int yr)
        {
            LeavePlanListViewModel model = new LeavePlanListViewModel();
            if (yr < 1)
            {
                model.yr = DateTime.Now.Year;
            }
            else { model.yr = yr; }

            model.nm = HttpContext.User.Identity.Name;
            model.ei = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            try
            {
                model.LeaveRequestList = await _lmsService.GetEmployeeLeavesAsync(model.ei, model.yr, false);
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            return View(model);
        }
        public async Task<IActionResult> NewLeaveRequest(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            if (id == 0)
            {
                model.LeaveStartDate = DateTime.Today;
                model.LeaveEndDate = null;
                model.LeaveYear = DateTime.Today.Year;
                //model.EmployeeFullName = HttpContext.User.Identity.Name;
                model.EmployeeId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                Employee e = await _ermService.GetEmployeeByIdAsync(model.EmployeeId);
                if (e == null || string.IsNullOrWhiteSpace(e.FullName)) { throw new Exception("Sorry, no record was found for this staff."); }
                else
                {
                    model.EmployeeId = e.EmployeeID;
                    model.EmployeeFullName = e.FullName;
                    model.DepartmentId = e.DepartmentID ?? 0;
                    model.UnitId = e.UnitID ?? 0;
                    model.LocationId = e.LocationID ?? 0;
                }
            }
            else
            {
                var leavePlan = await _lmsService.GetEmployeeLeaveAsync(id);
                if (leavePlan != null)
                {
                    model = model.Extract(leavePlan);
                }

            }
            model.IsPlan = false;
            model.LeaveStatus = LeaveStatus.New.ToString();

            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name"); }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NewLeaveRequest(LeavePlanViewModel model)
        {
            try
            {
                EmployeeLeave d = new EmployeeLeave();
                if (ModelState.IsValid)
                {
                    d = model.Convert();
                    long LeaveId = await _lmsService.CreateLeaveAsync(d);
                    if (LeaveId > 0)
                    {
                        return RedirectToAction("MyLeaveRequests", new { yr = model.LeaveStartDate.Year });
                    }
                    else { throw new Exception("An error was encountered. New Leave Request could not be added."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> EditLeaveRequest(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            var leavePlan = await _lmsService.GetEmployeeLeaveAsync(id);
            if (leavePlan != null)
            {
                model = model.Extract(leavePlan);
            }
            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name"); }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeaveRequest(LeavePlanViewModel model)
        {
            try
            {
                EmployeeLeave d = new EmployeeLeave();
                if (ModelState.IsValid)
                {
                    d = model.Convert();
                    bool IsUpdated = await _lmsService.UpdateLeaveAsync(d);
                    if (IsUpdated)
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Leave Request was updated successfully!";
                    }
                    else { throw new Exception("An error was encountered. Leave Request could not be updated."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex) { model.ViewModelErrorMessage = ex.Message; }
            List<LeaveType> entities = await _lmsService.GetLeaveTypes();
            if (entities != null) { ViewBag.LeaveTypeCodeList = new SelectList(entities, "Code", "Name", model.LeaveTypeCode); }
            return View(model);
        }

        public async Task<IActionResult> DeleteLeaveRequest(long id)
        {
            LeavePlanViewModel model = new LeavePlanViewModel();
            var leavePlan = await _lmsService.GetEmployeeLeaveAsync(id);
            if (leavePlan != null)
            {
                model = model.Extract(leavePlan);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLeaveRequest(LeavePlanViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    bool IsDeleted = await _lmsService.DeleteLeaveAsync(model.Id);
                    if (IsDeleted)
                    {
                        return RedirectToAction("MyLeavePlans", new { yr = model.LeaveStartDate.Year });
                    }
                    else { throw new Exception("An error was encountered. New Leave Plan could not be added."); }
                }
                else { throw new Exception("Sorry, some key form parameters are missing."); }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> LeaveAttachments(long id)
        {
            LeaveAttachmentsListViewModel model = new LeaveAttachmentsListViewModel();
            model.LeaveId = id;
            if (id > 0)
            {
                var entities = await _lmsService.GetLeaveDocumentsAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.AttachmentList = entities;
                }
            }
            if (TempData["DeleteErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["DeleteErrorMessage"].ToString();
            }
            if (TempData["DeleteSuccessMessage"] != null)
            {
                model.ViewModelSuccessMessage = TempData["DeleteSuccessMessage"].ToString();
            }
            return View(model);
        }

        public IActionResult NewAttachment(long id)
        {
            LeaveAttachmentViewModel model = new LeaveAttachmentViewModel();
            model.LeaveId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewAttachment(LeaveAttachmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = null;
                string absoluteFilePath = null;

                if (model.UploadedFile == null || model.UploadedFile.Length < 1)
                {
                    model.ViewModelErrorMessage = "Sorry, no attachment was found. Kindly attach a file and try again. ";
                    return View(model);
                }
                else
                {
                    var supportedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".gif" };
                    FileInfo fileInfo = new FileInfo(model.UploadedFile.FileName);
                    var fileExt = fileInfo.Extension;
                    if (!supportedTypes.Contains(fileExt))
                    {
                        model.ViewModelErrorMessage = "Sorry, invalid file format. Only files of type pdf, jpg, jpeg, png, gif are permitted.";
                        return View(model);
                    }

                    //if(fileInfo.Length / (1048576) > 1)
                    //{
                    //    model.ViewModelErrorMessage = "Sorry, this image is too large. Image size must not exceed 1MB.";
                    //    return View(model);
                    //}

                    uploadsFolder = "uploads/lms/" + Guid.NewGuid().ToString() + "_" + model.UploadedFile.FileName;
                    absoluteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolder);
                    using var fileStream = new FileStream(absoluteFilePath, FileMode.Create);
                    await model.UploadedFile.CopyToAsync(fileStream);

                    LeaveDocument doc = new LeaveDocument
                    {
                        DocumentTitle = model.DocumentTitle,
                        DocumentDescription = model.DocumentDescription,
                        DocumentFormat = model.DocumentFormat,
                        DocumentLink = uploadsFolder,
                        LeaveId = model.LeaveId,
                        UploadedBy = HttpContext.User.Identity.Name,
                        UploadedTime = DateTime.UtcNow,
                    };

                    if (await _lmsService.AddLeaveDocumentAsync(doc))
                    {
                        return RedirectToAction("LeaveAttachments", new { id = model.LeaveId });
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
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Attachment could not be uploaded.";
                    }
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Sorry, some required values are missing.";
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttachement(long id, long ld)
        {
            if (id < 1)
            {
                TempData["DeleteErrorMessage"] = "Sorry, required value is missing.";
            }
            else
            {
                LeaveDocument doc = await _lmsService.GetLeaveDocumentAsync(id);
                string filePath = string.Empty;
                if (!string.IsNullOrEmpty(doc.DocumentLink))
                {
                    filePath = Path.Combine(_webHostEnvironment.WebRootPath, doc.DocumentLink);
                }
                FileInfo file = new FileInfo(filePath);
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
                var result = await _lmsService.DeleteLeaveDocumentAsync(id);
                if (!result)
                {
                    TempData["DeleteErrorMessage"] = "Sorry, an error was encountered. ";
                }
                else
                {
                    TempData["DeleteSuccessMessage"] = "Attachment deleted successfully!";
                }
            }
            return RedirectToAction("LeaveAttachments", new { id = ld });
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
                    leaveEndDate = _lmsService.GenerateLeaveEndDate(convertedStartDate, dt, dr).ToString("yyyy-MM-dd");
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
        public string SaveLeaveNote(long id, string nm, string msg)
        {
            LeaveNote note = new LeaveNote()
            {
                TimeAdded = DateTime.Now,
                FromEmployeeName = nm,
                LeaveId = id,
                NoteContent = msg
            };

            if (id < 1 || string.IsNullOrWhiteSpace(nm) || string.IsNullOrWhiteSpace(msg)) { return "parameter"; }
            //string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_lmsService.AddLeaveNoteAsync(note).Result)
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

        #endregion
    }
}