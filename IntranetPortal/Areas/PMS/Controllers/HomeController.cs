using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Areas.PMS.Models;
using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Helpers;
using IntranetPortal.Models;

namespace IntranetPortal.Areas.PMS.Controllers
{
    [Area("PMS")]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        private readonly ISecurityService _securityService;
        private readonly IPerformanceService _performanceService;

        public HomeController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings, IErmService ermService,
                                    IBaseModelService baseModelService, IPerformanceService performanceService,
                                    ISecurityService securityService)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _ermService = ermService;
            _baseModelService = baseModelService;
            _securityService = securityService;
            _performanceService = performanceService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Enquiry and Reports
        public IActionResult Reports()
        {
            return View();
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> AppraisalEnquiry(int id, int? dd = null, int? ud = null, string nm = null)
        {
            PmsEnquiryViewModel model = new PmsEnquiryViewModel();
            model.ResultSummaryList = new List<ResultSummary>();
            model.id = id;
            model.dd = dd ?? 0;
            model.ud = ud ?? 0;
            model.nm = nm;

            try
            {
                if (id > 0)
                {
                    if (!string.IsNullOrWhiteSpace(nm))
                    {
                        var entities = await _performanceService.GetResultSummaryByReviewSessionIdAndAppraiseeNameAsync(id, nm);
                        if (entities != null && entities.Count > 0)
                        {
                            model.ResultSummaryList = entities;
                        }
                    }
                    else if (ud != null && ud > 0)
                    {
                        var entities = await _performanceService.GetResultSummaryByReviewSessionIdAndUnitCodeAsync(id, ud.Value);
                        if (entities != null && entities.Count > 0)
                        {
                            model.ResultSummaryList = entities;
                        }
                    }
                    else if (dd != null && dd > 0)
                    {
                        var entities = await _performanceService.GetResultSummaryByReviewSessionIdAndDepartmentCodeAsync(id, dd.Value);
                        if (entities != null && entities.Count > 0)
                        {
                            model.ResultSummaryList = entities;
                        }
                    }
                    else
                    {
                        var entities = await _performanceService.GetResultSummaryByReviewSessionIdAsync(id);
                        if (entities != null && entities.Count > 0)
                        {
                            model.ResultSummaryList = entities;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
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

            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ResultReport(int id, int? ld = null, int? dd = null, int? ud = null)
        {
            ResultReportViewModel model = new ResultReportViewModel();
            model.ResultDetailList = new List<ResultDetail>();
            model.id = id;
            model.ld = ld ?? 0;
            model.dd = dd ?? 0;
            model.ud = ud ?? 0;

            model.ResultDetailList = new List<ResultDetail>();
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetPrincipalResultDetailAsync(id, ld, dd, ud);
                    if (entities != null && entities.Count > 0)
                    {
                        model.ResultDetailList = entities;
                        ResultDetail resultDetail = new ResultDetail();
                        resultDetail = entities.FirstOrDefault();
                        model.ReviewSessionDescription = resultDetail.ReviewSessionName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
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

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> RejectedEvaluations(int id, int? ld = null)
        {
            ResultReportViewModel model = new ResultReportViewModel();
            model.ResultDetailList = new List<ResultDetail>();
            model.id = id;
            model.ld = ld ?? 0;

            model.ResultDetailList = new List<ResultDetail>();
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetRejectedPrincipalResultDetailAsync(id, ld);
                    if (entities != null && entities.Count > 0)
                    {
                        model.ResultDetailList = entities;
                        ResultDetail resultDetail = new ResultDetail();
                        resultDetail = entities.FirstOrDefault();
                        model.ReviewSessionDescription = resultDetail.ReviewSessionName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
            }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", ld);
            }

            if (TempData["ErrorMessage"] != null)
            {
                model.ViewModelErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ProgressStatus(int id, int? td = null, int? ld = null, int? ud = null, string nm = null)
        {
            ProgressStatusReportViewModel model = new ProgressStatusReportViewModel();
            model.ReviewHeaderList = new List<ReviewHeader>();
            model.id = id;
            model.td = td ?? 0;
            model.ld = ld ?? 0;
            model.nm = nm;
            model.ud = ud ?? 0;

            model.ReviewHeaderList = new List<ReviewHeader>();
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetReviewHeadersAsync(id, td, ld, ud, nm);
                    if (entities != null && entities.Count > 0)
                    {
                        model.ReviewHeaderList = entities;
                        model.RecordCount = entities.Count;
                        model.ReviewSessionDescription = entities.FirstOrDefault().ReviewSessionName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
            }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", ld);
            }

            var stage_entities = await _performanceService.GetReviewStagesAsync();
            if (stage_entities != null && stage_entities.Count > 0)
            {
                ViewBag.ReviewStageList = new SelectList(stage_entities, "ReviewStageId", "ReviewStageName", td);
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

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> AppraisalNonParticipants(int id, int? ld = null, int? ud = null)
        {
            AppraisalNonParticipantsViewModel model = new AppraisalNonParticipantsViewModel();
            model.EmployeesList = new List<Employee>();
            model.id = id;
            model.ld = ld ?? 0;
            model.ud = ud ?? 0;
            try
            {
               model.EmployeesList = await _performanceService.GetAppraisalNonParticipants(model.id, model.ld, model.ud);
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
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

        #endregion

        #region Download Action Methods
        public async Task<FileResult> DownloadResultReport(int id, int? ld = null, int? dd = null, int? ud = null)
        {
            List<ResultDetail> ResultDetailList = new List<ResultDetail>();
            string ReviewSessionDescription = string.Empty;
            string fileName = string.Empty;
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetPrincipalResultDetailAsync(id, ld, dd, ud);
                    if (entities != null && entities.Count > 0)
                    {
                        ResultDetailList = entities;
                        ResultDetail resultDetail = new ResultDetail();
                        resultDetail = entities.FirstOrDefault();
                        ReviewSessionDescription = resultDetail.ReviewSessionName;
                        fileName = $"Appraisal Report {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
                //TempData["ErrorMessage"] = ex.Message;
                //return RedirectToAction("ResultReport", new { id, lc, dc, uc });
            }
            return GenerateResultReportExcel(fileName, ResultDetailList);
        }

        public async Task<FileResult> DownloadProgressStatusReport(int id, int? td = null, int? ld = null, int? ud = null, string nm = null)
        {
            List<ReviewHeader> ReviewHeaderList = new List<ReviewHeader>();
            string ReviewSessionDescription = string.Empty;
            int RecordCount = 0;
            string fileName = string.Empty;
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetReviewHeadersAsync(id, td, ld, ud, nm);
                    if (entities != null && entities.Count > 0)
                    {
                        ReviewHeaderList = entities;
                        RecordCount = entities.Count;
                        ReviewSessionDescription = entities.FirstOrDefault().ReviewSessionName;
                        fileName = $"Appraisal Progress Status Report {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return GenerateProgressStatusReportExcel(fileName, ReviewHeaderList);
        }


        public async Task<FileResult> DownloadAppraisalNonParticipantsReport(int id, int? ld = null, int? ud = null)
        {
            List<Employee> EmployeesList = new List<Employee>();
            int RecordCount = 0;
            string fileName = string.Empty;
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetAppraisalNonParticipants(id, ld, ud);

                    if (entities != null && entities.Count > 0)
                    {
                        EmployeesList = entities;
                        RecordCount = entities.Count;
                        fileName = $"Appraisal Non Participants Report {DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}.xlsx";
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return GenerateAppraisalNonParticipantsReportExcel(fileName, EmployeesList);
        }


        #endregion

        #region Utilities Action Methods

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> EmployeeSearch(int id, int? ud = null, string sn = null)
        {
            PmsEmployeeSearchViewModel model = new PmsEmployeeSearchViewModel();
            model.id = id;
            model.ud = ud;
            model.sn = sn;

            if (!string.IsNullOrWhiteSpace(sn))
            {
                var employees = await _ermService.SearchEmployeesByNameAsync(sn);
                if (employees != null)
                {
                    model.EmployeesList = employees;
                }
            }
            else if (ud != null && ud > 0)
            {
                var employees = await _ermService.GetEmployeesByUnitIDAsync(ud.Value);
                if (employees != null)
                {
                    model.EmployeesList = employees;
                }
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", model.id);
            }

            var unit_entities = await _globalSettingsService.GetUnitsAsync();
            if (unit_entities != null && unit_entities.Count > 0)
            {
                ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", model.ud);
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ChangeAppraiser(int id, int tp, int? ud = null, string ad = null, string an = null)
        {
            ChangeAppraiserViewModel model = new ChangeAppraiserViewModel();
            model.AppraiseeID = ad;
            model.ChangeType = (AppraiserChangeType)tp;
            model.sn = string.Empty;
            model.ReviewSessionID = id;

            if (model.ChangeType == AppraiserChangeType.Unit)
            {
                var unit_entities = await _globalSettingsService.GetUnitsAsync();
                if (unit_entities != null && unit_entities.Count > 0)
                {
                    ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", ud);
                }
            }
            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", model.ReviewSessionID);
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> ChangeAppraiser(ChangeAppraiserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee PrincipalAppraiser = await _ermService.GetEmployeeByNameAsync(model.sn);
                if (PrincipalAppraiser != null)
                {
                    if (await _performanceService.UpdatePrincipalAppraiserAsync((int)model.ChangeType, model.ReviewSessionID, PrincipalAppraiser.EmployeeID, model.AppraiseeID, model.UnitID))
                    {
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Principal Appraiser was updated successfully!";
                    }
                }
            }

            if (model.ChangeType == AppraiserChangeType.Unit)
            {
                var unit_entities = await _globalSettingsService.GetUnitsAsync();
                if (unit_entities != null && unit_entities.Count > 0)
                {
                    ViewBag.UnitList = new SelectList(unit_entities, "UnitID", "UnitName", model.UnitID);
                }
            }
            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", model.ReviewSessionID);
            }

            return View(model);
        }


        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> CheckEntries(int id, int? td = null, int? ld = null, int? ud = null, string nm = null)
        {
            ProgressStatusReportViewModel model = new ProgressStatusReportViewModel();
            model.ReviewHeaderList = new List<ReviewHeader>();
            model.id = id;
            model.td = td ?? 0;
            model.ld = ld ?? 0;
            model.nm = nm;
            model.ud = ud ?? 0;

            model.ReviewHeaderList = new List<ReviewHeader>();
            try
            {
                if (id > 0)
                {
                    var entities = await _performanceService.GetReviewHeadersAsync(id, td, ld, ud, nm);
                    if (entities != null && entities.Count > 0)
                    {
                        model.ReviewHeaderList = entities;
                        model.RecordCount = entities.Count;
                        model.ReviewSessionDescription = entities.FirstOrDefault().ReviewSessionName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var sessions_entities = await _performanceService.GetReviewSessionsAsync();
            if (sessions_entities != null && sessions_entities.Count > 0)
            {
                ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
            }

            var loc_entities = await _globalSettingsService.GetAllLocationsAsync();
            if (loc_entities != null && loc_entities.Count > 0)
            {
                ViewBag.LocationList = new SelectList(loc_entities, "LocationID", "LocationName", ld);
            }

            var stage_entities = await _performanceService.GetReviewStagesAsync();
            if (stage_entities != null && stage_entities.Count > 0)
            {
                ViewBag.ReviewStageList = new SelectList(stage_entities, "ReviewStageId", "ReviewStageName", td);
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

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> ReturnforCorrection(int id)
        {
            ReturnforCorrectionViewModel model = new ReturnforCorrectionViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                model.ReviewHeaderID = id;
                if (id > 0)
                {
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(model.ReviewHeaderID);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.CurrentStageID = reviewHeader.ReviewStageId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> ReturnforCorrection(ReturnforCorrectionViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewHeader reviewHeader = new ReviewHeader();
                try
                {
                    bool reviewHeaderIsUpdated = false;
                    // Check if this submission has already been actioned by the recipient.
                    var reviewHeader_entity = await _performanceService.GetReviewHeaderAsync(model.ReviewHeaderID);
                    if (reviewHeader_entity != null)
                    {
                        reviewHeader = reviewHeader_entity;
                        if (model.ReturnToStageID == 1)
                        {
                            reviewHeader.ReviewStageId = model.ReturnToStageID;
                            reviewHeader.ContractIsAccepted = null;
                            reviewHeader.EvaluationIsAccepted = null;
                            reviewHeader.FlaggedReason = null;
                            reviewHeader.IsFlagged = false;
                            reviewHeader.TimeContractAccepted = null;
                            reviewHeader.TimeEvaluationAccepted = null;
                            reviewHeaderIsUpdated = await _performanceService.RollBackReviewHeaderAsync(reviewHeader);
                            if (reviewHeaderIsUpdated)
                            {
                                await _performanceService.DeleteAllApprovals(model.ReviewHeaderID);
                                await _performanceService.DeleteResultSummaryAsync(model.ReviewHeaderID, true);
                                await _performanceService.DeleteEvaluationsAsync(model.ReviewHeaderID, true);
                            }
                        }
                        else if (model.ReturnToStageID == 7)
                        {
                            reviewHeader.ReviewStageId = model.ReturnToStageID;
                            reviewHeader.EvaluationIsAccepted = null;
                            reviewHeader.FlaggedReason = null;
                            reviewHeader.IsFlagged = false;
                            reviewHeader.TimeEvaluationAccepted = null;
                            reviewHeaderIsUpdated = await _performanceService.RollBackReviewHeaderAsync(reviewHeader);
                            if (reviewHeaderIsUpdated)
                            {
                                await _performanceService.DeleteApprovalByType(model.ReviewHeaderID, Base.Enums.ReviewApprovalType.ApproveEvaluationResult);
                                await _performanceService.DeleteResultSummaryAsync(model.ReviewHeaderID, true);
                                await _performanceService.DeleteEvaluationsAsync(model.ReviewHeaderID, false);
                            }
                        }
                    }
                    // End of checking

                    if (reviewHeaderIsUpdated)
                    {
                        model.FromEmployeeName = HttpContext.User.Identity.Name;
                        model.FromEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;

                        Employee hrRep = new Employee();
                        hrRep = await _ermService.GetEmployeeByIdAsync(model.FromEmployeeID);
                        Employee appraisee = new Employee();
                        appraisee = await _ermService.GetEmployeeByIdAsync(model.AppraiseeID);

                        //====== Add Appraisal Activity History =======//
                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Appraisal was returned from HR Department for corrections to {appraisee.FullName} by {hrRep.FullName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT.";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        history.ReviewSessionId = model.ReviewSessionID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);

                        //===== Add Appraisal Note ================//
                        ReviewMessage note = new ReviewMessage();
                        note.MessageBody = model.ReturnInstruction;
                        note.MessageIsCancelled = false;
                        note.MessageTime = DateTime.UtcNow;
                        note.ReviewHeaderId = model.ReviewHeaderID;
                        note.ReviewSessionId = model.ReviewSessionID;

                        if (hrRep != null && !string.IsNullOrWhiteSpace(hrRep.FullName))
                        {
                            note.FromEmployeeId = hrRep.EmployeeID;
                            note.FromEmployeeSex = hrRep.Sex;
                            note.FromEmployeeName = hrRep.FullName;
                            await _performanceService.AddReviewMessageAsync(note);
                        }

                        //===== Send Notificiation Message to Approver ========//
                        Message message = new Message
                        {
                            MessageID = Guid.NewGuid().ToString(),
                            RecipientID = appraisee.EmployeeID,
                            RecipientName = appraisee.FullName,
                            SentTime = DateTime.UtcNow,
                            SentBy = model.FromEmployeeName
                        };

                        message.Subject = "Your Appraisal was Returned by HR Department";
                        message.MessageBody = UtilityHelper.GetHrReturnAppraisalMessageContent(appraisee.FullName, hrRep.FullName);
                        bool messageSent = await _baseModelService.SendMessageAsync(message);

                        if (!string.IsNullOrWhiteSpace(appraisee.OfficialEmail))
                        {
                            //===== Send Email Notifications to Approver =========//
                            bool approverEmailCopySent = false;
                            UtilityHelper utilityHelper = new UtilityHelper(_configuration);
                            EmailModel recipientEmailCopy = new EmailModel();
                            recipientEmailCopy.RecipientName = appraisee.FullName;
                            recipientEmailCopy.RecipientEmail = appraisee.OfficialEmail;
                            recipientEmailCopy.SenderName = model.FromEmployeeName;
                            recipientEmailCopy.Subject = "Your Appraisal was Returned by HR Department";
                            recipientEmailCopy.HtmlContent = UtilityHelper.GetHrReturnAppraisalEmailHtmlContent(appraisee.FullName, hrRep.FullName);
                            recipientEmailCopy.PlainContent = UtilityHelper.GetHrReturnAppraisalEmailPlainContent(appraisee.FullName, hrRep.FullName);
                            approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
                        }
                        model.OperationIsCompleted = true;
                        model.OperationIsSuccessful = true;
                        model.ViewModelSuccessMessage = "Appraisal returned successfully!";
                        //return RedirectToAction("CheckEntries", new { id = model.ReviewSessionID });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted return failed.";
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

        #region Helper Methods
        private FileResult GenerateResultReportExcel(string fileName, IEnumerable<ResultDetail> results)
        {
            DataTable dataTable = new DataTable("results");
            dataTable.Columns.AddRange(new DataColumn[]
            {
new DataColumn("Appraisee No"),
new DataColumn("Appraisee Name"),
new DataColumn("Appraisee Designation"),
new DataColumn("Appraisee Unit"),
new DataColumn("Appraisee Department"),
new DataColumn("Appraisee Location"),
new DataColumn("Feedback Problems"),
new DataColumn("Feedback Solutions"),
new DataColumn("Appraisee Disagrees"),
new DataColumn("Appraiser Name"),
new DataColumn("Appraiser Designation"),
new DataColumn("Appraiser Role"),
new DataColumn("Appraiser Type"),
new DataColumn("Kpa Score"),
new DataColumn("Competency Score"),
new DataColumn("Total Score"),
new DataColumn("Rating"),
new DataColumn("Line Manager Recommendation"),
new DataColumn("Line Manager Comments"),
new DataColumn("Line Manager Name"),
new DataColumn("Unit Head Recommendation"),
new DataColumn("Unit Head Comments"),
new DataColumn("Unit Head Name"),
new DataColumn("Department Head Recommendation"),
new DataColumn("Department Head Comments"),
new DataColumn("Department Head Name"),
new DataColumn("HR Recommendation"),
new DataColumn("HR Comments"),
new DataColumn("Management Decision"),
new DataColumn("ManagementComments"),
            });

            foreach (var result in results)
            {
                dataTable.Rows.Add(
                    result.EmployeeNo,
                    result.AppraiseeName,
                    result.AppraiseeDesignation,
                    result.UnitName,
                    result.DepartmentName,
                    result.LocationName,
                    result.FeedbackProblems,
                    result.FeedbackSolutions,
                    result.IsFlagged,
                    result.AppraiserName,
                    result.AppraiserDesignation,
                    result.AppraiserRoleDescription,
                    result.AppraiserTypeDescription,
                    result.KpaScoreObtained,
                    result.CompetencyScoreObtained,
                    result.CombinedScoreObtained,
                    result.PerformanceRating,
                    result.LineManagerRecommendation,
                    result.LineManagerComments,
                    result.LineManagerName,
                    result.UnitHeadRecommendation,
                    result.UnitHeadComments,
                    result.UnitHeadName,
                    result.DepartmentHeadRecommendation,
                    result.DepartmentHeadComments,
                    result.DepartmentHeadName,
                    result.HrRecommendation,
                    result.HrComments,
                    result.ManagementDecision,
                    result.ManagementComments
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

        private FileResult GenerateProgressStatusReportExcel(string fileName, IEnumerable<ReviewHeader> records)
        {
            DataTable dataTable = new DataTable("records");
            int RowNumber = 0;
            int RecordCount = records.ToList().Count;
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn(RecordCount.ToString()),
                new DataColumn("Name"),
                new DataColumn("Designation"),
                new DataColumn("Unit"),
                new DataColumn("Department"),
                new DataColumn("Location"),
                new DataColumn("Appraiser Name"),
                new DataColumn("Appraiser Designation"),
                new DataColumn("Appraisal Stage"),
            });

            foreach (var result in records)
            {
                RowNumber++;
                dataTable.Rows.Add(
                    RowNumber.ToString(),
                    result.AppraiseeName,
                    result.AppraiseeDesignation,
                    result.UnitName,
                    result.DepartmentName,
                    result.LocationName,
                    result.PrimaryAppraiserName,
                    result.PrimaryAppraiserDesignation,
                    result.ReviewStageDescription
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

        private FileResult GenerateAppraisalNonParticipantsReportExcel(string fileName, IEnumerable<Employee> records)
        {
            DataTable dataTable = new DataTable("records");
            int RowNumber = 0;
            int RecordCount = records.ToList().Count;
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn(RecordCount.ToString()),
                new DataColumn("Staff No."),
                new DataColumn("Full Name"),
                new DataColumn("Gender"),
                new DataColumn("Phone No."),
                new DataColumn("Email"),
                new DataColumn("Unit"),
                new DataColumn("Department"),
                new DataColumn("Location"),
            });

            foreach (var e in records)
            {
                RowNumber++;
                dataTable.Rows.Add(
                    RowNumber.ToString(),
                    e.EmployeeNo1,
                    e.FullName,
                    e.Sex,
                    e.PhoneNo1,
                    e.OfficialEmail,
                    e.UnitName,
                    e.DepartmentName,
                    e.LocationName
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