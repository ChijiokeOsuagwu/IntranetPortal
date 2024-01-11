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

namespace IntranetPortal.Areas.PMS.Controllers
{
    [Area("PMS")]
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

        [Authorize(Roles = "PMSAPRPRC, XYALLACCZ")]
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

        [Authorize(Roles = "PMSAPRPRC, XYALLACCZ")]
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

            //ApplicationUser user = new ApplicationUser();
            EmployeeUser employeeUser = new EmployeeUser();
            string userId = string.Empty;
            string user_name = HttpContext.User.Identity.Name;
            userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            //if (!string.IsNullOrWhiteSpace(userStringId)) { userId = Convert.ToInt32(userStringId); }
            var employee_users = await _securityService.GetEmployeeUsersByNameAsync(user_name);
            //employeeUser = await _ermService.GetEmployeeByIdAsync(userId);

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
                else
                {
                    ReviewSchedule reviewSchedule = new ReviewSchedule();
                    reviewSchedule = await _performanceService.GetEmployeePerformanceScheduleAsync(id, model.AppraiseeId);
                    if ((reviewSchedule == null) || (reviewSchedule.AllActivitiesScheduled == false && reviewSchedule.ContractDefinitionScheduled == false && reviewSchedule.PerformanceEvaluationScheduled == false))
                    {
                        model.ViewModelWarningMessage = $"Note: You are not scheduled for any activity on this Appraisal Session. All action buttons have been disabled. ";
                    }

                    model.AllActivitiesScheduled = reviewSchedule.AllActivitiesScheduled;
                    model.ContractDefinitionScheduled = reviewSchedule.ContractDefinitionScheduled;
                    model.PerformanceEvaluationScheduled = reviewSchedule.PerformanceEvaluationScheduled;
                }
            }


            if (id > 0 && !string.IsNullOrWhiteSpace(model.AppraiseeId))
            {
                ReviewHeader reviewHeader = new ReviewHeader();
                reviewHeader = await _performanceService.GetReviewHeaderAsync(model.AppraiseeId, id);
                if (reviewHeader != null)
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

        [Authorize(Roles = "PMSAPRPRC, PMSAPRAPV, XYALLACCZ")]
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
    }
}