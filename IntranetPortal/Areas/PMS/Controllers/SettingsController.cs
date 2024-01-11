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

namespace IntranetPortal.Areas.PMS.Controllers
{
    [Area("PMS")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly IDataProtector _dataProtector;
        private readonly IPerformanceService _performanceService;

        public SettingsController(IConfiguration configuration,
                                    IGlobalSettingsService globalSettingsService, IDataProtectionProvider dataProtectionProvider,
                                    DataProtectionEncryptionStrings dataProtectionEncryptionStrings, IErmService ermService,
                                    IBaseModelService baseModelService, IPerformanceService performanceService)
        {
            _configuration = configuration;
            _globalSettingsService = globalSettingsService;
            _ermService = ermService;
            _baseModelService = baseModelService;
            _performanceService = performanceService;
            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionEncryptionStrings.RouteValuesEncryptionCode);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public IActionResult Index()
        {
            return View();
        }

        #region Performance Year Controller Actions

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> PerformanceYears()
        {
            PerformanceYearsListViewModel model = new PerformanceYearsListViewModel();
            var entities = await _performanceService.GetPerformanceYearsAsync();
            if (entities != null && entities.Count > 0)
            {
                model.PerformanceYearList = entities.ToList();
            }

            if (TempData["Error"] != null)
            {
                model.ViewModelErrorMessage = TempData["Error"].ToString();
            }

            if (TempData["Success"] != null)
            {
                model.ViewModelSuccessMessage = TempData["Success"].ToString();
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManagePerformanceYear(int id)
        {
            PerformanceYearViewModel model = new PerformanceYearViewModel();
            if (id > 0)
            {
                PerformanceYear performanceYear = await _performanceService.GetPerformanceYearAsync(id);
                if (performanceYear != null && !string.IsNullOrWhiteSpace(performanceYear.Name))
                {
                    model = model.ExtractFromPerformanceYear(performanceYear);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> ManagePerformanceYear(PerformanceYearViewModel model)
        {
            try
            {
                PerformanceYear performanceYear = new PerformanceYear();
                if (ModelState.IsValid)
                {
                    performanceYear = model.ConvertToPerformanceYear();
                    performanceYear.CreatedBy = HttpContext.User.Identity.Name;
                    performanceYear.CreatedTime = DateTime.UtcNow;
                    if (performanceYear.Id < 1)
                    {
                        bool IsAdded = await _performanceService.AddPerformanceYearAsync(performanceYear);
                        if (IsAdded)
                        {
                            return RedirectToAction("PerformanceYears");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        performanceYear.LastModifiedBy = HttpContext.User.Identity.Name;
                        performanceYear.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditPerformanceYearAsync(performanceYear);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Performance Year was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
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

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeletePerformanceYear(int yd)
        {

            try
            {
                if (yd > 0)
                {
                    bool IsDeleted = await _performanceService.DeletePerformanceYearAsync(yd);
                    if (IsDeleted)
                    {
                        TempData["Success"] = "Records deleted successfully!";
                    }
                    else
                    {
                        TempData["Error"] = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("PerformanceYears");
        }
        #endregion

        #region Approver Roles Controller Actions

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ApproverRoles()
        {
            ApprovalRolesListViewModel model = new ApprovalRolesListViewModel();
            var entities = await _performanceService.GetApprovalRolesAsync();
            if (entities != null && entities.Count > 0)
            {
                model.ApprovalRoleList = entities.ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManageApproverRole(int id)
        {
            ApprovalRoleViewModel model = new ApprovalRoleViewModel();
            if (id > 0)
            {
                ApprovalRole approvalRole = await _performanceService.GetApprovalRoleAsync(id);
                if (approvalRole != null && !string.IsNullOrWhiteSpace(approvalRole.ApprovalRoleName))
                {
                    model.ApprovalRoleID = approvalRole.ApprovalRoleId;
                    model.ApprovalRoleName = approvalRole.ApprovalRoleName;
                    model.MustApproveContract = approvalRole.MustApproveContract;
                    model.MustApproveEvaluation = approvalRole.MustApproveEvaluation;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> ManageApproverRole(ApprovalRoleViewModel model)
        {
            try
            {
                ApprovalRole approvalRole = new ApprovalRole();
                if (ModelState.IsValid)
                {
                    approvalRole.ApprovalRoleId = model.ApprovalRoleID;
                    approvalRole.ApprovalRoleName = model.ApprovalRoleName;
                    approvalRole.MustApproveContract = model.MustApproveContract;
                    approvalRole.MustApproveEvaluation = model.MustApproveEvaluation;

                    if (approvalRole.ApprovalRoleId < 1)
                    {
                        bool IsAdded = await _performanceService.AddApprovalRoleAsync(approvalRole);
                        if (IsAdded)
                        {
                            return RedirectToAction("ApproverRoles");
                            //model.OperationIsSuccessful = true;
                           // model.ViewModelSuccessMessage = "New Approver Role was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _performanceService.EditApprovalRoleAsync(approvalRole);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Approver Role was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
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

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteApproverRole(int id)
        {
            ApprovalRoleViewModel model = new ApprovalRoleViewModel();
            if (id > 0)
            {
                ApprovalRole approvalRole = await _performanceService.GetApprovalRoleAsync(id);
                if (approvalRole != null && !string.IsNullOrWhiteSpace(approvalRole.ApprovalRoleName))
                {
                    model.ApprovalRoleID = approvalRole.ApprovalRoleId;
                    model.ApprovalRoleName = approvalRole.ApprovalRoleName;
                    model.MustApproveContract = approvalRole.MustApproveContract;
                    model.MustApproveEvaluation = approvalRole.MustApproveEvaluation;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        [HttpPost]
        public async Task<IActionResult> DeleteApproverRole(ApprovalRoleViewModel model)
        {
            try
            {
                if (model.ApprovalRoleID > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteApprovalRoleAsync(model.ApprovalRoleID);
                    if (IsDeleted)
                    {
                        return RedirectToAction("ApproverRoles");
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
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

        #region Review Session Controller Actions

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ReviewSessions(int? id)
        {
            ReviewSessionsListViewModel model = new ReviewSessionsListViewModel();
            if (id != null && id.Value > 0)
            {
                var entities = await _performanceService.GetReviewSessionsAsync(id.Value);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSessionsList = entities.ToList();
                }
            }
            else
            {
                var entities = await _performanceService.GetReviewSessionsAsync();
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSessionsList = entities.ToList();
                }
            }

            List<PerformanceYear> pyears = await _performanceService.GetPerformanceYearsAsync();
            if (pyears != null && pyears.Count > 0)
            {
                ViewBag.PerformanceYearsList = new SelectList(pyears, "Id", "Name", id);
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManageReviewSession(int id)
        {
            ManageReviewSessionViewModel model = new ManageReviewSessionViewModel();
            if (id > 0)
            {
                ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model = model.ExtractViewModel(reviewSession);
                    if (reviewSession.IsActive) { model.IsActive = 1; } else { model.IsActive = 0; }
                }
            }

            List<PerformanceYear> pyears = await _performanceService.GetPerformanceYearsAsync();
            if (pyears != null && pyears.Count > 0)
            {
                ViewBag.PerformanceYearsList = new SelectList(pyears, "Id", "Name");
            }

            List<ReviewType> types = await _performanceService.GetReviewTypesAsync();
            if (types != null && types.Count > 0)
            {
                ViewBag.ReviewTypesList = new SelectList(types, "ReviewTypeId", "ReviewTypeName");
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageReviewSession(ManageReviewSessionViewModel model)
        {
            try
            {
                ReviewSession reviewSession = new ReviewSession();
                if (ModelState.IsValid)
                {
                    reviewSession = model.ConvertToReviewSession();
                    reviewSession.CreatedBy = HttpContext.User.Identity.Name;
                    reviewSession.CreatedTime = DateTime.UtcNow;
                    if (model.IsActive == 0) { reviewSession.IsActive = false; } else { reviewSession.IsActive = true; }
                    if (reviewSession.Id < 1)
                    {
                        bool IsAdded = await _performanceService.AddReviewSessionAsync(reviewSession);
                        if (IsAdded)
                        {
                            return RedirectToAction("ReviewSessions");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        reviewSession.LastModifiedBy = HttpContext.User.Identity.Name;
                        reviewSession.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditReviewSessionAsync(reviewSession);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Appraisal Session was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
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

        public async Task<IActionResult> ShowReviewSession(int id)
        {
            ManageReviewSessionViewModel model = new ManageReviewSessionViewModel();
            if (id > 0)
            {
                ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model = model.ExtractViewModel(reviewSession);
                    model.StartDateFormatted = reviewSession.StartDate.Value.ToString("yyyy-MM-dd");
                    model.EndDateFormatted = reviewSession.EndDate.Value.ToString("yyyy-MM-dd");
                    if (reviewSession.IsActive) { model.IsActive = 1; } else { model.IsActive = 0; }
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the record you selected was not found.";
                }
                return View(model);
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteReviewSession(int id)
        {
            ManageReviewSessionViewModel model = new ManageReviewSessionViewModel();
            if (id > 0)
            {
                ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model = model.ExtractViewModel(reviewSession);
                    model.StartDateFormatted = reviewSession.StartDate.Value.ToString("yyyy-MM-dd");
                    model.EndDateFormatted = reviewSession.EndDate.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the record you selected was not found.";
                }
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteReviewSession(ManageReviewSessionViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteReviewSessionAsync(model.Id.Value);
                    if (IsDeleted)
                    {
                        return RedirectToAction("ReviewSessions");
                    }
                    else
                    {
                        model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                    }
                }
                else
                {
                    model.ViewModelSuccessMessage = "The record was not deleted. Because some parameter had invalid values. Please try again.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ReviewSessionInfo(int id, string src = null)
        {
            ManageReviewSessionViewModel model = new ManageReviewSessionViewModel();
            if (!string.IsNullOrWhiteSpace(src))
            {
                model.src = src;
            }
            if (id > 0)
            {
                ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model = model.ExtractViewModel(reviewSession);
                    model.StartDateFormatted = reviewSession.StartDate.Value.ToString("yyyy-MM-dd");
                    model.EndDateFormatted = reviewSession.EndDate.Value.ToString("yyyy-MM-dd");
                    if (reviewSession.IsActive) { model.IsActive = 1; } else { model.IsActive = 0; }
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, the record you selected was not found.";
                }
            }
            return View(model);
        }

        #endregion

        #region Review Grades Controller Actions

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> Grading()
        {
            GradingListViewModel model = new GradingListViewModel();
            var entities = await _performanceService.GetGradeHeadersAsync();
            if (entities != null && entities.Count > 0)
            {
                model.GradeHeaderList = entities.ToList();
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManageGradeProfile(int id)
        {
            ManageGradeProfileViewModel model = new ManageGradeProfileViewModel();
            if (id > 0)
            {
                GradeHeader gradeHeader = await _performanceService.GetGradeHeaderAsync(id);
                if (gradeHeader != null && !string.IsNullOrWhiteSpace(gradeHeader.GradeHeaderName))
                {
                    model.GradeHeaderId = gradeHeader.GradeHeaderId;
                    model.GradeHeaderName = gradeHeader.GradeHeaderName;
                    model.GradeHeaderDescription = gradeHeader.GradeHeaderDescription;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManageGradeProfile(ManageGradeProfileViewModel model)
        {
            try
            {
                GradeHeader gradeHeader = new GradeHeader();
                if (ModelState.IsValid)
                {
                    gradeHeader.GradeHeaderId = model.GradeHeaderId ?? 0;
                    gradeHeader.GradeHeaderName = model.GradeHeaderName;
                    gradeHeader.GradeHeaderDescription = model.GradeHeaderDescription;

                    if (gradeHeader.GradeHeaderId < 1)
                    {
                        bool IsAdded = await _performanceService.AddGradeHeaderAsync(gradeHeader);
                        if (IsAdded)
                        {
                            return RedirectToAction("Grading");
                            //model.OperationIsSuccessful = true;
                            //model.ViewModelSuccessMessage = "New Grading Profile was added successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        bool IsUpdated = await _performanceService.EditGradeHeaderAsync(gradeHeader);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Grading Profile was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
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

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteGradeProfile(int id)
        {
            ManageGradeProfileViewModel model = new ManageGradeProfileViewModel();
            if (id > 0)
            {
                GradeHeader gradeHeader = await _performanceService.GetGradeHeaderAsync(id);
                if (gradeHeader != null && !string.IsNullOrWhiteSpace(gradeHeader.GradeHeaderName))
                {
                    model.GradeHeaderId = gradeHeader.GradeHeaderId;
                    model.GradeHeaderName = gradeHeader.GradeHeaderName;
                    model.GradeHeaderDescription = gradeHeader.GradeHeaderDescription;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteGradeProfile(ManageGradeProfileViewModel model)
        {
            try
            {
                GradeHeader gradeHeader = new GradeHeader();
                if (ModelState.IsValid)
                {
                    gradeHeader.GradeHeaderId = model.GradeHeaderId ?? 0;
                    gradeHeader.GradeHeaderName = model.GradeHeaderName;
                    gradeHeader.GradeHeaderDescription = model.GradeHeaderDescription;

                    if (gradeHeader.GradeHeaderId > 0)
                    {
                        bool IsDeleted = await _performanceService.DeleteGradeHeaderAsync(gradeHeader.GradeHeaderId);
                        if (IsDeleted)
                        {
                            return RedirectToAction("Grading");
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        model.ViewModelSuccessMessage = "Sorry, a key parameter is missing. The operation could not be completed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }


        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> PerformanceGradeTemplate(int id)
        {
            ReviewGradeListViewModel model = new ReviewGradeListViewModel();
            if (id > 0)
            {
                model.TemplateId = id;
                var entities = await _performanceService.GetPerformanceGradesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewGradeList = entities.ToList();
                }
            }

            return View(model);
        }


        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManagePerformanceGrade(int td, int? id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            model.GradeHeaderId = td;
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id.Value);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManagePerformanceGrade(ManageReviewGradeViewModel model)
        {
            try
            {
                ReviewGrade reviewGrade = new ReviewGrade();
                if (ModelState.IsValid)
                {
                    reviewGrade = model.ConvertToPerformanceGrade();
                    reviewGrade.CreatedBy = HttpContext.User.Identity.Name;
                    reviewGrade.CreatedTime = DateTime.UtcNow;
                    if (reviewGrade.ReviewGradeId < 1)
                    {
                        bool IsAdded = await _performanceService.AddReviewGradeAsync(reviewGrade);
                        if (IsAdded)
                        {
                            return RedirectToAction("PerformanceGradeTemplate", new { id = reviewGrade.GradeHeaderId });
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        reviewGrade.LastModifiedBy = HttpContext.User.Identity.Name;
                        reviewGrade.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditReviewGradeAsync(reviewGrade);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Performance Grade was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
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

        public async Task<IActionResult> PerformanceGradeInfo(int id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeletePerformanceGrade(int id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeRankDescription = reviewGrade.GradeRankDescription;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeletePerformanceGrade(ManageReviewGradeViewModel model)
        {

            try
            {
                if (model != null && model.ReviewGradeId > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteReviewGradeAsync(model.ReviewGradeId);
                    if (IsDeleted)
                    {
                        model.ViewModelSuccessMessage = "Records deleted successfully!";
                        return RedirectToAction("PerformanceGradeTemplate", new { id = model.GradeHeaderId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        //============================== Competency Grades =======================================//    

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> CompetencyGradeTemplate(int id)
        {
            ReviewGradeListViewModel model = new ReviewGradeListViewModel();
            if (id > 0)
            {
                model.TemplateId = id;
                var entities = await _performanceService.GetCompetencyGradesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewGradeList = entities.ToList();
                }
            }

            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManageCompetencyGrade(int td, int? id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            model.GradeHeaderId = td;
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id.Value);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> ManageCompetencyGrade(ManageReviewGradeViewModel model)
        {
            try
            {
                ReviewGrade reviewGrade = new ReviewGrade();
                if (ModelState.IsValid)
                {
                    reviewGrade = model.ConvertToCompetencyGrade();
                    reviewGrade.CreatedBy = HttpContext.User.Identity.Name;
                    reviewGrade.CreatedTime = DateTime.UtcNow;
                    if (reviewGrade.ReviewGradeId < 1)
                    {
                        bool IsAdded = await _performanceService.AddReviewGradeAsync(reviewGrade);
                        if (IsAdded)
                        {
                            return RedirectToAction("CompetencyGradeTemplate", new { id = reviewGrade.GradeHeaderId });
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
                        }
                    }
                    else
                    {
                        reviewGrade.LastModifiedBy = HttpContext.User.Identity.Name;
                        reviewGrade.LastModifiedTime = DateTime.UtcNow;
                        bool IsUpdated = await _performanceService.EditReviewGradeAsync(reviewGrade);
                        if (IsUpdated)
                        {
                            model.OperationIsSuccessful = true;
                            model.ViewModelSuccessMessage = "Competency Grade was updated successfully!";
                        }
                        else
                        {
                            model.ViewModelSuccessMessage = "Sorry, an error was encountered. Please try again.";
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

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> CompetencyGradeInfo(int id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteCompetencyGrade(int id)
        {
            ManageReviewGradeViewModel model = new ManageReviewGradeViewModel();
            if (id > 0)
            {
                ReviewGrade reviewGrade = await _performanceService.GetReviewGradeAsync(id);
                if (reviewGrade != null && !string.IsNullOrWhiteSpace(reviewGrade.ReviewGradeDescription))
                {
                    model.GradeHeaderId = reviewGrade.GradeHeaderId;
                    model.GradeHeaderName = reviewGrade.GradeHeaderName;
                    model.ReviewGradeDescription = reviewGrade.ReviewGradeDescription;
                    model.GradeRank = reviewGrade.GradeRank;
                    model.GradeRankDescription = reviewGrade.GradeRankDescription;
                    model.GradeType = reviewGrade.GradeType;
                    model.LowerBandScore = reviewGrade.LowerBandScore;
                    model.ReviewGradeId = reviewGrade.ReviewGradeId;
                    model.UpperBandScore = reviewGrade.UpperBandScore;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTT, XYALLACCZ")]
        public async Task<IActionResult> DeleteCompetencyGrade(ManageReviewGradeViewModel model)
        {

            try
            {
                if (model != null && model.ReviewGradeId > 0)
                {
                    bool IsDeleted = await _performanceService.DeleteReviewGradeAsync(model.ReviewGradeId);
                    if (IsDeleted)
                    {
                        model.ViewModelSuccessMessage = "Records deleted successfully!";
                        return RedirectToAction("CompetencyGradeTemplate", new { id = model.GradeHeaderId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "Sorry, an error was encountered. Delete operation failed.";
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

        #region Appraisal Schedules Controller Actions

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> AppraisalSchedules(int id)
        {
            AppraisalSchedulesListViewModel model = new AppraisalSchedulesListViewModel();
            ReviewSession reviewSession = new ReviewSession();
            model.ReviewSessionId = id;
            reviewSession = await _performanceService.GetReviewSessionAsync(id);
            if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
            {
                model.ReviewSessionName = reviewSession.Name;
                model.ReviewSessionId = reviewSession.Id;

                var entities = await _performanceService.GetSessionSchdulesAsync(model.ReviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    model.SessionScheduleList = entities;
                }
            }

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.DepartmentList = new SelectList(depts, "DepartmentID", "DepartmentName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");

            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> AppraisalSessionSchedules(int id)
        {
            AppraisalSchedulesListViewModel model = new AppraisalSchedulesListViewModel();
            ReviewSession reviewSession = new ReviewSession();
            model.ReviewSessionId = id;
            reviewSession = await _performanceService.GetReviewSessionAsync(id);
            if (reviewSession != null && !string.IsNullOrWhiteSpace(reviewSession.Name))
            {
                model.ReviewSessionName = reviewSession.Name;
                model.ReviewSessionId = reviewSession.Id;

                var entities = await _performanceService.GetSessionSchdulesAsync(model.ReviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    model.SessionScheduleList = entities;
                }
            }

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.DepartmentList = new SelectList(depts, "DepartmentID", "DepartmentName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");

            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> NewAppraisalSchedule(int id, int tp)
        {
            ManageAppraisalScheduleViewModel model = new ManageAppraisalScheduleViewModel();
            model.ReviewSessionId = id;
            model.ScheduleTypeId = tp;

            var locations = await _globalSettingsService.GetAllLocationsAsync();
            var depts = await _globalSettingsService.GetDepartmentsAsync();
            var units = await _globalSettingsService.GetUnitsAsync();

            ViewBag.LocationList = new SelectList(locations, "LocationID", "LocationName");
            ViewBag.DepartmentList = new SelectList(depts, "DepartmentID", "DepartmentName");
            ViewBag.UnitList = new SelectList(units, "UnitID", "UnitName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> NewAppraisalSchedule(ManageAppraisalScheduleViewModel model)
        {
            SessionSchedule sessionSchedule = new SessionSchedule();
            sessionSchedule = model.ConvertToSessionSchedule();
            bool IsAdded = await _performanceService.AddSessionScheduleAsync(sessionSchedule);
            if (IsAdded) { return RedirectToAction("AppraisalSchedules", new { id = model.ReviewSessionId }); }
            else { model.ViewModelErrorMessage = "Sorry an error was encountered. New Schedule could not be created."; }
            return View(model);
        }


        public async Task<IActionResult> AppraisalScheduleInfo(int id)
        {
            ManageAppraisalScheduleViewModel model = new ManageAppraisalScheduleViewModel();
            if (id > 0)
            {
                SessionSchedule sessionSchedule = new SessionSchedule();
                sessionSchedule = await _performanceService.GetSessionScheduleAsync(id);
                if (sessionSchedule != null)
                {
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ActivityTypeId = (int)sessionSchedule.ActivityType;
                    model.ActivityTypeDescription = sessionSchedule.SessionActivityTypeDescription;
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ReviewYearId = sessionSchedule.ReviewYearId;
                    model.ReviewYearName = sessionSchedule.ReviewYearName;
                    model.ScheduleDepartmentId = sessionSchedule.ScheduleDepartmentId;
                    model.ScheduleDepartmentName = sessionSchedule.ScheduleDepartmentName;
                    model.ScheduleEmployeeId = sessionSchedule.ScheduleEmployeeId;
                    model.ScheduleEmployeeName = sessionSchedule.ScheduleEmployeeName;
                    model.ScheduleEndTime = sessionSchedule.ScheduleEndTime;
                    model.ScheduleLocationId = sessionSchedule.ScheduleLocationId;
                    model.ScheduleLocationName = sessionSchedule.ScheduleLocationName;
                    model.ScheduleStartTime = sessionSchedule.ScheduleStartTime;
                    model.ScheduleTypeDescription = sessionSchedule.ScheduleTypeDescription;
                    model.ScheduleTypeId = (int)sessionSchedule.ScheduleType;
                    model.ScheduleUnitId = sessionSchedule.ScheduleUnitId;
                    model.ScheduleUnitName = sessionSchedule.ScheduleUnitName;
                    model.SessionScheduleId = sessionSchedule.SessionScheduleId;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> CancelAppraisalSchedule(int id)
        {
            ManageAppraisalScheduleViewModel model = new ManageAppraisalScheduleViewModel();
            if (id > 0)
            {
                SessionSchedule sessionSchedule = new SessionSchedule();
                sessionSchedule = await _performanceService.GetSessionScheduleAsync(id);
                if (sessionSchedule != null)
                {
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ActivityTypeId = (int)sessionSchedule.ActivityType;
                    model.ActivityTypeDescription = sessionSchedule.SessionActivityTypeDescription;
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ReviewYearId = sessionSchedule.ReviewYearId;
                    model.ReviewYearName = sessionSchedule.ReviewYearName;
                    model.ScheduleDepartmentId = sessionSchedule.ScheduleDepartmentId;
                    model.ScheduleDepartmentName = sessionSchedule.ScheduleDepartmentName;
                    model.ScheduleEmployeeId = sessionSchedule.ScheduleEmployeeId;
                    model.ScheduleEmployeeName = sessionSchedule.ScheduleEmployeeName;
                    model.ScheduleEndTime = sessionSchedule.ScheduleEndTime;
                    model.ScheduleLocationId = sessionSchedule.ScheduleLocationId;
                    model.ScheduleLocationName = sessionSchedule.ScheduleLocationName;
                    model.ScheduleStartTime = sessionSchedule.ScheduleStartTime;
                    model.ScheduleTypeDescription = sessionSchedule.ScheduleTypeDescription;
                    model.ScheduleTypeId = (int)sessionSchedule.ScheduleType;
                    model.ScheduleUnitId = sessionSchedule.ScheduleUnitId;
                    model.ScheduleUnitName = sessionSchedule.ScheduleUnitName;
                    model.SessionScheduleId = sessionSchedule.SessionScheduleId;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> CancelAppraisalSchedule(ManageAppraisalScheduleViewModel model)
        {
            if (model != null && model.SessionScheduleId > 0)
            {
                string CancelledBy = HttpContext.User.Identity.Name;
                bool IsAdded = await _performanceService.CancelSessionScheduleAsync(model.SessionScheduleId, CancelledBy);
                if (IsAdded)
                {
                    model.ViewModelSuccessMessage = "Appraisal Schedule was cancelled successfully!";
                    model.OperationIsSuccessful = true;
                }
                else { model.ViewModelErrorMessage = "Sorry an error was encountered. Appraisal Schedule could not be cancelled."; }
            }

            SessionSchedule sessionSchedule = new SessionSchedule();
            sessionSchedule = await _performanceService.GetSessionScheduleAsync(model.SessionScheduleId);
            if (sessionSchedule != null)
            {
                model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                model.ActivityTypeId = (int)sessionSchedule.ActivityType;
                model.ActivityTypeDescription = sessionSchedule.SessionActivityTypeDescription;
                model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                model.ReviewYearId = sessionSchedule.ReviewYearId;
                model.ReviewYearName = sessionSchedule.ReviewYearName;
                model.ScheduleDepartmentId = sessionSchedule.ScheduleDepartmentId;
                model.ScheduleDepartmentName = sessionSchedule.ScheduleDepartmentName;
                model.ScheduleEmployeeId = sessionSchedule.ScheduleEmployeeId;
                model.ScheduleEmployeeName = sessionSchedule.ScheduleEmployeeName;
                model.ScheduleEndTime = sessionSchedule.ScheduleEndTime;
                model.ScheduleLocationId = sessionSchedule.ScheduleLocationId;
                model.ScheduleLocationName = sessionSchedule.ScheduleLocationName;
                model.ScheduleStartTime = sessionSchedule.ScheduleStartTime;
                model.ScheduleTypeDescription = sessionSchedule.ScheduleTypeDescription;
                model.ScheduleTypeId = (int)sessionSchedule.ScheduleType;
                model.ScheduleUnitId = sessionSchedule.ScheduleUnitId;
                model.ScheduleUnitName = sessionSchedule.ScheduleUnitName;
                model.SessionScheduleId = sessionSchedule.SessionScheduleId;
            }
            return View(model);
        }

        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> DeleteAppraisalSchedule(int id)
        {
            ManageAppraisalScheduleViewModel model = new ManageAppraisalScheduleViewModel();
            if (id > 0)
            {
                SessionSchedule sessionSchedule = new SessionSchedule();
                sessionSchedule = await _performanceService.GetSessionScheduleAsync(id);
                if (sessionSchedule != null)
                {
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ActivityTypeId = (int)sessionSchedule.ActivityType;
                    model.ActivityTypeDescription = sessionSchedule.SessionActivityTypeDescription;
                    model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                    model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                    model.ReviewYearId = sessionSchedule.ReviewYearId;
                    model.ReviewYearName = sessionSchedule.ReviewYearName;
                    model.ScheduleDepartmentId = sessionSchedule.ScheduleDepartmentId;
                    model.ScheduleDepartmentName = sessionSchedule.ScheduleDepartmentName;
                    model.ScheduleEmployeeId = sessionSchedule.ScheduleEmployeeId;
                    model.ScheduleEmployeeName = sessionSchedule.ScheduleEmployeeName;
                    model.ScheduleEndTime = sessionSchedule.ScheduleEndTime;
                    model.ScheduleLocationId = sessionSchedule.ScheduleLocationId;
                    model.ScheduleLocationName = sessionSchedule.ScheduleLocationName;
                    model.ScheduleStartTime = sessionSchedule.ScheduleStartTime;
                    model.ScheduleTypeDescription = sessionSchedule.ScheduleTypeDescription;
                    model.ScheduleTypeId = (int)sessionSchedule.ScheduleType;
                    model.ScheduleUnitId = sessionSchedule.ScheduleUnitId;
                    model.ScheduleUnitName = sessionSchedule.ScheduleUnitName;
                    model.SessionScheduleId = sessionSchedule.SessionScheduleId;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PMSSTTMGA, XYALLACCZ")]
        public async Task<IActionResult> DeleteAppraisalSchedule(ManageAppraisalScheduleViewModel model)
        {
            if (model != null && model.SessionScheduleId > 0)
            {
                string CancelledBy = HttpContext.User.Identity.Name;
                bool IsDeleted = await _performanceService.DeleteSessionScheduleAsync(model.SessionScheduleId);
                if (IsDeleted)
                {
                    return RedirectToAction("AppraisalSchedules", new { id = model.ReviewSessionId});
                }
                else { model.ViewModelErrorMessage = "Sorry an error was encountered. Appraisal Schedule could not be Deleted."; }
            }

            SessionSchedule sessionSchedule = new SessionSchedule();
            sessionSchedule = await _performanceService.GetSessionScheduleAsync(model.SessionScheduleId);
            if (sessionSchedule != null)
            {
                model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                model.ActivityTypeId = (int)sessionSchedule.ActivityType;
                model.ActivityTypeDescription = sessionSchedule.SessionActivityTypeDescription;
                model.ReviewSessionId = sessionSchedule.ReviewSessionId;
                model.ReviewSessionName = sessionSchedule.ReviewSessionName;
                model.ReviewYearId = sessionSchedule.ReviewYearId;
                model.ReviewYearName = sessionSchedule.ReviewYearName;
                model.ScheduleDepartmentId = sessionSchedule.ScheduleDepartmentId;
                model.ScheduleDepartmentName = sessionSchedule.ScheduleDepartmentName;
                model.ScheduleEmployeeId = sessionSchedule.ScheduleEmployeeId;
                model.ScheduleEmployeeName = sessionSchedule.ScheduleEmployeeName;
                model.ScheduleEndTime = sessionSchedule.ScheduleEndTime;
                model.ScheduleLocationId = sessionSchedule.ScheduleLocationId;
                model.ScheduleLocationName = sessionSchedule.ScheduleLocationName;
                model.ScheduleStartTime = sessionSchedule.ScheduleStartTime;
                model.ScheduleTypeDescription = sessionSchedule.ScheduleTypeDescription;
                model.ScheduleTypeId = (int)sessionSchedule.ScheduleType;
                model.ScheduleUnitId = sessionSchedule.ScheduleUnitId;
                model.ScheduleUnitName = sessionSchedule.ScheduleUnitName;
                model.SessionScheduleId = sessionSchedule.SessionScheduleId;
            }
            return View(model);
        }

        #endregion
    }
}