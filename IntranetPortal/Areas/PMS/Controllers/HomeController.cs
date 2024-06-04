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
            //model.ReviewHeaderList = new List<ReviewHeader>();
            //model.id = id;
            //model.td = td ?? 0;
            //model.ld = ld ?? 0;
            //model.nm = nm;
            //model.ud = ud ?? 0;
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
new DataColumn("Line Manager"),
new DataColumn("Unit Head"),
new DataColumn("Department Head"),
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
                    result.LineManagerName,
                    result.UnitHeadName,
                    result.DepartmentHeadName
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