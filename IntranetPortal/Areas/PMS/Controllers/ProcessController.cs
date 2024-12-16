using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Base.Services;
using IntranetPortal.Configurations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntranetPortal.Areas.PMS.Models;
using IntranetPortal.Base.Models.PmsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using Microsoft.AspNetCore.Authentication;
using IntranetPortal.Base.Enums;
using System.Text;
using IntranetPortal.Models;
using IntranetPortal.Helpers;
using IntranetPortal.Base.Models.BaseModels;

namespace IntranetPortal.Areas.PMS.Controllers
{
    [Area("PMS")]
    [Authorize]
    public class ProcessController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGlobalSettingsService _globalSettingsService;
        private readonly IErmService _ermService;
        private readonly IBaseModelService _baseModelService;
        private readonly ISecurityService _securityService;
        private readonly IDataProtector _dataProtector;
        private readonly IPerformanceService _performanceService;

        public ProcessController(IConfiguration configuration,
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

        #region My Appraisal Sessions Controller Actions

        public async Task<IActionResult> MyAppraisalSessions(int? id)
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
            ViewBag.AppraiseeName = HttpContext.User.Identity.Name;

            return View(model);
        }

        public async Task<IActionResult> MyAppraisalSteps(int id)
        {
            MyAppraisalStepsViewModel model = new MyAppraisalStepsViewModel();
            ReviewSession reviewSession = new ReviewSession();

            if (id > 0)
            {
                model.ReviewSessionId = id;
                reviewSession = await _performanceService.GetReviewSessionAsync(id);
                if (!string.IsNullOrWhiteSpace(reviewSession.Name))
                {
                    model.ReviewSessionName = reviewSession.Name;
                    model.IsActive = reviewSession.IsActive;
                    if (!model.IsActive)
                    {
                        model.ViewModelWarningMessage = $"Note: The selected Appraisal Session is closed. All action buttons have been disabled. ";
                    }
                }
            }

            EmployeeUser employeeUser = new EmployeeUser();
            ApplicationUser user = new ApplicationUser();
            //string userId = string.Empty;
            string userName = HttpContext.User.Identity.Name;
            string userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(userName))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }
            else if (string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(userName))
            {
                var employee_users = await _securityService.GetEmployeeUsersByNameAsync(userName);
                if (employee_users != null && employee_users.Count > 0) { employeeUser = employee_users.FirstOrDefault(); }
                if (employeeUser != null && !string.IsNullOrWhiteSpace(employeeUser.FullName))
                {
                    model.AppraiseeName = employeeUser.FullName;
                    model.AppraiseeId = employeeUser.UserID;
                    if (string.IsNullOrWhiteSpace(employeeUser.UserID))
                    {
                        model.ViewModelErrorMessage = "Sorry, no employee record was found for the selected user. ";
                        return View(model);
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, no employee record was found for the selected user. ";
                    return View(model);
                }
            }
            else if (!string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(userName))
            {
                var employee = await _ermService.GetEmployeeByIdAsync(userId);
                if (employee != null && !string.IsNullOrWhiteSpace(employee.FullName))
                {
                    model.AppraiseeName = employee.FullName;
                    model.AppraiseeId = employee.EmployeeID;
                }
                else
                {
                    model.ViewModelErrorMessage = "Sorry, no employee record was found for the selected user. ";
                    return View(model);
                }
            }
            else
            {
                model.AppraiseeId = userId;
                model.AppraiseeName = userName;
            }

            ReviewSchedule reviewSchedule = new ReviewSchedule();
            reviewSchedule = await _performanceService.GetEmployeePerformanceScheduleAsync(id, model.AppraiseeId);
            if ((reviewSchedule == null) || (reviewSchedule.AllActivitiesScheduled == false && reviewSchedule.ContractDefinitionScheduled == false && reviewSchedule.PerformanceEvaluationScheduled == false))
            {
                model.ViewModelWarningMessage = $"Note: You are not scheduled for any activity on this Appraisal Session. All action buttons have been disabled. ";
            }

            model.AllActivitiesScheduled = reviewSchedule.AllActivitiesScheduled;
            model.ContractDefinitionScheduled = reviewSchedule.ContractDefinitionScheduled;
            model.PerformanceEvaluationScheduled = reviewSchedule.PerformanceEvaluationScheduled;

            if (id > 0 && !string.IsNullOrWhiteSpace(model.AppraiseeId))
            {
                ReviewHeader reviewHeader = new ReviewHeader();
                reviewHeader = await _performanceService.GetReviewHeaderAsync(model.AppraiseeId, id);
                if (reviewHeader != null && reviewHeader.ReviewHeaderId > 0)
                {
                    model.CurrentReviewStageId = reviewHeader.ReviewStageId;
                    model.ReviewHeaderId = reviewHeader.ReviewHeaderId;
                }
                else
                {
                    model.CurrentReviewStageId = 0;
                }
            }
            var stagesEntities = await _performanceService.GetReviewStagesAsync();
            if (stagesEntities != null && stagesEntities.Count > 0)
            {
                model.AppraisalStageList = stagesEntities;
            }
            return View(model);
        }

        public async Task<IActionResult> AppraisalActivities(int id, string sp)
        {
            AppraisalActivitiesViewModel model = new AppraisalActivitiesViewModel();
            model.SourcePage = sp;
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;

                    Employee employee_user = new Employee();
                    string userId = string.Empty;
                    userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                    //if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
                    //user = await _securityService.GetUserByIdAsync(userId);
                    employee_user = await _ermService.GetEmployeeByIdAsync(userId);

                    if (!string.IsNullOrWhiteSpace(employee_user.FullName))
                    {
                        model.LoggedInEmployeeID = employee_user.EmployeeID;
                    }
                }

                var entities = await _performanceService.GetPmsActivityHistory(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewActivityList = entities.ToList();
                }
            }
            return View(model);
        }

        #endregion

        #region Performance Goals Controller Actions

        public async Task<IActionResult> CreatePerformanceGoals(int id, string ad)
        {
            CreatePerformanceGoalsViewModel model = new CreatePerformanceGoalsViewModel();
            model.AppraiseeID = ad;
            model.ReviewSessionID = id;
            //List<EmployeeReport> reports = new List<EmployeeReport>();
            List<EmployeeReportLine> reports = new List<EmployeeReportLine>();

            //var reporting_entities = await _employeeRecordService.GetEmployeeReportsByEmployeeIdAsync(ad);
            var reporting_entities = await _ermService.GetActiveEmployeeReportLinesByEmployeeIdAsync(ad);
            if (reporting_entities != null && reporting_entities.Count > 0)
            {
                reports = reporting_entities;
            }
            ViewBag.ReportList = new SelectList(reports, "ReportsToEmployeeID", "ReportsToEmployeeName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePerformanceGoals(CreatePerformanceGoalsViewModel model)
        {
            List<EmployeeReportLine> reports = new List<EmployeeReportLine>();
            if (ModelState.IsValid)
            {
                try
                {
                    ReviewHeader reviewHeader = new ReviewHeader();
                    reviewHeader.ReviewSessionId = model.ReviewSessionID;
                    reviewHeader.ReviewHeaderId = model.ReviewHeaderID ?? 0;
                    reviewHeader.AppraiseeId = model.AppraiseeID;
                    reviewHeader.PerformanceGoal = model.PerformanceGoals;
                    reviewHeader.PrimaryAppraiserId = model.AppraiserID;
                    reviewHeader.ReviewStageId = 1;

                    Employee employee = await _ermService.GetEmployeeByIdAsync(model.AppraiseeID);
                    if (employee != null)
                    {
                        if (employee.UnitID == null || employee.UnitID < 1)
                        {
                            reviewHeader.UnitId = null;
                        }
                        else { reviewHeader.UnitId = employee.UnitID; }

                        if (employee.DepartmentID == null || employee.DepartmentID < 1)
                        {
                            reviewHeader.DepartmentId = null;
                        }
                        else { reviewHeader.DepartmentId = employee.DepartmentID; }

                        if (employee.LocationID == null || employee.LocationID < 1) { reviewHeader.LocationId = null; }
                        else { reviewHeader.LocationId = employee.LocationID; }
                    }

                    ReviewSession reviewSession = new ReviewSession();
                    reviewSession = await _performanceService.GetReviewSessionAsync(model.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        reviewHeader.ReviewYearId = reviewSession.ReviewYearId;
                    }

                    bool isAdded = await _performanceService.AddReviewHeaderAsync(reviewHeader);
                    if (isAdded)
                    {
                        PmsActivityHistory activityHistory = new PmsActivityHistory();
                        var entity = await _performanceService.GetReviewHeaderAsync(model.AppraiseeID, model.ReviewSessionID);
                        if (entity != null)
                        {
                            activityHistory.ReviewHeaderId = entity.ReviewHeaderId;
                            activityHistory.ReviewSessionId = entity.ReviewSessionId;
                            activityHistory.ActivityTime = DateTime.UtcNow;
                            activityHistory.ActivityDescription = $"Started the Appraisal Process and added performance goal.";
                            await _performanceService.AddPmsActivityHistoryAsync(activityHistory);
                        }
                        model.ViewModelSuccessMessage = "Performance Goal added successfully!";
                        model.OperationIsSuccessful = true;
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            var reporting_entities = await _ermService.GetActiveEmployeeReportLinesByEmployeeIdAsync(model.AppraiseeID);
            if (reporting_entities != null && reporting_entities.Count > 0)
            {
                reports = reporting_entities;
            }
            ViewBag.ReportList = new SelectList(reports, "ReportsToEmployeeID", "ReportsToEmployeeName");

            return View(model);
        }

        public async Task<IActionResult> ManagePerformanceGoal(int id)
        {
            CreatePerformanceGoalsViewModel model = new CreatePerformanceGoalsViewModel();
            List<EmployeeReportLine> reports = new List<EmployeeReportLine>();
            ReviewHeader reviewHeader = new ReviewHeader();

            try
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.PerformanceGoals = reviewHeader.PerformanceGoal;
                    model.AppraiserID = reviewHeader.PrimaryAppraiserId;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var reporting_entities = await _ermService.GetActiveEmployeeReportLinesByEmployeeIdAsync(model.AppraiseeID);
            if (reporting_entities != null && reporting_entities.Count > 0)
            {
                reports = reporting_entities;
            }
            ViewBag.ReportList = new SelectList(reports, "ReportsToEmployeeID", "ReportsToEmployeeName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePerformanceGoal(CreatePerformanceGoalsViewModel model)
        {
            List<EmployeeReportLine> reports = new List<EmployeeReportLine>();
            try
            {
                if (model.ReviewHeaderID > 0)
                {
                    bool isUpdated = await _performanceService.UpdatePerformanceGoalAsync(model.ReviewHeaderID.Value, model.PerformanceGoals, model.AppraiserID);
                    if (isUpdated)
                    {
                        PmsActivityHistory activityHistory = new PmsActivityHistory();
                        var entity = await _performanceService.GetReviewHeaderAsync(model.AppraiseeID, model.ReviewSessionID);
                        if (entity != null)
                        {
                            activityHistory.ReviewHeaderId = entity.ReviewHeaderId;
                            activityHistory.ReviewSessionId = entity.ReviewSessionId;
                            activityHistory.ActivityTime = DateTime.UtcNow;
                            activityHistory.ActivityDescription = $"Updated performance goal(s) for this appraisal session.";
                            await _performanceService.AddPmsActivityHistoryAsync(activityHistory);
                        }
                        model.ViewModelSuccessMessage = "Performance Goal updated successfully!";
                        model.OperationIsSuccessful = true;
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Missing Parameter Error. The operation failed because a key parameter is missing.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var reporting_entities = await _ermService.GetActiveEmployeeReportLinesByEmployeeIdAsync(model.AppraiseeID);
            if (reporting_entities != null && reporting_entities.Count > 0)
            {
                reports = reporting_entities;
            }
            ViewBag.ReportList = new SelectList(reports, "ReportsToEmployeeID", "ReportsToEmployeeName");
            return View(model);
        }

        public async Task<IActionResult> ShowPerformanceGoal(int id)
        {
            CreatePerformanceGoalsViewModel model = new CreatePerformanceGoalsViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.PerformanceGoals = reviewHeader.PerformanceGoal;
                    model.AppraiserName = reviewHeader.PrimaryAppraiserName;
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        #endregion

        #region MoveToNextStep Controller Actions

        public async Task<IActionResult> MoveToNextStep(int id, string ad = null)
        {
            MoveToNextStageModel model = new MoveToNextStageModel();
            if (id > 0)
            {
                model = await _performanceService.ValidateMoveRequestAsync(id, ad);
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToNextStep(MoveToNextStageModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ReviewHeaderID > 0 && model.NextStageID > 1)
                {
                    await _performanceService.UpdateReviewHeaderStageAsync(model.ReviewHeaderID, model.NextStageID);
                }
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            return RedirectToAction("MyAppraisalSteps", new { id = model.ReviewSessionID });
        }

        public async Task<IActionResult> MoveToPreviousStep(int id)
        {
            MoveToPreviousStepModel model = new MoveToPreviousStepModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            List<ReviewStage> ReviewStages = new List<ReviewStage>();
            if (id > 0)
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.CurrentStageDescription = reviewHeader.ReviewStageDescription;
                    model.CurrentStageID = reviewHeader.ReviewStageId;
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;

                    var entities = await _performanceService.GetPreviousReviewStagesAsync(reviewHeader.ReviewStageId);
                    if (entities != null && entities.Count > 0)
                    {
                        ReviewStages = entities;
                    }
                }
                else { model.ViewModelErrorMessage = "The selected record could not be retrieved at this time. Please try again."; }
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            ViewBag.ReviewStageList = new SelectList(ReviewStages, "ReviewStageId", "ReviewStageName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToPreviousStep(MoveToPreviousStepModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ReviewHeaderID > 0 && model.NewStageID > 0 && model.CurrentStageID < 6)
                {
                    await _performanceService.UpdateReviewHeaderStageAsync(model.ReviewHeaderID, model.NewStageID);
                }
            }
            else
            {
                return RedirectToAction("MyAppraisalSessions");
            }
            return RedirectToAction("MyAppraisalSteps", new { id = model.ReviewSessionID });
        }
        #endregion

        #region Appraisal KPAs Controller Actions

        public async Task<IActionResult> AppraisalKpas(int id)
        {
            AppraisalKpaViewModel model = new AppraisalKpaViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetKpasAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMetricList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        public async Task<IActionResult> ShowAppraisalKpas(int id)
        {
            AppraisalKpaViewModel model = new AppraisalKpaViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetKpasAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMetricList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        public async Task<IActionResult> ManageAppraisalKpa(int id, int? md = null)
        {
            ManageAppraisalKpaViewModel model = new ManageAppraisalKpaViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                model.ReviewHeaderId = id;
                if (id > 0)
                {
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionId = reviewHeader.ReviewSessionId;
                        model.ReviewYearId = reviewHeader.ReviewYearId;
                        model.AppraiseeId = reviewHeader.AppraiseeId;
                    }
                }

                if (md != null && md.Value > 0)
                {
                    ReviewMetric reviewMetric = await _performanceService.GetReviewMetricAsync(md.Value);
                    if (reviewMetric != null)
                    {
                        model.ReviewMetricId = reviewMetric.ReviewMetricId;
                        model.ReviewMetricDescription = reviewMetric.ReviewMetricDescription;
                        model.ReviewMetricKpi = reviewMetric.ReviewMetricKpi;
                        model.ReviewMetricTarget = reviewMetric.ReviewMetricTarget;
                        model.ReviewMetricWeightage = reviewMetric.ReviewMetricWeightage;
                    }
                }

            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAppraisalKpa(ManageAppraisalKpaViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isUpdated = await _performanceService.UpdateReviewMetricAsync(reviewMetric);
                        if (isUpdated)
                        {
                            model.ViewModelSuccessMessage = "KPA updated successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        bool isAdded = await _performanceService.AddReviewMetricAsync(reviewMetric);
                        if (isAdded)
                        {
                            return RedirectToAction("AppraisalKpas", new { id = model.ReviewHeaderId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
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

        public async Task<IActionResult> DeleteAppraisalKpa(int id)
        {
            ManageAppraisalKpaViewModel model = new ManageAppraisalKpaViewModel();
            ReviewMetric reviewMetric = new ReviewMetric();
            try
            {
                if (id > 0)
                {
                    reviewMetric = await _performanceService.GetReviewMetricAsync(id);
                    if (reviewMetric != null)
                    {
                        model.ReviewMetricId = reviewMetric.ReviewMetricId;
                        model.ReviewMetricDescription = reviewMetric.ReviewMetricDescription;
                        model.ReviewMetricKpi = reviewMetric.ReviewMetricKpi;
                        model.ReviewMetricTarget = reviewMetric.ReviewMetricTarget;
                        model.ReviewMetricWeightage = reviewMetric.ReviewMetricWeightage;
                        model.ReviewSessionId = reviewMetric.ReviewSessionId;
                        model.ReviewYearId = reviewMetric.ReviewYearId;
                        model.AppraiseeId = reviewMetric.AppraiseeId;
                        model.ReviewHeaderId = reviewMetric.ReviewHeaderId;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppraisalKpa(ManageAppraisalKpaViewModel model)
        {
            try
            {
                if (model.ReviewMetricId != null && model.ReviewMetricId > 0)
                {
                    bool isDeleted = await _performanceService.DeleteReviewMetricAsync(model.ReviewMetricId.Value);
                    if (isDeleted)
                    {
                        return RedirectToAction("AppraisalKpas", new { id = model.ReviewHeaderId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted delete operation failed.";
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

        #region Appraisal Competencies Controller Actions

        public async Task<IActionResult> AppraisalCompetencies(int id)
        {
            AppraisalCompetenciesViewModel model = new AppraisalCompetenciesViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetCompetenciesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMetricList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        public async Task<IActionResult> ShowAppraisalCompetencies(int id)
        {
            AppraisalCompetenciesViewModel model = new AppraisalCompetenciesViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetCompetenciesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMetricList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            return View(model);
        }

        public async Task<IActionResult> SelectCompetencies(int id, int vd, int cd)
        {
            SelectCompetencyViewModel model = new SelectCompetencyViewModel();
            if (id > 0)
            {
                model.ReviewHeaderID = id;
                var entities = await _performanceService.SearchFromCompetencyDictionaryAsync(cd, vd);
                if (entities != null && entities.Count > 0)
                {
                    model.CompetenciesList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            var levelEntities = await _performanceService.GetCompetencyLevelsAsync();
            var categoryEntities = await _performanceService.GetCompetencyCategoriesAsync();

            ViewBag.LevelList = new SelectList(levelEntities, "Id", "Description", vd);
            ViewBag.CategoryList = new SelectList(categoryEntities, "Id", "Description", cd);
            return View(model);
        }

        public async Task<IActionResult> AddCompetency(int id, int rd)
        {
            AddCompetencyViewModel model = new AddCompetencyViewModel();
            ReviewMetric reviewMetric = new ReviewMetric();
            ReviewHeader reviewHeader = new ReviewHeader();
            if (id < 1)
            {
                model.ViewModelErrorMessage = "Sorry, a key parameter is missing thus the selected Competency could not be retrieved. Please try again.";
                model.ReviewHeaderId = rd;
            }
            else
            {
                Competency competency = new Competency();
                competency = await _performanceService.GetFromCompetencyDictionaryByIdAsync(id);
                if (competency != null && !string.IsNullOrWhiteSpace(competency.Description))
                {
                    model.CompetencyId = competency.Id;
                    model.ReviewMetricDescription = competency.Description;
                    if (rd < 1)
                    {
                        model.ViewModelErrorMessage = "Sorry, a key parameter is missing thus the selected Competency could not be retrieved. Please try again.";
                    }
                    else
                    {
                        reviewHeader = await _performanceService.GetReviewHeaderAsync(rd);
                        if (reviewHeader != null)
                        {
                            model.AppraiseeId = reviewHeader.AppraiseeId;
                            model.AppraiseeName = reviewHeader.AppraiseeName;
                            model.ReviewHeaderId = reviewHeader.ReviewHeaderId;
                            model.ReviewSessionId = reviewHeader.ReviewSessionId;
                            model.ReviewYearId = reviewHeader.ReviewYearId;
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCompetency(AddCompetencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isUpdated = await _performanceService.UpdateReviewMetricAsync(reviewMetric);
                        if (isUpdated)
                        {
                            model.ViewModelSuccessMessage = "Competency updated successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        bool isAdded = await _performanceService.AddReviewMetricAsync(reviewMetric);
                        if (isAdded)
                        {
                            return RedirectToAction("SelectCompetencies", new { id = model.ReviewHeaderId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        public async Task<IActionResult> ManageAppraisalCompetency(int id)
        {
            AddCompetencyViewModel model = new AddCompetencyViewModel();
            ReviewMetric reviewMetric = new ReviewMetric();
            ReviewHeader reviewHeader = new ReviewHeader();
            if (id < 1)
            {
                model.ViewModelErrorMessage = "Sorry, a key parameter is missing thus the selected Competency could not be retrieved. Please try again.";
            }
            else
            {
                reviewMetric = await _performanceService.GetReviewMetricAsync(id);
                if (reviewMetric != null)
                {
                    model.AppraiseeId = reviewMetric.AppraiseeId;
                    model.AppraiseeName = reviewMetric.AppraiseeName;
                    model.ReviewHeaderId = reviewMetric.ReviewHeaderId;
                    model.ReviewSessionId = reviewMetric.ReviewSessionId;
                    model.ReviewYearId = reviewMetric.ReviewYearId;
                    model.ReviewMetricDescription = reviewMetric.ReviewMetricDescription;
                    model.ReviewMetricId = reviewMetric.ReviewMetricId;
                    model.ReviewMetricWeightage = reviewMetric.ReviewMetricWeightage;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAppraisalCompetency(AddCompetencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isUpdated = await _performanceService.UpdateReviewMetricAsync(reviewMetric);
                        if (isUpdated)
                        {
                            model.ViewModelSuccessMessage = "Competency updated successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        bool isAdded = await _performanceService.AddReviewMetricAsync(reviewMetric);
                        if (isAdded)
                        {
                            model.ViewModelSuccessMessage = "Competency added successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        public async Task<IActionResult> DeleteAppraisalCompetency(int id)
        {
            AddCompetencyViewModel model = new AddCompetencyViewModel();
            ReviewMetric reviewMetric = new ReviewMetric();
            ReviewHeader reviewHeader = new ReviewHeader();
            if (id < 1)
            {
                model.ViewModelErrorMessage = "Sorry, a key parameter is missing thus the selected Competency could not be retrieved. Please try again.";
            }
            else
            {
                reviewMetric = await _performanceService.GetReviewMetricAsync(id);
                if (reviewMetric != null)
                {
                    model.AppraiseeId = reviewMetric.AppraiseeId;
                    model.AppraiseeName = reviewMetric.AppraiseeName;
                    model.ReviewHeaderId = reviewMetric.ReviewHeaderId;
                    model.ReviewSessionId = reviewMetric.ReviewSessionId;
                    model.ReviewSessionDescription = reviewMetric.ReviewSessionDescription;
                    model.ReviewYearId = reviewMetric.ReviewYearId;
                    model.ReviewYearName = reviewMetric.ReviewYearName;
                    model.ReviewMetricDescription = reviewMetric.ReviewMetricDescription;
                    model.ReviewMetricId = reviewMetric.ReviewMetricId;
                    model.ReviewMetricWeightage = reviewMetric.ReviewMetricWeightage;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppraisalCompetency(AddCompetencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMetric reviewMetric = new ReviewMetric();
                reviewMetric = model.ConvertToReviewMetric();
                try
                {
                    if (reviewMetric.ReviewMetricId > 0)
                    {
                        bool isDeleted = await _performanceService.DeleteReviewMetricAsync(reviewMetric.ReviewMetricId);
                        if (isDeleted)
                        {
                            return RedirectToAction("AppraisalCompetencies", new { id = model.ReviewHeaderId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted delete operation failed.";
                        }
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "A key parameter is missing. The delete operation failed. Please try again.";
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

        #region Appraisal Career Development Goals Controller Actions

        public async Task<IActionResult> AppraisalCdgs(int id)
        {
            AppraisalCdgViewModel model = new AppraisalCdgViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetReviewCdgsAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewCdgList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }
            return View(model);
        }

        public async Task<IActionResult> ShowAppraisalCdgs(int id)
        {
            AppraisalCdgViewModel model = new AppraisalCdgViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetReviewCdgsAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewCdgList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }
            return View(model);
        }

        public async Task<IActionResult> ManageAppraisalCdg(int id, int? cd = null)
        {
            ManageAppraisalCdgViewModel model = new ManageAppraisalCdgViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                model.ReviewHeaderId = id;
                if (id > 0)
                {
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionId = reviewHeader.ReviewSessionId;
                        model.ReviewYearId = reviewHeader.ReviewYearId;
                        model.AppraiseeId = reviewHeader.AppraiseeId;
                    }
                }

                if (cd != null && cd.Value > 0)
                {
                    ReviewCDG reviewCDG = await _performanceService.GetReviewCdgAsync(cd.Value);
                    if (reviewCDG != null)
                    {
                        model.ReviewCdgId = reviewCDG.ReviewCdgId;
                        model.ReviewCdgDescription = reviewCDG.ReviewCdgDescription;
                        model.AppraiseeId = reviewCDG.AppraiseeId;
                        model.ReviewCdgObjective = reviewCDG.ReviewCdgObjective;
                        model.ReviewCdgActionPlan = reviewCDG.ReviewCdgActionPlan;
                        model.ReviewHeaderId = reviewCDG.ReviewHeaderId;
                        model.ReviewSessionId = reviewCDG.ReviewSessionId;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAppraisalCdg(ManageAppraisalCdgViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewCDG reviewCDG = new ReviewCDG();
                reviewCDG = model.ConvertToReviewCdg();
                try
                {
                    if (reviewCDG.ReviewCdgId > 0)
                    {
                        bool isUpdated = await _performanceService.UpdateReviewCdgAsync(reviewCDG);
                        if (isUpdated)
                        {
                            model.ViewModelSuccessMessage = "CDG updated successfully!";
                            model.OperationIsSuccessful = true;
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
                        }
                    }
                    else
                    {
                        bool isAdded = await _performanceService.AddReviewCdgAsync(reviewCDG);
                        if (isAdded)
                        {
                            model.ViewModelSuccessMessage = "CDG added successfully!";
                            model.OperationIsSuccessful = true;
                            return RedirectToAction("AppraisalCdgs", new { id = model.ReviewHeaderId });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted update failed.";
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

        public async Task<IActionResult> DeleteAppraisalCdg(int id)
        {
            ManageAppraisalCdgViewModel model = new ManageAppraisalCdgViewModel();
            ReviewCDG reviewCDG = new ReviewCDG();
            try
            {
                if (id > 0)
                {
                    reviewCDG = await _performanceService.GetReviewCdgAsync(id);
                    if (reviewCDG != null)
                    {
                        model.ReviewCdgId = reviewCDG.ReviewCdgId;
                        model.ReviewCdgDescription = reviewCDG.ReviewCdgDescription;
                        model.AppraiseeId = reviewCDG.AppraiseeId;
                        model.AppraiseeName = reviewCDG.AppraiseeName;
                        model.ReviewCdgObjective = reviewCDG.ReviewCdgObjective;
                        model.ReviewCdgActionPlan = reviewCDG.ReviewCdgActionPlan;
                        model.ReviewSessionId = reviewCDG.ReviewSessionId;
                        model.ReviewSessionDescription = reviewCDG.ReviewSessionName;
                        model.ReviewYearId = reviewCDG.ReviewYearId;
                        model.AppraiseeId = reviewCDG.AppraiseeId;
                        model.ReviewHeaderId = reviewCDG.ReviewHeaderId;
                    }
                }

            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppraisalCdg(ManageAppraisalCdgViewModel model)
        {
            try
            {
                if (model != null && model.ReviewCdgId != null && model.ReviewCdgId > 0)
                {
                    bool isDeleted = await _performanceService.DeleteReviewCdgAsync(model.ReviewCdgId.Value);
                    if (isDeleted)
                    {
                        return RedirectToAction("AppraisalCdgs", new { id = model.ReviewHeaderId });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted delete operation failed.";
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

        #region Appraisal Submissions Controller Actions
        public async Task<IActionResult> AppraisalSubmissionHistory(int id)
        {
            AppraisalSubmissionHistoryViewModel model = new AppraisalSubmissionHistoryViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetReviewSubmissionsByReviewHeaderIdAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSubmissionList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }

            return View(model);
        }

        public async Task<IActionResult> SubmitAppraisal(int id)
        {
            SubmitAppraisalViewModel model = new SubmitAppraisalViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                model.ReviewHeaderID = id;
                if (id > 0)
                {
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.FromEmployeeID = reviewHeader.AppraiseeId;
                        model.ToEmployeeID = reviewHeader.PrimaryAppraiserId;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            var role_entities = await _performanceService.GetApprovalRolesAsync();
            ViewBag.ApproverRolesList = new SelectList(role_entities, "ApprovalRoleId", "ApprovalRoleName", model.ToEmployeeID);

            var entities = await _ermService.GetEmployeeReportLinesByEmployeeIdAsync(model.AppraiseeID);
            ViewBag.ReportingLinesList = new SelectList(entities, "ReportsToEmployeeID", "ReportsToEmployeeName", model.ToEmployeeID);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAppraisal(SubmitAppraisalViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewSubmission reviewSubmission = new ReviewSubmission();
                reviewSubmission = model.ConvertToReviewSubmission();
                reviewSubmission.TimeSubmitted = DateTime.UtcNow;
                ReviewHeader reviewHeader = new ReviewHeader();
                try
                {
                    // Check if this submission has already been actioned by the recipient.
                    var reviewHeader_entity = await _performanceService.GetReviewHeaderAsync(reviewSubmission.ReviewHeaderId);
                    if (reviewHeader_entity != null)
                    {
                        reviewHeader = reviewHeader_entity;
                        if (reviewSubmission.SubmissionPurposeId == 1)
                        {
                            var approvalEntities = await _performanceService.GetReviewApprovalsApprovedAsync(reviewSubmission.ReviewHeaderId, 1, reviewSubmission.ToEmployeeRoleId);
                            if (approvalEntities != null && approvalEntities.Count > 0)
                            {
                                ReviewApproval reviewApproval = approvalEntities.FirstOrDefault();
                                model.ViewModelWarningMessage = $"{reviewApproval.ApproverName} has already approved as {reviewApproval.ApproverRoleDescription} on {reviewApproval.ApprovedTime.Value.ToLongDateString()}.";

                                var role_entities = await _performanceService.GetApprovalRolesAsync();
                                ViewBag.ApproverRolesList = new SelectList(role_entities, "ApprovalRoleId", "ApprovalRoleName", model.ToEmployeeID);

                                var entities = await _ermService.GetEmployeeReportLinesByEmployeeIdAsync(model.AppraiseeID);
                                ViewBag.ReportingLinesList = new SelectList(entities, "ReportsToEmployeeID", "ReportsToEmployeeName", model.ToEmployeeID);

                                return View(model);
                            }
                        }
                        else if(reviewSubmission.SubmissionPurposeId == 2)
                        {
                            var resultEntities = await _performanceService.GetReviewResultByAppraiserIdAndReviewHeaderIdAsync(reviewSubmission.ReviewHeaderId, reviewSubmission.ToEmployeeId);
                            if (resultEntities != null && resultEntities.Count > 0)
                            {
                                ReviewResult reviewResult = resultEntities.FirstOrDefault();
                                model.ViewModelWarningMessage = $"Sorry, double evaluation is not permitted. {reviewResult.AppraiserName} has already evaluated you as {reviewResult.AppraiserRoleName} on {reviewResult.ScoreTime.Value.ToLongDateString()}.";

                                var role_entities = await _performanceService.GetApprovalRolesAsync();
                                ViewBag.ApproverRolesList = new SelectList(role_entities, "ApprovalRoleId", "ApprovalRoleName", model.ToEmployeeID);

                                var entities = await _ermService.GetEmployeeReportLinesByEmployeeIdAsync(model.AppraiseeID);
                                ViewBag.ReportingLinesList = new SelectList(entities, "ReportsToEmployeeID", "ReportsToEmployeeName", model.ToEmployeeID);

                                return View(model);
                            }
                        }
                        else if (reviewSubmission.SubmissionPurposeId == 3)
                        {
                            var approvalEntities = await _performanceService.GetReviewApprovalsApprovedAsync(reviewSubmission.ReviewHeaderId, 2, reviewSubmission.ToEmployeeRoleId);
                            if (approvalEntities != null && approvalEntities.Count > 0)
                            {
                                ReviewApproval reviewApproval = approvalEntities.FirstOrDefault();
                                model.ViewModelWarningMessage = $"{reviewApproval.ApproverName} has already approved as {reviewApproval.ApproverRoleDescription} on {reviewApproval.ApprovedTime.Value.ToLongDateString()}.";


                                var role_entities = await _performanceService.GetApprovalRolesAsync();
                                ViewBag.ApproverRolesList = new SelectList(role_entities, "ApprovalRoleId", "ApprovalRoleName", model.ToEmployeeID);

                                var entities = await _ermService.GetEmployeeReportLinesByEmployeeIdAsync(model.AppraiseeID);
                                ViewBag.ReportingLinesList = new SelectList(entities, "ReportsToEmployeeID", "ReportsToEmployeeName", model.ToEmployeeID);
                                return View(model);
                            }
                        }
                    }
                    // End of checking

                    bool isAdded = await _performanceService.AddReviewSubmissionAsync(reviewSubmission);
                    if (isAdded)
                    {
                        Employee sender = new Employee();
                        sender = await _ermService.GetEmployeeByIdAsync(model.FromEmployeeID);
                        Employee approver = new Employee();
                        approver = await _ermService.GetEmployeeByIdAsync(model.ToEmployeeID);

                        //====== Add Appraisal Activity History =======//
                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Appraisal was submitted to {approver.FullName} by {sender.FullName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()}";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        history.ReviewSessionId = model.ReviewSessionID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);

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
                        recipientEmailCopy.RecipientEmail = approver.OfficialEmail;
                        recipientEmailCopy.SenderName = sender.FullName;
                        switch (reviewSubmission.SubmissionPurposeId)
                        {
                            case 1:
                                recipientEmailCopy.Subject = "Request for Performance Contract Approval";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetPerformanceContractApprovalEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetPerformanceContractApprovalEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Request for Performance Contract Approval";
                                message.MessageBody = UtilityHelper.GetPerformanceContractApprovalMessageContent(approver.FullName, sender.FullName);
                                break;
                            case 2:
                                recipientEmailCopy.Subject = "Request for Final Performance Evaluation";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetRequestForFinalEvaluationEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetRequestForFinalEvaluationEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Request for Final Performance Evaluation";
                                message.MessageBody = UtilityHelper.GetRequestForFinalEvaluationMessageContent(approver.FullName, sender.FullName);

                                break;
                            case 3:
                                recipientEmailCopy.Subject = "Request for Performance Evaluation Result Approval";
                                recipientEmailCopy.HtmlContent = UtilityHelper.GetRequestForEvaluationResultApprovalEmailHtmlContent(approver.FullName, sender.FullName);
                                recipientEmailCopy.PlainContent = UtilityHelper.GetRequestForEvaluationResultApprovalEmailPlainContent(approver.FullName, sender.FullName);

                                message.Subject = "Request for Performance Evaluation Result Approval";
                                message.MessageBody = UtilityHelper.GetRequestForEvaluationResultApprovalMessageContent(approver.FullName, sender.FullName);
                                break;
                            default:
                                break;
                        }

                        bool messageSent = await _baseModelService.SendMessageAsync(message);
                        approverEmailCopySent = utilityHelper.SendEmailWithSendGrid(recipientEmailCopy);
                        return RedirectToAction("AppraisalSubmissionHistory", new { id = model.ReviewHeaderID });
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

        public async Task<IActionResult> AppraisalSubmissionDetails(int id)
        {
            SubmitAppraisalViewModel model = new SubmitAppraisalViewModel();
            ReviewSubmission reviewSubmission = new ReviewSubmission();
            try
            {
                model.ReviewSubmissionID = id;
                if (id > 0)
                {
                    reviewSubmission = await _performanceService.GetReviewSubmissionByIdAsync(id);
                    if (reviewSubmission != null)
                    {
                        model.ReviewSessionID = reviewSubmission.ReviewSessionId;
                        model.FromEmployeeID = reviewSubmission.FromEmployeeId;
                        model.FromEmployeeName = reviewSubmission.FromEmployeeName;
                        model.ReviewHeaderID = reviewSubmission.ReviewHeaderId;
                        model.ReviewSubmissionID = reviewSubmission.ReviewSubmissionId;
                        model.SubmissionMessage = reviewSubmission.SubmissionMessage;
                        model.SubmissionPurposeDescription = reviewSubmission.SubmissionPurposeDescription;
                        model.SubmissionPurposeID = reviewSubmission.SubmissionPurposeId;
                        model.TimeSubmitted = reviewSubmission.TimeSubmitted;
                        model.ToEmployeeID = reviewSubmission.ToEmployeeId;
                        model.ToEmployeeName = reviewSubmission.ToEmployeeName;
                        model.ToEmployeeRoleID = reviewSubmission.ToEmployeeRoleId;
                        model.ToEmployeeRoleName = reviewSubmission.ToEmployeeRoleName;

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

        #region Appraisals Submitted To Me Controller Actions

        public async Task<IActionResult> AppraisalsSubmittedtoMe(int? id)
        {
            AppraisalsSubmittedtoMeViewModel model = new AppraisalsSubmittedtoMeViewModel();
            model.id = id;

            ApplicationUser user = new ApplicationUser();
            string userId = string.Empty;
            string userFullName = string.Empty;
            userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                return LocalRedirect("/Home/Login");
            }

            model.AppraiserName = HttpContext.User.Identity.Name;
            model.AppraiserID = userId;

            if (model.id != null && model.id.Value > 0)
            {
                model.id = id.Value;
                var entities = await _performanceService.GetReviewSubmissionsByApproverIdAsync(model.AppraiserID, model.id.Value);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSubmissionList = entities.ToList();
                }
            }
            else
            {
                var entities = await _performanceService.GetReviewSubmissionsByApproverIdAsync(model.AppraiserID, null);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewSubmissionList = entities.ToList();
                }
            }

            List<ReviewSession> reviewSessions = await _performanceService.GetReviewSessionsAsync();
            if (reviewSessions != null && reviewSessions.Count > 0)
            {
                ViewBag.ReviewSessionList = new SelectList(reviewSessions, "Id", "Name", id);
            }

            return View(model);
        }

        public async Task<IActionResult> AppraisalInfo(int id, int sd, string src)
        {
            AppraisalInfoViewModel model = new AppraisalInfoViewModel();
            model.SourcePage = src;
            ReviewHeader reviewHeader = new ReviewHeader();
            List<ReviewMetric> reviewKpas = new List<ReviewMetric>();
            List<ReviewMetric> reviewCmps = new List<ReviewMetric>();
            List<ReviewCDG> reviewCDGs = new List<ReviewCDG>();

            if (sd > 0) { model.ReviewSubmissionID = sd; }

            if (id > 0)
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null) { model.AppraisalReviewHeader = reviewHeader; }
                reviewKpas = await _performanceService.GetKpasAsync(id);
                if (reviewKpas != null && reviewKpas.Count > 0) { model.KpaList = reviewKpas; }
                reviewCmps = await _performanceService.GetCompetenciesAsync(id);
                if (reviewCmps != null && reviewCmps.Count > 0) { model.CompetencyList = reviewCmps; }
                reviewCDGs = await _performanceService.GetReviewCdgsAsync(id);
                if (reviewCDGs != null && reviewCDGs.Count > 0) { model.CdgList = reviewCDGs; }
            }

            return View(model);
        }

        public async Task<IActionResult> ReturnContract(int id, int sd)
        {
            ReturnContractViewModel model = new ReturnContractViewModel();
            ReviewMessage reviewMessage = new ReviewMessage();
            ReviewSubmission reviewSubmission = new ReviewSubmission();
            try
            {
                if (id > 0) { model.ReviewHeaderID = id; }
                if (sd > 0)
                {
                    model.ReviewSubmissionID = sd;
                    reviewSubmission = await _performanceService.GetReviewSubmissionByIdAsync(sd);
                    if (reviewSubmission != null)
                    {
                        model.FromEmployeeID = reviewSubmission.FromEmployeeId;
                        model.ReviewHeaderID = reviewSubmission.ReviewHeaderId;
                        model.ReviewSessionID = reviewSubmission.ReviewSessionId;
                        model.FromEmployeeName = reviewSubmission.FromEmployeeName;
                    }
                }

                model.LoggedInEmployeeID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnContract(ReturnContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ReviewMessage reviewMessage = new ReviewMessage();
                    if (!string.IsNullOrWhiteSpace(model.MessageBody))
                    {
                        reviewMessage = model.ConvertToReviewMessage();
                        reviewMessage.MessageTime = DateTime.UtcNow;
                        reviewMessage.FromEmployeeId = model.LoggedInEmployeeID;
                    }
                    else { reviewMessage = null; }

                    bool isReturned = await _performanceService.ReturnContractToAppraisee(1, model.ReviewSubmissionID, reviewMessage);
                    if (isReturned)
                    {
                        model.ViewModelSuccessMessage = "Returned to Appraisee successfully!";
                        model.OperationIsSuccessful = true;
                        //int reviewSessionId = 0;
                        //var reviewHeader = await _performanceService.GetReviewHeaderAsync(model.ReviewHeaderID);
                        //if(reviewHeader != null) { reviewSessionId = reviewHeader.ReviewSessionId; }

                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Performance Contract was not approved by {model.FromEmployeeName}. The appraisal was returned for corrections on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        history.ReviewSessionId = model.ReviewSessionID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);

                        return RedirectToAction("AppraisalInfo", new {id = model.ReviewHeaderID, sd = model.ReviewSubmissionID, src = "stm" });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ApproveContract(int id, int sd)
        {
            ApproveContractViewModel model = new ApproveContractViewModel();
            model.SubmissionID = sd;
            model.ApprovalTypeID = (int)ReviewApprovalType.ApprovePerformanceContract;

            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    }
                }

                model.ApproverID = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                model.ApproverName = HttpContext.User.Identity.Name;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            List<ApprovalRole> roles = new List<ApprovalRole>();
            var role_entities = await _performanceService.GetApprovalRolesAsync();
            if (role_entities != null) { roles = role_entities; }
            ViewBag.RoleList = new SelectList(roles, "ApprovalRoleId", "ApprovalRoleName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveContract(ApproveContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewApproval reviewApproval = model.ConvertToReviewApproval();
                reviewApproval.IsApproved = true;
                reviewApproval.ApprovedTime = DateTime.UtcNow;
                reviewApproval.ApprovalTypeId = (int)ReviewApprovalType.ApprovePerformanceContract;
                try
                {
                    ApprovalRole approvalRole = new ApprovalRole();
                    approvalRole = await _performanceService.GetApprovalRoleAsync(reviewApproval.ApproverRoleId);
                    if (approvalRole != null) { model.ApproverRoleDescription = approvalRole.ApprovalRoleName; }

                    bool isApproved = await _performanceService.ApproveContractToAppraisee(reviewApproval, model.SubmissionID);
                    if (isApproved)
                    {
                        model.ViewModelSuccessMessage = "Appraisal Approved successfully!";
                        model.OperationIsSuccessful = true;

                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Performance Contract was approved by {model.ApproverName} as {model.ApproverRoleDescription} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        history.ReviewSessionId = model.ReviewSessionID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);

                        return RedirectToAction("AppraisalInfo", new { id = model.ReviewHeaderID, sd = model.SubmissionID, src = "stm" });

                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            List<ApprovalRole> roles = new List<ApprovalRole>();
            var role_entities = await _performanceService.GetApprovalRolesAsync();
            if (role_entities != null) { roles = role_entities; }
            ViewBag.RoleList = new SelectList(roles, "ApprovalRoleId", "ApprovalRoleName");

            return View(model);
        }
        #endregion

        #region Appraisee Accept Contract & Evaluation
        public async Task<IActionResult> AcceptContract(int id)
        {
            AcceptContractViewModel model = new AcceptContractViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptContract(AcceptContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isAccepted = await _performanceService.AcceptContractByAppraisee(model.ReviewHeaderID);
                    if (isAccepted)
                    {
                        model.ViewModelSuccessMessage = "Performance Contract Agreement Signed Off successfully!";
                        model.OperationIsSuccessful = true;

                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Performance Contract was agreed to and signed off by {model.AppraiseeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        history.ReviewSessionId = model.ReviewSessionID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);

                        return RedirectToAction("MyAppraisalSteps", new { id = model.ReviewSessionID});
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> AcceptEvaluation(int id)
        {
            AcceptContractViewModel model = new AcceptContractViewModel();
            model.IsNotAccepted = false;
            //model.RejectionReason = "None";

            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptEvaluation(AcceptContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool resultUploaded = await _performanceService.UploadResults(model.ReviewHeaderID);
                    if (!resultUploaded)
                    {
                        model.ViewModelErrorMessage = "An error was encountered while trying to process your evaluation result. The attempted operation could not be completed.";
                    }
                    else
                    {
                        bool isSignedOff = await _performanceService.AcceptEvaluationByAppraisee(model.ReviewHeaderID);
                        if (isSignedOff)
                        {
                            model.ViewModelSuccessMessage = "Performance Evaluation Result Signed Off successfully!";
                            model.OperationIsSuccessful = true;

                            PmsActivityHistory history = new PmsActivityHistory();
                            history.ActivityDescription = $"Performance Evaluation Result was signed off by {model.AppraiseeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                            history.ActivityTime = DateTime.UtcNow;
                            history.ReviewHeaderId = model.ReviewHeaderID;
                            history.ReviewSessionId = model.ReviewSessionID;
                            await _performanceService.AddPmsActivityHistoryAsync(history);
                            //return RedirectToAction("MyAppraisalSteps", "Process", new { id = model.ReviewSessionID });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        public async Task<IActionResult> RejectEvaluation(int id)
        {
            RejectEvaluationViewModel model = new RejectEvaluationViewModel();
            model.IsNotAccepted = true;

            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectEvaluation(RejectEvaluationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool resultUploaded = await _performanceService.UploadResults(model.ReviewHeaderID);
                    if (!resultUploaded)
                    {
                        model.ViewModelErrorMessage = "An error was encountered while trying to process your evaluation result. The attempted operation could not be completed.";
                    }
                    else
                    {
                        if (model.IsNotAccepted)
                        {
                            await _performanceService.UpdateAppraiseeFlagAsync(model.ReviewHeaderID, true, model.RejectionReason);
                        }

                        bool isSignedOff = await _performanceService.AcceptEvaluationByAppraisee(model.ReviewHeaderID);
                        if (isSignedOff)
                        {
                            model.ViewModelSuccessMessage = "Performance Evaluation Result was Signed Off successfully!";
                            model.OperationIsSuccessful = true;

                            PmsActivityHistory history = new PmsActivityHistory();
                            history.ActivityDescription = $"Performance Evaluation Result was signed off by {model.AppraiseeName} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} GMT";
                            history.ActivityTime = DateTime.UtcNow;
                            history.ReviewHeaderId = model.ReviewHeaderID;
                            history.ReviewSessionId = model.ReviewSessionID;
                            await _performanceService.AddPmsActivityHistoryAsync(history);
                            //return RedirectToAction("MyAppraisalSteps", "Process", new { id = model.ReviewSessionID });
                        }
                        else
                        {
                            model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        public async Task<IActionResult> ShowAppraisalApprovals(int id)
        {
            AppraisalApprovalViewModel model = new AppraisalApprovalViewModel();
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewStageID = reviewHeader.ReviewStageId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                }

                var entities = await _performanceService.GetReviewApprovalsAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewApprovalList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }
            return View(model);
        }

        public async Task<IActionResult> ShowAppraiseeAgreement(int id)
        {
            AcceptContractViewModel model = new AcceptContractViewModel();
            try
            {
                if (id > 0)
                {
                    ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
                        model.SignedOffTimeFormatted = $"{reviewHeader.TimeContractAccepted.Value.ToLongDateString()} {reviewHeader.TimeContractAccepted.Value.ToLongTimeString()} GMT";
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> ShowContractInfo(int id)
        {
            AppraisalInfoViewModel model = new AppraisalInfoViewModel();
            ReviewHeader reviewHeader = new ReviewHeader();
            List<ReviewMetric> reviewKpas = new List<ReviewMetric>();
            List<ReviewMetric> reviewCmps = new List<ReviewMetric>();
            List<ReviewCDG> reviewCDGs = new List<ReviewCDG>();

            if (id > 0)
            {
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null) { model.AppraisalReviewHeader = reviewHeader; }
                reviewKpas = await _performanceService.GetKpasAsync(id);
                if (reviewKpas != null && reviewKpas.Count > 0) { model.KpaList = reviewKpas; }
                reviewCmps = await _performanceService.GetCompetenciesAsync(id);
                if (reviewCmps != null && reviewCmps.Count > 0) { model.CompetencyList = reviewCmps; }
                reviewCDGs = await _performanceService.GetReviewCdgsAsync(id);
                if (reviewCDGs != null && reviewCDGs.Count > 0) { model.CdgList = reviewCDGs; }
            }

            return View(model);
        }

        public async Task<IActionResult> ShowEvaluationSignOff(int id)
        {
            AcceptContractViewModel model = new AcceptContractViewModel();
            try
            {
                if (id > 0)
                {
                    ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.IsNotAccepted = reviewHeader.IsFlagged;
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewYearName = reviewHeader.ReviewYearName;
                        model.SignedOffTimeFormatted = $"{reviewHeader.TimeEvaluationAccepted.Value.ToLongDateString()} {reviewHeader.TimeEvaluationAccepted.Value.ToLongTimeString()} GMT";
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        #endregion

        #region Approve Evaluation

        public async Task<IActionResult> FullEvaluationResult(int id, int sd)
        {
            ViewBag.ReviewSubmissionID = sd;

            ShowFullResultViewModel model = new ShowFullResultViewModel();
            model.EvaluationSummaryResult = new EvaluationResultViewModel();
            model.KpaFullResult = new EvaluationListViewModel();
            model.CmpFullResult = new EvaluationListViewModel();

            model.EvaluationSummaryResult.ReviewHeaderID = id;
            if (id > 0)
            {
                ReviewHeader reviewHeader = new ReviewHeader();
                reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.EvaluationSummaryResult.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.EvaluationSummaryResult.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.EvaluationSummaryResult.ReviewSessionName = reviewHeader.ReviewSessionName;
                    model.EvaluationSummaryResult.ReviewYearID = reviewHeader.ReviewYearId;
                    model.EvaluationSummaryResult.ReviewYearName = reviewHeader.ReviewYearName;
                    model.EvaluationSummaryResult.AppraiseeID = reviewHeader.AppraiseeId;
                    model.EvaluationSummaryResult.AppraiseeName = reviewHeader.AppraiseeName;
                    model.EvaluationSummaryResult.AppraiserID = reviewHeader.PrimaryAppraiserId;

                    ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.EvaluationSummaryResult.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        model.EvaluationSummaryResult.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                        model.EvaluationSummaryResult.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                        model.EvaluationSummaryResult.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                    }
                }

                var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID, null);
                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    model.EvaluationSummaryResult.AppraiserName = reviewResult.AppraiserName;
                    model.EvaluationSummaryResult.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                    model.EvaluationSummaryResult.AppraiserRoleName = reviewResult.AppraiserRoleName;
                    model.EvaluationSummaryResult.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    model.EvaluationSummaryResult.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                }

                ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID);
                if (scoreSummary != null)
                {
                    model.EvaluationSummaryResult.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                    model.EvaluationSummaryResult.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                    model.EvaluationSummaryResult.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                }

                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.EvaluationSummaryResult.ReviewSessionID, ReviewGradeType.Performance, model.EvaluationSummaryResult.TotalScoreObtained);
                if (appraisalGrade != null)
                {
                    model.EvaluationSummaryResult.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                    model.EvaluationSummaryResult.PerformanceRank = appraisalGrade.GradeRankDescription;
                }

                // Get KPA Results
                var kpa_entities = await _performanceService.GetInitialReviewResultKpasAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID);
                if (kpa_entities != null && kpa_entities.Count > 0)
                {
                    model.KpaFullResult.ReviewResultList = kpa_entities.ToList();
                }

                // Get Competency Results
                var cmp_entities = await _performanceService.GetInitialReviewResultCmpsAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID);
                if (cmp_entities != null && cmp_entities.Count > 0)
                {
                    model.CmpFullResult.ReviewResultList = cmp_entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        public async Task<IActionResult> ApproveEvaluation(int id, int sd = 0)
        {
            ApproveContractViewModel model = new ApproveContractViewModel();
            model.SubmissionID = sd;
            model.ApprovalTypeID = (int)ReviewApprovalType.ApproveEvaluationResult;

            ReviewHeader reviewHeader = new ReviewHeader();
            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                        model.AppraiseeName = reviewHeader.AppraiseeName;
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    }
                }

                ApplicationUser user = new ApplicationUser();
                string userId = string.Empty;
                userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    return LocalRedirect("/Home/Login");
                }
                model.ApproverID = userId;
                model.ApproverName = HttpContext.User.Identity.Name;
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            List<ApprovalRole> roles = new List<ApprovalRole>();
            var role_entities = await _performanceService.GetApprovalRolesAsync();
            if (role_entities != null) { roles = role_entities; }
            ViewBag.RoleList = new SelectList(roles, "ApprovalRoleId", "ApprovalRoleName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveEvaluation(ApproveContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewApproval reviewApproval = model.ConvertToReviewApproval();
                reviewApproval.IsApproved = true;
                reviewApproval.ApprovedTime = DateTime.UtcNow;
                reviewApproval.ApprovalTypeId = (int)ReviewApprovalType.ApproveEvaluationResult;
                reviewApproval.SubmissionPurposeId = (int)ReviewSubmissionPurpose.ResultApproval;
                try
                {
                    ApprovalRole approvalRole = new ApprovalRole();
                    approvalRole = await _performanceService.GetApprovalRoleAsync(reviewApproval.ApproverRoleId);
                    if (approvalRole != null) { model.ApproverRoleDescription = approvalRole.ApprovalRoleName; }

                    bool isApproved = await _performanceService.ApproveContractToAppraisee(reviewApproval, null);
                    if (isApproved)
                    {
                        model.ViewModelSuccessMessage = "Evaluation approved successfully!";
                        model.OperationIsSuccessful = true;

                        PmsActivityHistory history = new PmsActivityHistory();
                        history.ActivityDescription = $"Evaluation result was approved by {model.ApproverName} as {model.ApproverRoleDescription} on {DateTime.UtcNow.ToLongDateString()} at {DateTime.UtcNow.ToLongTimeString()} WAT";
                        history.ActivityTime = DateTime.UtcNow;
                        history.ReviewHeaderId = model.ReviewHeaderID;
                        history.ReviewSessionId = model.ReviewSessionID;
                        await _performanceService.AddPmsActivityHistoryAsync(history);
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        #region Appraisal Notes Controller Actions
        public async Task<IActionResult> AppraisalNotes(int id, string sp, string psp = null, int? sbm = null)
        {
            AppraisalNotesViewModel model = new AppraisalNotesViewModel();
            model.ReviewSubmissionID = sbm ?? 0;
            model.SourcePage = sp;
            model.src = sp;
            model.psp = psp;
            if (id > 0)
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;

                    ApplicationUser user = new ApplicationUser();
                    string userId = string.Empty;
                    userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                        return LocalRedirect("/Home/Login");
                    }
                    model.LoggedInEmployeeID = userId;
                }

                var entities = await _performanceService.GetReviewMessagesAsync(id);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewMessageList = entities.ToList();
                }
            }
            return View(model);
        }

        public IActionResult AddAppraisalNote(int id, string sd, string sp, string psp = null, int? sbm = null)
        {
            AddAppraisalNoteViewModel model = new AddAppraisalNoteViewModel();
            model.ReviewSubmissionID = sbm ?? 0;
            model.SourcePage = sp;
            model.src = sp;
            model.psp = psp;
            ReviewMessage reviewMessage = new ReviewMessage();
            try
            {
                if (id > 0) { model.ReviewHeaderID = id; }
                if (!string.IsNullOrWhiteSpace(sd)) { model.FromEmployeeID = sd; }
                if (!string.IsNullOrWhiteSpace(sp)) { model.SourcePage = sp; }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAppraisalNote(AddAppraisalNoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewMessage reviewMessage = new ReviewMessage();
                reviewMessage = model.ConvertToReviewMessage();
                reviewMessage.MessageTime = DateTime.UtcNow;
                try
                {
                    bool isAdded = await _performanceService.AddReviewMessageAsync(reviewMessage);
                    if (isAdded)
                    {
                        model.OperationIsSuccessful = true;
                        model.OperationIsCompleted = true;
                        model.ViewModelSuccessMessage = "New Note added successfully!";
                        return RedirectToAction("AppraisalNotes", new { id = model.ReviewHeaderID, sp = model.src, model.psp, sbm = model.ReviewSubmissionID });
                    }
                    else
                    {
                        model.ViewModelErrorMessage = "An error was encountered. The attempted operation failed.";
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

        #region Self Evaluation Controller Actions 

        //==== Start of Self Evaluation Controller Action Methods ======//
        public async Task<IActionResult> KpaSelfEvaluationList(int id, string ad)
        {
            EvaluationListViewModel model = new EvaluationListViewModel();
            if (id > 0 && !string.IsNullOrWhiteSpace(ad))
            {
                model.AppraiserID = ad;
                model.ReviewHeaderID = id;
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.PrimaryAppraiserID = reviewHeader.PrimaryAppraiserId;
                }

                var entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewResultList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            if (TempData["kpaCountErrorMessage"] != null)
            {
                if (!string.IsNullOrWhiteSpace(model.ViewModelErrorMessage))
                {
                    string errMsg = TempData["kpaCountErrorMessage"].ToString();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(model.ViewModelErrorMessage);
                    sb.Append(errMsg);
                    model.ViewModelErrorMessage = sb.ToString();
                }
                else
                {
                    model.ViewModelErrorMessage = TempData["kpaCountErrorMessage"].ToString();
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CmpSelfEvaluationList(int id, string ad)
        {
            EvaluationListViewModel model = new EvaluationListViewModel();
            if (id > 0 && !string.IsNullOrWhiteSpace(ad))
            {
                model.ReviewHeaderID = id;
                model.AppraiserID = ad;

                //Check to confirm that all KPAs have been evaluated
                int totalNumberOfKpas = 0;
                int totalNumberOfEvaluatedKpas = 0;
                totalNumberOfKpas = await _performanceService.GetMetricCountAsync(id, ReviewMetricType.KPA);
                totalNumberOfEvaluatedKpas = await _performanceService.GetEvaluatedMetricCountAsync(id, ad, ReviewMetricType.KPA);
                if (totalNumberOfEvaluatedKpas < totalNumberOfKpas)
                {
                    TempData["kpaCountErrorMessage"] = "Sorry, it appears not all KPAs are evaluated yet. Please confirm that all KPAs have been evaluated and try again.";
                    return RedirectToAction("KpaSelfEvaluationList", new { id, ad });
                }


                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.PrimaryAppraiserID = reviewHeader.PrimaryAppraiserId;
                }

                var entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewResultList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            var grade_entities = await _performanceService.GetAppraisalCompetencyGradesAsync(model.ReviewSessionID);
            if (grade_entities != null)
            {
                ViewBag.GradeList = new SelectList(grade_entities, "UpperBandScore", "AppraisalGradeDescription");
            }

            if (TempData["cmpCountErrorMessage"] != null)
            {
                if (!string.IsNullOrWhiteSpace(model.ViewModelErrorMessage))
                {
                    string errMsg = TempData["cmpCountErrorMessage"].ToString();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(model.ViewModelErrorMessage);
                    sb.Append(errMsg);
                    model.ViewModelErrorMessage = sb.ToString();
                }
                else
                {
                    model.ViewModelErrorMessage = TempData["cmpCountErrorMessage"].ToString();
                }
            }
            return View(model);
        }

        public async Task<IActionResult> EvaluationResult(int id, string ad)
        {
            EvaluationResultViewModel model = new EvaluationResultViewModel();
            model.AppraiserID = ad;
            model.ReviewHeaderID = id;
            if (id > 0 && !string.IsNullOrWhiteSpace(ad))
            {
                model.ReviewHeaderID = id;
                model.AppraiserID = ad;

                //Check to confirm that all KPAs have been evaluated
                int totalNumberOfCmps = 0;
                int totalNumberOfEvaluatedCmps = 0;
                totalNumberOfCmps = await _performanceService.GetMetricCountAsync(id, ReviewMetricType.Competency);
                totalNumberOfEvaluatedCmps = await _performanceService.GetEvaluatedMetricCountAsync(id, ad, ReviewMetricType.Competency);
                if (totalNumberOfEvaluatedCmps < totalNumberOfCmps)
                {
                    TempData["cmpCountErrorMessage"] = "Sorry, it appears not all Competencies are evaluated yet. Please confirm that all Competencies have been evaluated and try again.";
                    return RedirectToAction("CmpSelfEvaluationList", new { id, ad });
                }

                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewSessionName = reviewHeader.ReviewSessionName;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.ReviewYearName = reviewHeader.ReviewYearName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;

                    ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        model.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                        model.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                        model.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                    }
                }

                var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.ReviewHeaderID, model.AppraiserID, null);
                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    model.AppraiserName = reviewResult.AppraiserName;
                    model.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                    model.AppraiserRoleName = reviewResult.AppraiserRoleName;
                    model.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    model.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                }

                ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                if (scoreSummary != null)
                {
                    model.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                    model.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                    model.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                }

                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.ReviewSessionID, ReviewGradeType.Performance, model.TotalScoreObtained);
                if (appraisalGrade != null)
                {
                    model.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                    model.PerformanceRank = appraisalGrade.GradeRankDescription;
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be found.";
            }
            return View(model);
        }

        //==== End of Self Evaluation Controller Action Methods =======//

        #endregion

        #region Final Evaluation Controller Actions 

        //==== Start of Final Evaluation Controller Action Methods ======//
        public async Task<IActionResult> KpaFinalEvaluationList(int id, string ad, int sd)
        {
            EvaluationListViewModel model = new EvaluationListViewModel();
            if (id > 0 && !string.IsNullOrWhiteSpace(ad))
            {
                model.AppraiserID = ad;
                model.ReviewHeaderID = id;
                model.SubmissionID = sd;

                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.PrimaryAppraiserID = reviewHeader.PrimaryAppraiserId;
                }

                var entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewResultList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }

            if (TempData["kpaCountErrorMessage"] != null)
            {
                if (!string.IsNullOrWhiteSpace(model.ViewModelErrorMessage))
                {
                    string errMsg = TempData["kpaCountErrorMessage"].ToString();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(model.ViewModelErrorMessage);
                    sb.Append(errMsg);
                    model.ViewModelErrorMessage = sb.ToString();
                }
                else
                {
                    model.ViewModelErrorMessage = TempData["kpaCountErrorMessage"].ToString();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> CmpFinalEvaluationList(int id, string ad, int sd)
        {
            EvaluationListViewModel model = new EvaluationListViewModel();
            if (id > 0 && !string.IsNullOrWhiteSpace(ad))
            {
                model.ReviewHeaderID = id;
                model.AppraiserID = ad;
                model.SubmissionID = sd;

                //Check to confirm that all KPAs have been evaluated
                int totalNumberOfKpas = 0;
                int totalNumberOfEvaluatedKpas = 0;
                totalNumberOfKpas = await _performanceService.GetMetricCountAsync(id, ReviewMetricType.KPA);
                totalNumberOfEvaluatedKpas = await _performanceService.GetEvaluatedMetricCountAsync(id, ad, ReviewMetricType.KPA);
                if (totalNumberOfEvaluatedKpas < totalNumberOfKpas)
                {
                    TempData["kpaCountErrorMessage"] = "Sorry, it appears not all KPAs are evaluated yet. Please confirm that all KPAs have been evaluated and try again.";
                    return RedirectToAction("KpaFinalEvaluationList", new { id, ad, sd });
                }

                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.PrimaryAppraiserID = reviewHeader.PrimaryAppraiserId;
                }

                var entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
                if (entities != null && entities.Count > 0)
                {
                    model.ReviewResultList = entities.ToList();
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be retrieved.";
            }

            var grade_entities = await _performanceService.GetAppraisalCompetencyGradesAsync(model.ReviewSessionID);
            if (grade_entities != null)
            {
                ViewBag.GradeList = new SelectList(grade_entities, "UpperBandScore", "AppraisalGradeDescription");
            }

            if (TempData["cmpCountErrorMessage"] != null)
            {
                if (!string.IsNullOrWhiteSpace(model.ViewModelErrorMessage))
                {
                    string errMsg = TempData["cmpCountErrorMessage"].ToString();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(model.ViewModelErrorMessage);
                    sb.Append(errMsg);
                    model.ViewModelErrorMessage = sb.ToString();
                }
                else
                {
                    model.ViewModelErrorMessage = TempData["cmpCountErrorMessage"].ToString();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> FinalEvaluationResult(int id, string ad, int sd)
        {
            EvaluationResultViewModel model = new EvaluationResultViewModel();
            model.AppraiserID = ad;
            model.ReviewHeaderID = id;
            model.SubmissionID = sd;

            if (id > 0 && !string.IsNullOrWhiteSpace(ad))
            {
                //Check to confirm that all KPAs have been evaluated
                int totalNumberOfCmps = 0;
                int totalNumberOfEvaluatedCmps = 0;
                totalNumberOfCmps = await _performanceService.GetMetricCountAsync(id, ReviewMetricType.Competency);
                totalNumberOfEvaluatedCmps = await _performanceService.GetEvaluatedMetricCountAsync(id, ad, ReviewMetricType.Competency);
                if (totalNumberOfEvaluatedCmps < totalNumberOfCmps)
                {
                    TempData["cmpCountErrorMessage"] = "Sorry, it appears not all Competencies are evaluated yet. Please confirm that all Competencies have been evaluated and try again.";
                    return RedirectToAction("CmpFinalEvaluationList", new { id, ad, sd });
                }

                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewSessionName = reviewHeader.ReviewSessionName;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.ReviewYearName = reviewHeader.ReviewYearName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;

                    ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        model.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                        model.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                        model.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                    }
                }

                var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.ReviewHeaderID, model.AppraiserID, null);
                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    model.AppraiserName = reviewResult.AppraiserName;
                    model.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                    model.AppraiserRoleName = reviewResult.AppraiserRoleName;
                    model.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    model.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                }

                ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                if (scoreSummary != null)
                {
                    model.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                    model.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                    model.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                }

                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.ReviewSessionID, ReviewGradeType.Performance, model.TotalScoreObtained);
                if (appraisalGrade != null)
                {
                    model.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                    model.PerformanceRank = appraisalGrade.GradeRankDescription;
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }

        //==== End of Final Evaluation Controller Action Methods =======//

        #endregion

        #region Evaluation Results
        public async Task<IActionResult> ShowEvaluations(int id)
        {
            ShowEvaluationsViewModel model = new ShowEvaluationsViewModel();

            try
            {
                if (id > 0)
                {
                    model.ReviewHeaderID = id;
                    var entities = await _performanceService.GetEvaluationHeadersAsync(model.ReviewHeaderID);
                    if (entities != null && entities.Count > 0)
                    {
                        model.Evaluations = entities;
                        model.AppraiseeID = entities.First().AppraiseeId;
                        model.AppraiseeName = entities.First().AppraiseeName;
                        model.ReviewSessionID = entities.First().ReviewSessionId;
                        model.ReviewSessionName = entities.First().ReviewSessionName;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }
        public async Task<IActionResult> ShowResultSummary(int id, string ad)
        {
            EvaluationResultViewModel model = new EvaluationResultViewModel();
            model.AppraiserID = ad;
            model.ReviewHeaderID = id;
            if (id > 0 && !string.IsNullOrWhiteSpace(ad))
            {
                ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                if (reviewHeader != null)
                {
                    model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                    model.ReviewSessionID = reviewHeader.ReviewSessionId;
                    model.ReviewSessionName = reviewHeader.ReviewSessionName;
                    model.ReviewYearID = reviewHeader.ReviewYearId;
                    model.ReviewYearName = reviewHeader.ReviewYearName;
                    model.AppraiseeID = reviewHeader.AppraiseeId;
                    model.AppraiseeName = reviewHeader.AppraiseeName;

                    ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.ReviewSessionID);
                    if (reviewSession != null)
                    {
                        model.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                        model.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                        model.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                    }
                }

                var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.ReviewHeaderID, model.AppraiserID, null);
                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    model.AppraiserName = reviewResult.AppraiserName;
                    model.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                    model.AppraiserRoleName = reviewResult.AppraiserRoleName;
                    model.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    model.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                }

                ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                if (scoreSummary != null)
                {
                    model.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                    model.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                    model.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                }

                AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.ReviewSessionID, ReviewGradeType.Performance, model.TotalScoreObtained);
                if (appraisalGrade != null)
                {
                    model.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                    model.PerformanceRank = appraisalGrade.GradeRankDescription;
                }
            }
            else
            {
                model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            }
            return View(model);
        }
        public async Task<IActionResult> ShowFullResult(int id, string ad)
        {
            ShowSelectedResultViewModel model = new ShowSelectedResultViewModel();
            model.EvaluationSummaryResult = new EvaluationResultViewModel();
            model.KpaFullResult = new EvaluationListViewModel();
            model.CmpFullResult = new EvaluationListViewModel();
            model.ReviewHeaderInfo = new ReviewHeader();
            model.ReviewCDGs = new List<ReviewCDG>();
            model.id = id;
            model.ad = ad;
            if (!string.IsNullOrWhiteSpace(ad))
            {
                model.EvaluationSummaryResult.AppraiserID = ad;
                model.EvaluationSummaryResult.ReviewHeaderID = id;
                if (id > 0 && !string.IsNullOrWhiteSpace(ad))
                {
                    ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewHeaderInfo = reviewHeader;
                        model.EvaluationSummaryResult.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                        model.EvaluationSummaryResult.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.EvaluationSummaryResult.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.EvaluationSummaryResult.ReviewYearID = reviewHeader.ReviewYearId;
                        model.EvaluationSummaryResult.ReviewYearName = reviewHeader.ReviewYearName;
                        model.EvaluationSummaryResult.AppraiseeID = reviewHeader.AppraiseeId;
                        model.EvaluationSummaryResult.AppraiseeName = reviewHeader.AppraiseeName;

                        ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.EvaluationSummaryResult.ReviewSessionID);
                        if (reviewSession != null)
                        {
                            model.EvaluationSummaryResult.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                            model.EvaluationSummaryResult.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                            model.EvaluationSummaryResult.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                        }

                        List<ReviewCDG> reviewCDGs = await _performanceService.GetReviewCdgsAsync(id);
                        if (reviewCDGs != null)
                        {
                            model.ReviewCDGs = reviewCDGs;
                        }
                    }

                    var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID, null);
                    if (result_entities != null && result_entities.Count > 0)
                    {
                        ReviewResult reviewResult = result_entities.FirstOrDefault();
                        model.EvaluationSummaryResult.AppraiserName = reviewResult.AppraiserName;
                        model.EvaluationSummaryResult.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                        model.EvaluationSummaryResult.AppraiserRoleName = reviewResult.AppraiserRoleName;
                        model.EvaluationSummaryResult.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                        model.EvaluationSummaryResult.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                    }

                    ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                    if (scoreSummary != null)
                    {
                        model.EvaluationSummaryResult.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                        model.EvaluationSummaryResult.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                        model.EvaluationSummaryResult.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                    }

                    AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.EvaluationSummaryResult.ReviewSessionID, ReviewGradeType.Performance, model.EvaluationSummaryResult.TotalScoreObtained);
                    if (appraisalGrade != null)
                    {
                        model.EvaluationSummaryResult.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                        model.EvaluationSummaryResult.PerformanceRank = appraisalGrade.GradeRankDescription;
                    }

                    // Get KPA Results
                    var kpa_entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
                    if (kpa_entities != null && kpa_entities.Count > 0)
                    {
                        model.KpaFullResult.ReviewResultList = kpa_entities.ToList();
                    }

                    // Get Competency Results
                    var cmp_entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
                    if (cmp_entities != null && cmp_entities.Count > 0)
                    {
                        model.CmpFullResult.ReviewResultList = cmp_entities.ToList();
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
                }
            }

            var appraisers = await _performanceService.GetAppraiserDetailsAsync(id);
            if (appraisers != null)
            {
                ViewBag.AppraisersList = new SelectList(appraisers, "AppraiserId", "AppraiserFullDescription", ad);
            }
            return View(model);




            //ShowFullResultViewModel model = new ShowFullResultViewModel();
            //model.EvaluationSummaryResult = new EvaluationResultViewModel();
            //model.KpaFullResult = new EvaluationListViewModel();
            //model.CmpFullResult = new EvaluationListViewModel();

            //model.EvaluationSummaryResult.AppraiserID = ad;
            //model.EvaluationSummaryResult.ReviewHeaderID = id;
            //if (id > 0 && !string.IsNullOrWhiteSpace(ad))
            //{
            //    ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
            //    if (reviewHeader != null)
            //    {
            //        model.EvaluationSummaryResult.ReviewHeaderID = reviewHeader.ReviewHeaderId;
            //        model.EvaluationSummaryResult.ReviewSessionID = reviewHeader.ReviewSessionId;
            //        model.EvaluationSummaryResult.ReviewSessionName = reviewHeader.ReviewSessionName;
            //        model.EvaluationSummaryResult.ReviewYearID = reviewHeader.ReviewYearId;
            //        model.EvaluationSummaryResult.ReviewYearName = reviewHeader.ReviewYearName;
            //        model.EvaluationSummaryResult.AppraiseeID = reviewHeader.AppraiseeId;
            //        model.EvaluationSummaryResult.AppraiseeName = reviewHeader.AppraiseeName;

            //        ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.EvaluationSummaryResult.ReviewSessionID);
            //        if (reviewSession != null)
            //        {
            //            model.EvaluationSummaryResult.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
            //            model.EvaluationSummaryResult.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
            //            model.EvaluationSummaryResult.TotalScoreObtainable = reviewSession.TotalCombinedScore;
            //        }
            //    }

            //    var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID, null);
            //    if (result_entities != null && result_entities.Count > 0)
            //    {
            //        ReviewResult reviewResult = result_entities.FirstOrDefault();
            //        model.EvaluationSummaryResult.AppraiserName = reviewResult.AppraiserName;
            //        model.EvaluationSummaryResult.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
            //        model.EvaluationSummaryResult.AppraiserRoleName = reviewResult.AppraiserRoleName;
            //        model.EvaluationSummaryResult.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
            //        model.EvaluationSummaryResult.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
            //    }

            //    ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
            //    if (scoreSummary != null)
            //    {
            //        model.EvaluationSummaryResult.QualitativeScoreObtained = scoreSummary.QualitativeScore;
            //        model.EvaluationSummaryResult.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
            //        model.EvaluationSummaryResult.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
            //    }

            //    AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.EvaluationSummaryResult.ReviewSessionID, ReviewGradeType.Performance, model.EvaluationSummaryResult.TotalScoreObtained);
            //    if (appraisalGrade != null)
            //    {
            //        model.EvaluationSummaryResult.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
            //        model.EvaluationSummaryResult.PerformanceRank = appraisalGrade.GradeRankDescription;
            //    }

            //    // Get KPA Results
            //    var kpa_entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
            //    if (kpa_entities != null && kpa_entities.Count > 0)
            //    {
            //        model.KpaFullResult.ReviewResultList = kpa_entities.ToList();
            //    }

            //    // Get Competency Results
            //    var cmp_entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
            //    if (cmp_entities != null && cmp_entities.Count > 0)
            //    {
            //        model.CmpFullResult.ReviewResultList = cmp_entities.ToList();
            //    }
            //}
            //else
            //{
            //    model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
            //}
            //return View(model);
        }
        public async Task<IActionResult> ShowSelectedResult(int id, string ad)
        {
            ShowSelectedResultViewModel model = new ShowSelectedResultViewModel();
            model.EvaluationSummaryResult = new EvaluationResultViewModel();
            model.KpaFullResult = new EvaluationListViewModel();
            model.CmpFullResult = new EvaluationListViewModel();
            model.ReviewHeaderInfo = new ReviewHeader();
            model.ReviewCDGs = new List<ReviewCDG>();
            model.id = id;
            model.ad = ad;
            if (!string.IsNullOrWhiteSpace(ad))
            {
                model.EvaluationSummaryResult.AppraiserID = ad;
                model.EvaluationSummaryResult.ReviewHeaderID = id;
                if (id > 0 && !string.IsNullOrWhiteSpace(ad))
                {
                    ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewHeaderInfo = reviewHeader;
                        model.EvaluationSummaryResult.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                        model.EvaluationSummaryResult.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.EvaluationSummaryResult.ReviewSessionName = reviewHeader.ReviewSessionName;
                        model.EvaluationSummaryResult.ReviewYearID = reviewHeader.ReviewYearId;
                        model.EvaluationSummaryResult.ReviewYearName = reviewHeader.ReviewYearName;
                        model.EvaluationSummaryResult.AppraiseeID = reviewHeader.AppraiseeId;
                        model.EvaluationSummaryResult.AppraiseeName = reviewHeader.AppraiseeName;

                        ReviewSession reviewSession = await _performanceService.GetReviewSessionAsync(model.EvaluationSummaryResult.ReviewSessionID);
                        if (reviewSession != null)
                        {
                            model.EvaluationSummaryResult.QualitativeScoreObtainable = reviewSession.TotalCompetencyScore;
                            model.EvaluationSummaryResult.QuantitativeScoreObtainable = reviewSession.TotalKpaScore;
                            model.EvaluationSummaryResult.TotalScoreObtainable = reviewSession.TotalCombinedScore;
                        }

                        List<ReviewCDG> reviewCDGs = await _performanceService.GetReviewCdgsAsync(id);
                        if (reviewCDGs != null)
                        {
                            model.ReviewCDGs = reviewCDGs;
                        }
                    }

                    var result_entities = await _performanceService.GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(model.EvaluationSummaryResult.ReviewHeaderID, model.EvaluationSummaryResult.AppraiserID, null);
                    if (result_entities != null && result_entities.Count > 0)
                    {
                        ReviewResult reviewResult = result_entities.FirstOrDefault();
                        model.EvaluationSummaryResult.AppraiserName = reviewResult.AppraiserName;
                        model.EvaluationSummaryResult.AppraiserRoleID = reviewResult.AppraiserRoleId ?? 0;
                        model.EvaluationSummaryResult.AppraiserRoleName = reviewResult.AppraiserRoleName;
                        model.EvaluationSummaryResult.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                        model.EvaluationSummaryResult.AppraisalTime = $"{reviewResult.ScoreTime.Value.ToLongDateString()} {reviewResult.ScoreTime.Value.ToLongTimeString()} GMT";
                    }

                    ScoreSummary scoreSummary = await _performanceService.GetScoreSummaryAsync(id, ad);
                    if (scoreSummary != null)
                    {
                        model.EvaluationSummaryResult.QualitativeScoreObtained = scoreSummary.QualitativeScore;
                        model.EvaluationSummaryResult.QuantitativeScoreObtained = scoreSummary.QuantitativeScore;
                        model.EvaluationSummaryResult.TotalScoreObtained = scoreSummary.TotalPerformanceScore;
                    }

                    AppraisalGrade appraisalGrade = await _performanceService.GetAppraisalGradeAsync(model.EvaluationSummaryResult.ReviewSessionID, ReviewGradeType.Performance, model.EvaluationSummaryResult.TotalScoreObtained);
                    if (appraisalGrade != null)
                    {
                        model.EvaluationSummaryResult.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                        model.EvaluationSummaryResult.PerformanceRank = appraisalGrade.GradeRankDescription;
                    }

                    // Get KPA Results
                    var kpa_entities = await _performanceService.GetInitialReviewResultKpasAsync(id, ad);
                    if (kpa_entities != null && kpa_entities.Count > 0)
                    {
                        model.KpaFullResult.ReviewResultList = kpa_entities.ToList();
                    }

                    // Get Competency Results
                    var cmp_entities = await _performanceService.GetInitialReviewResultCmpsAsync(id, ad);
                    if (cmp_entities != null && cmp_entities.Count > 0)
                    {
                        model.CmpFullResult.ReviewResultList = cmp_entities.ToList();
                    }
                }
                else
                {
                    model.ViewModelErrorMessage = "Error: a key parameter is missing. No record could be r";
                }
            }

            var appraisers = await _performanceService.GetAppraiserDetailsAsync(id);
            if (appraisers != null)
            {
                ViewBag.AppraisersList = new SelectList(appraisers, "AppraiserId", "AppraiserFullDescription", ad);
            }
            return View(model);
        }
        #endregion

        #region Evaluation Feedback
        public async Task<IActionResult> ManageFeedback(int id, int sd)
        {
            ManageFeedbackViewModel model = new ManageFeedbackViewModel();
            ReviewHeader reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
            if (reviewHeader != null)
            {
                model.ProblemDescription = reviewHeader.FeedbackProblems;
                model.SolutionDescription = reviewHeader.FeedbackSolutions;
            }
            model.ReviewHeaderID = id;
            model.ReviewSessionID = sd;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageFeedback(ManageFeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int ReviewSessionId = model.ReviewSessionID;
                    int ReviewHeaderId = model.ReviewHeaderID;
                    string FeedbackProblems = model.ProblemDescription;
                    string FeedbackSolutions = model.SolutionDescription;

                    bool isAdded = await _performanceService.UpdateFeedbackAsync(ReviewHeaderId, FeedbackProblems, FeedbackSolutions);
                    if (isAdded)
                    {
                        PmsActivityHistory activityHistory = new PmsActivityHistory();
                        activityHistory.ReviewHeaderId = ReviewHeaderId;
                        activityHistory.ReviewSessionId = ReviewSessionId;
                        activityHistory.ActivityTime = DateTime.UtcNow;
                        activityHistory.ActivityDescription = $"Added Feedback to the Performance Evaluation record.";
                        await _performanceService.AddPmsActivityHistoryAsync(activityHistory);

                        model.ViewModelSuccessMessage = "Feedback added successfully!";
                        model.OperationIsSuccessful = true;
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

        #region Direct Reports
        public async Task<IActionResult> DirectReportEvaluations(int id, string sd)
        {
            DirectReportEvaluationsViewModel model = new DirectReportEvaluationsViewModel();
            model.id = id;
            model.sd = sd;
            model.ReportsResultSummaryList = new List<ResultSummary>();
            string ReportsToID = string.Empty;
            try
            {
                ApplicationUser user = new ApplicationUser();
                string userId = string.Empty;
                userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    return LocalRedirect("/Home/Login");
                }
                ReportsToID = userId;

                if (string.IsNullOrWhiteSpace(ReportsToID))
                {
                    model.ViewModelErrorMessage = "Sorry, it appears your session has expired. Please login and try again.";
                }
                else
                {
                    if (id > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(sd))
                        {
                            var entities = await _performanceService.GetResultSummaryForReportsAsync(ReportsToID, id, sd);
                            if (entities != null && entities.Count > 0)
                            {
                                model.ReportsResultSummaryList = entities;
                            }
                        }
                        else
                        {
                            var entities = await _performanceService.GetResultSummaryForReportsAsync(ReportsToID, id);
                            if (entities != null && entities.Count > 0)
                            {
                                model.ReportsResultSummaryList = entities;
                            }
                        }
                    }

                    var reports_entities = await _ermService.GetEmployeeReportsByReportsToEmployeeIdAsync(ReportsToID);
                    if (reports_entities != null && reports_entities.Count > 0)
                    {
                        ViewBag.ReportsList = new SelectList(reports_entities, "EmployeeID", "EmployeeName", sd);
                    }
                }

                var sessions_entities = await _performanceService.GetReviewSessionsAsync();
                if (sessions_entities != null && sessions_entities.Count > 0)
                {
                    ViewBag.SessionsList = new SelectList(sessions_entities, "Id", "Name", id);
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> AddRecommendation(int id)
        {
            AddRecommendationViewModel model = new AddRecommendationViewModel();
            try
            {
                model.ReviewHeaderID = id;
                if (id > 0)
                {
                    ReviewHeader reviewHeader = new ReviewHeader();
                    reviewHeader = await _performanceService.GetReviewHeaderAsync(id);
                    if (reviewHeader != null)
                    {
                        model.ReviewSessionID = reviewHeader.ReviewSessionId;
                        model.AppraiseeID = reviewHeader.AppraiseeId;
                    }
                }

                ApplicationUser user = new ApplicationUser();
                string userId = string.Empty;
                userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    await HttpContext.SignOutAsync(SecurityConstants.ChxCookieAuthentication);
                    return LocalRedirect("/Home/Login");
                }

                model.RecommenderID = userId;
                model.RecommenderName = HttpContext.User.Identity.Name;

                List<AppraisalRecommendation> entities = await _performanceService.GetAppraisalRecommendationsAsync();
                if (entities != null && entities.Count > 0)
                {
                    ViewBag.RecommendedActionList = new SelectList(entities, "Description", "Description");
                }
            }
            catch (Exception ex)
            {
                model.ViewModelErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecommendation(AddRecommendationViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReviewHeaderRecommendation recommendationModel = new ReviewHeaderRecommendation();
                recommendationModel.RecommendationRemarks = model.Remarks;
                recommendationModel.RecommendationType = model.RecommenderRole;
                recommendationModel.RecommendedAction = model.RecommendedAction;
                recommendationModel.ReviewHeaderId = model.ReviewHeaderID;
                recommendationModel.ReviewSessionId = model.ReviewSessionID;
                recommendationModel.RecommendedByName = model.RecommenderName;
                try
                {
                    bool isAdded = await _performanceService.AddAppraisalRecommendationAsync(recommendationModel);
                    if (isAdded)
                    {
                        PmsActivityHistory activityHistory = new PmsActivityHistory();
                        activityHistory.ReviewHeaderId = model.ReviewHeaderID;
                        activityHistory.ReviewSessionId = model.ReviewSessionID;
                        activityHistory.ActivityTime = DateTime.UtcNow;
                        activityHistory.ActivityDescription = $"{model.RecommenderRole} recommendation was added to the Performance Evaluation record by {model.RecommenderName}.";
                        await _performanceService.AddPmsActivityHistoryAsync(activityHistory);

                        model.ViewModelSuccessMessage = "Recommendation added successfully!";
                        model.OperationIsSuccessful = true;
                    }
                }
                catch (Exception ex)
                {
                    model.ViewModelErrorMessage = ex.Message;
                }
            }

            List<AppraisalRecommendation> entities = await _performanceService.GetAppraisalRecommendationsAsync();
            if (entities != null && entities.Count > 0)
            {
                ViewBag.RecommendedActionList = new SelectList(entities, "Description", "Description", model.RecommendedAction);
            }
            return View(model);
        }

        #endregion

        #region Helper Methods
        public string DeleteSubmission(int sd)
        {
            if (sd < 1) { return "parameter"; }
            //string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_performanceService.DeleteReviewSubmissionAsync(sd).Result)
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

        public string MarkSubmissionAsDone(int sd)
        {
            if (sd < 1) { return "parameter"; }
            //string actionBy = HttpContext.User.Identity.Name;
            try
            {
                if (_performanceService.UpdateReviewSubmissionAsync(sd).Result)
                {
                    return "marked";
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

        public string EvaluateKpa(int rh, int rm, string ap, string aa, int sc, string ac, string pa, int id = 0, int sd = 0)
        {
            ReviewResult reviewResult = new ReviewResult();
            reviewResult.ReviewResultId = id;
            reviewResult.AppraiserId = ap;
            reviewResult.ActualAchievement = aa;
            reviewResult.AppraiserScore = sc;
            reviewResult.AppraiserComment = ac;
            reviewResult.ReviewHeaderId = rh;
            reviewResult.ReviewMetricId = rm;
            reviewResult.ScoreTime = DateTime.UtcNow;

            try
            {
                if (sd > 0)
                {
                    ReviewSubmission reviewSubmission = _performanceService.GetReviewSubmissionByIdAsync(sd).Result;
                    if (reviewSubmission != null)
                    {
                        reviewResult.AppraiserRoleId = reviewSubmission.ToEmployeeRoleId;
                        reviewResult.AppraiserRoleName = reviewSubmission.ToEmployeeRoleName;
                    }
                }

                ReviewMetric reviewMetric = _performanceService.GetReviewMetricAsync(rm).Result;
                if (reviewMetric != null)
                {
                    reviewResult.ReviewMetricTypeId = reviewMetric.ReviewMetricTypeId;
                    reviewResult.ReviewSessionId = reviewMetric.ReviewSessionId;
                    reviewResult.ReviewYearId = reviewMetric.ReviewYearId;
                    reviewResult.AppraiseeId = reviewMetric.AppraiseeId;
                    reviewResult.AppraiserScoreDescription = $"{(Convert.ToInt32(sc)).ToString("D")}%";
                    reviewResult.AppraiserScoreValue = (sc * reviewMetric.ReviewMetricWeightage) / 100;
                }


                if (!string.IsNullOrWhiteSpace(pa) && !string.IsNullOrWhiteSpace(reviewResult.AppraiseeId))
                {
                    if (reviewResult.AppraiserId == reviewResult.AppraiseeId)
                    {
                        reviewResult.AppraiserTypeId = (int)AppraiserType.SelfAppraiser;
                    }
                    else if (reviewResult.AppraiserId == pa)
                    {
                        reviewResult.AppraiserTypeId = (int)AppraiserType.PrincipalAppraiser;
                    }
                    else
                    {
                        reviewResult.AppraiserTypeId = (int)AppraiserType.ThirdPartyAppraiser;
                    }
                }

                if (reviewResult.ReviewResultId > 0)
                {
                    bool IsSuccessful = _performanceService.UpdateReviewResultAsync(reviewResult).Result;
                    if (IsSuccessful) { return "saved"; }
                    else { return "failed"; }
                }
                else
                {
                    var result_entities = _performanceService.GetReviewResultByAppraiserIdAndReviewMetricIdAsync(rh, ap, rm).Result;
                    if(result_entities != null && result_entities.Count > 0)
                    {
                        reviewResult.ReviewResultId = result_entities.FirstOrDefault().ReviewResultId;
                        bool IsSuccessful = _performanceService.UpdateReviewResultAsync(reviewResult).Result;
                        if (IsSuccessful) { return "saved"; }
                        else { return "failed"; }
                    }
                    else
                    {
                        bool IsAdded = _performanceService.AddReviewResultAsync(reviewResult).Result;
                        if (IsAdded) { return "saved"; }
                        else { return "failed"; }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string EvaluateCmp(int rh, int rm, string ap, decimal sc, string ac, string pa, int id = 0, int sd = 0)
        {
            AppraisalGrade appraisalGrade = new AppraisalGrade();
            ReviewResult reviewResult = new ReviewResult();
            reviewResult.ReviewResultId = id;
            reviewResult.AppraiserId = ap;
            reviewResult.ActualAchievement = null;
            reviewResult.AppraiserScore = sc;
            reviewResult.AppraiserComment = ac;
            reviewResult.ReviewHeaderId = rh;
            reviewResult.ReviewMetricId = rm;
            reviewResult.ScoreTime = DateTime.UtcNow;

            try
            {
                if (sd > 0)
                {
                    ReviewSubmission reviewSubmission = _performanceService.GetReviewSubmissionByIdAsync(sd).Result;
                    if (reviewSubmission != null)
                    {
                        reviewResult.AppraiserRoleId = reviewSubmission.ToEmployeeRoleId;
                        reviewResult.AppraiserRoleName = reviewSubmission.ToEmployeeRoleName;
                    }
                }

                ReviewMetric reviewMetric = _performanceService.GetReviewMetricAsync(rm).Result;
                if (reviewMetric != null)
                {
                    reviewResult.ReviewMetricTypeId = reviewMetric.ReviewMetricTypeId;
                    reviewResult.ReviewSessionId = reviewMetric.ReviewSessionId;
                    reviewResult.ReviewYearId = reviewMetric.ReviewYearId;
                    reviewResult.AppraiseeId = reviewMetric.AppraiseeId;
                }


                if (!string.IsNullOrWhiteSpace(pa) && !string.IsNullOrWhiteSpace(reviewResult.AppraiseeId))
                {
                    if (reviewResult.AppraiserId == reviewResult.AppraiseeId)
                    {
                        reviewResult.AppraiserTypeId = (int)AppraiserType.SelfAppraiser;
                    }
                    else if (reviewResult.AppraiserId == pa)
                    {
                        reviewResult.AppraiserTypeId = (int)AppraiserType.PrincipalAppraiser;
                    }
                    else
                    {
                        reviewResult.AppraiserTypeId = (int)AppraiserType.ThirdPartyAppraiser;
                    }
                }

                if (reviewResult.AppraiserScore > 0)
                {
                    appraisalGrade = _performanceService.GetAppraisalGradeAsync(reviewMetric.ReviewSessionId, ReviewGradeType.Competency, reviewResult.AppraiserScore).Result;
                    if (appraisalGrade != null)
                    {
                        reviewResult.AppraiserScoreDescription = $"{appraisalGrade.AppraisalGradeDescription} ({appraisalGrade.UpperBandScore})";
                        reviewResult.AppraiserScoreValue = appraisalGrade.UpperBandScore;
                    }
                }

                if (reviewResult.ReviewResultId > 0)
                {
                    bool IsSuccessful = _performanceService.UpdateReviewResultAsync(reviewResult).Result;
                    if (IsSuccessful) { return "saved"; }
                    else { return "failed"; }
                }
                else
                {
                    var result_entities = _performanceService.GetReviewResultByAppraiserIdAndReviewMetricIdAsync(rh, ap, rm).Result;
                    if (result_entities != null && result_entities.Count > 0)
                    {
                        reviewResult.ReviewResultId = result_entities.FirstOrDefault().ReviewResultId;
                        bool IsSuccessful = _performanceService.UpdateReviewResultAsync(reviewResult).Result;
                        if (IsSuccessful) { return "saved"; }
                        else { return "failed"; }
                    }
                    else
                    {
                        bool IsAdded = _performanceService.AddReviewResultAsync(reviewResult).Result;
                        if (IsAdded) { return "saved"; }
                        else { return "failed"; }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion
    }
}