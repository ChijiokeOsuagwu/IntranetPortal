using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Base.Repositories.ErmRepository;
using IntranetPortal.Base.Repositories.PmsRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace IntranetPortal.Base.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IPerformanceYearRepository _performanceYearRepository;
        private readonly IReviewSessionRepository _reviewSessionRepository;
        private readonly IPerformanceSettingsRepository _performanceSettingsRepository;
        private readonly IReviewGradeRepository _reviewGradeRepository;
        private readonly IGradeHeaderRepository _gradeHeaderRepository;
        private readonly IAppraisalGradeRepository _appraisalGradeRepository;
        private readonly ISessionScheduleRepository _sessionScheduleRepository;
        private readonly IReviewHeaderRepository _reviewHeaderRepository;
        private readonly IReviewStageRepository _reviewStageRepository;
        private readonly IPmsActivityHistoryRepository _pmsActivityHistoryRepository;
        private readonly IReviewMetricRepository _reviewMetricRepository;
        private readonly ICompetencyRepository _competencyRepository;
        private readonly IPmsSystemRepository _pmsSystemRepository;
        private readonly IReviewCDGRepository _reviewCDGRepository;
        private readonly IReviewSubmissionRepository _reviewSubmissionRepository;
        private readonly IReviewMessageRepository _reviewMessageRepository;
        private readonly IApprovalRoleRepository _approvalRoleRepository;
        private readonly IReviewApprovalRepository _reviewApprovalRepository;
        private readonly IReviewResultRepository _reviewResultRepository;

        public PerformanceService(IEmployeesRepository employeesRepository, IPerformanceYearRepository performanceYearRepository,
            IReviewSessionRepository reviewSessionRepository, IPerformanceSettingsRepository performanceSettingsRepository,
            IReviewGradeRepository reviewGradeRepository, IGradeHeaderRepository gradeHeaderRepository,
            IAppraisalGradeRepository appraisalGradeRepository, ISessionScheduleRepository sessionScheduleRepository,
            IReviewHeaderRepository reviewHeaderRepository, IReviewStageRepository reviewStageRepository,
            IPmsActivityHistoryRepository pmsActivityHistoryRepository, IReviewMetricRepository reviewMetricRepository,
            ICompetencyRepository competencyRepository, IPmsSystemRepository pmsSystemRepository,
            IReviewCDGRepository reviewCDGRepository, IReviewSubmissionRepository reviewSubmissionRepository,
            IReviewMessageRepository reviewMessageRepository, IApprovalRoleRepository approvalRoleRepository,
            IReviewApprovalRepository reviewApprovalRepository, IReviewResultRepository reviewResultRepository)
        {
            _employeesRepository = employeesRepository;
            _performanceYearRepository = performanceYearRepository;
            _reviewSessionRepository = reviewSessionRepository;
            _performanceSettingsRepository = performanceSettingsRepository;
            _reviewGradeRepository = reviewGradeRepository;
            _gradeHeaderRepository = gradeHeaderRepository;
            _appraisalGradeRepository = appraisalGradeRepository;
            _sessionScheduleRepository = sessionScheduleRepository;
            _reviewHeaderRepository = reviewHeaderRepository;
            _reviewStageRepository = reviewStageRepository;
            _pmsActivityHistoryRepository = pmsActivityHistoryRepository;
            _reviewMetricRepository = reviewMetricRepository;
            _competencyRepository = competencyRepository;
            _pmsSystemRepository = pmsSystemRepository;
            _reviewCDGRepository = reviewCDGRepository;
            _reviewSubmissionRepository = reviewSubmissionRepository;
            _reviewMessageRepository = reviewMessageRepository;
            _approvalRoleRepository = approvalRoleRepository;
            _reviewApprovalRepository = reviewApprovalRepository;
            _reviewResultRepository = reviewResultRepository;
        }

        #region Performance Year Service Methods

        public async Task<List<PerformanceYear>> GetPerformanceYearsAsync()
        {
            List<PerformanceYear> performanceYearList = new List<PerformanceYear>();
            var entities = await _performanceYearRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                performanceYearList = entities.ToList();
            }
            return performanceYearList;
        }

        public async Task<PerformanceYear> GetPerformanceYearAsync(int PerformanceYearId)
        {
            PerformanceYear performanceYear = new PerformanceYear();
            var entities = await _performanceYearRepository.GetByIdAsync(PerformanceYearId);
            if (entities != null && entities.Count > 0)
            {
                performanceYear = entities.ToList().FirstOrDefault();
            }
            return performanceYear;
        }

        public async Task<bool> AddPerformanceYearAsync(PerformanceYear performanceYear)
        {
            if (performanceYear == null) { throw new ArgumentNullException(nameof(performanceYear)); }
            var entities = await _performanceYearRepository.GetByNameAsync(performanceYear.Name);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Performance Yaer with the same name already exists in the system.");
            }

            var overlapping_entities = await _performanceYearRepository.GetByOverlappingDatesAsync(performanceYear.StartDate.Value, performanceYear.EndDate.Value);
            if (overlapping_entities != null && overlapping_entities.Count > 0)
            {
                throw new Exception("It appears the Start and End dates overlaps with another Year in the system.");
            }

            return await _performanceYearRepository.AddAsync(performanceYear);
        }

        public async Task<bool> EditPerformanceYearAsync(PerformanceYear performanceYear)
        {
            if (performanceYear == null) { throw new ArgumentNullException(nameof(performanceYear)); }
            var entities = await _performanceYearRepository.GetByNameAsync(performanceYear.Name);
            if (entities != null && entities.Count > 0)
            {
                List<PerformanceYear> performanceYears = entities.ToList();
                foreach (PerformanceYear yr in performanceYears)
                {
                    if (yr.Id != performanceYear.Id)
                    {
                        throw new Exception("A Performance Yaer with the same name already exists in the system.");
                    }
                }
            }

            var overlapping_entities = await _performanceYearRepository.GetByOverlappingDatesAsync(performanceYear.StartDate.Value, performanceYear.EndDate.Value);
            if (overlapping_entities != null && overlapping_entities.Count > 0)
            {
                List<PerformanceYear> performanceYears = entities.ToList();
                foreach (PerformanceYear yr in performanceYears)
                {
                    if (yr.Id != performanceYear.Id)
                    {
                        throw new Exception("It appears the Start and End dates overlaps with another Year in the system.");
                    }
                }
            }
            return await _performanceYearRepository.UpdateAsync(performanceYear);
        }

        public async Task<bool> DeletePerformanceYearAsync(int performanceYearId)
        {
            if (performanceYearId < 1) { throw new ArgumentNullException(nameof(performanceYearId)); }
            return await _performanceYearRepository.DeleteAsync(performanceYearId);
        }
        #endregion

        #region Approval Role Service Methods
        public async Task<List<ApprovalRole>> GetApprovalRolesAsync()
        {
            List<ApprovalRole> ApprovalRoleList = new List<ApprovalRole>();
            var entities = await _approvalRoleRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                ApprovalRoleList = entities.ToList();
            }
            return ApprovalRoleList;
        }

        public async Task<ApprovalRole> GetApprovalRoleAsync(int ApprovalRoleId)
        {
            ApprovalRole approvalRole = new ApprovalRole();
            var entities = await _approvalRoleRepository.GetByIdAsync(ApprovalRoleId);
            if (entities != null && entities.Count > 0)
            {
                approvalRole = entities.ToList().FirstOrDefault();
            }
            return approvalRole;
        }

        public async Task<bool> AddApprovalRoleAsync(ApprovalRole approvalRole)
        {
            if (approvalRole == null) { throw new ArgumentNullException(nameof(approvalRole)); }
            var entities = await _approvalRoleRepository.GetByNameAsync(approvalRole.ApprovalRoleName);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("An Approval Role with the same name already exists in the system.");
            }


            return await _approvalRoleRepository.AddAsync(approvalRole);
        }

        public async Task<bool> EditApprovalRoleAsync(ApprovalRole approvalRole)
        {
            if (approvalRole == null) { throw new ArgumentNullException(nameof(approvalRole)); }
            var entities = await _approvalRoleRepository.GetByNameAsync(approvalRole.ApprovalRoleName);
            if (entities != null && entities.Count > 0)
            {
                List<ApprovalRole> approvalRoles = entities.ToList();
                foreach (ApprovalRole apr in approvalRoles)
                {
                    if (apr.ApprovalRoleId != approvalRole.ApprovalRoleId)
                    {
                        throw new Exception("An Approval Role with the same name already exists in the system.");
                    }
                }
            }
            return await _approvalRoleRepository.UpdateAsync(approvalRole);
        }

        public async Task<bool> DeleteApprovalRoleAsync(int approvalRoleId)
        {
            if (approvalRoleId < 1) { throw new ArgumentNullException(nameof(approvalRoleId)); }
            return await _approvalRoleRepository.DeleteAsync(approvalRoleId);
        }
        #endregion

        #region Review Session Read Service Methods
        public async Task<List<ReviewSession>> GetReviewSessionsAsync()
        {
            List<ReviewSession> reviewSessionList = new List<ReviewSession>();
            var entities = await _reviewSessionRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                reviewSessionList = entities.ToList();
            }
            return reviewSessionList;
        }

        public async Task<List<ReviewSession>> GetReviewSessionsAsync(int PerformanceYearId)
        {
            List<ReviewSession> reviewSessionList = new List<ReviewSession>();
            var entities = await _reviewSessionRepository.GetByYearIdAsync(PerformanceYearId);
            if (entities != null && entities.Count > 0)
            {
                reviewSessionList = entities.ToList();
            }
            return reviewSessionList;
        }

        public async Task<ReviewSession> GetReviewSessionAsync(int ReviewSessionId)
        {
            ReviewSession reviewSession = new ReviewSession();
            var entities = await _reviewSessionRepository.GetByIdAsync(ReviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                reviewSession = entities.ToList().FirstOrDefault();
            }
            return reviewSession;
        }

        #endregion

        #region Review Session Write Service Methods
        public async Task<bool> AddReviewSessionAsync(ReviewSession reviewSession)
        {
            if (reviewSession == null) { throw new ArgumentNullException(nameof(reviewSession)); }
            var entities = await _reviewSessionRepository.GetByNameAsync(reviewSession.Name);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Review Session with the same name already exists in the system.");
            }

            return await _reviewSessionRepository.AddAsync(reviewSession);
        }

        public async Task<bool> EditReviewSessionAsync(ReviewSession reviewSession)
        {
            if (reviewSession == null) { throw new ArgumentNullException(nameof(reviewSession)); }
            var entities = await _reviewSessionRepository.GetByNameAsync(reviewSession.Name);
            if (entities != null && entities.Count > 0)
            {
                List<ReviewSession> reviewSessions = entities.ToList();
                foreach (ReviewSession r in reviewSessions)
                {
                    if (r.Id != reviewSession.Id)
                    {
                        throw new Exception("A Review Session with the same name already exists in the system.");
                    }
                }
            }
            return await _reviewSessionRepository.UpdateAsync(reviewSession);
        }

        public async Task<bool> DeleteReviewSessionAsync(int ReviewSessionId)
        {
            if (ReviewSessionId < 1) { throw new ArgumentNullException(nameof(ReviewSessionId)); }
            return await _reviewSessionRepository.DeleteAsync(ReviewSessionId);
        }
        #endregion

        #region Grade Header Service Methods
        public async Task<List<GradeHeader>> GetGradeHeadersAsync()
        {
            List<GradeHeader> gradeHeaderList = new List<GradeHeader>();
            var entities = await _gradeHeaderRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                gradeHeaderList = entities.ToList();
            }
            return gradeHeaderList;
        }

        public async Task<GradeHeader> GetGradeHeaderAsync(int GradeHeaderId)
        {
            GradeHeader gradeHeader = new GradeHeader();
            var entities = await _gradeHeaderRepository.GetByIdAsync(GradeHeaderId);
            if (entities != null && entities.Count > 0)
            {
                gradeHeader = entities.ToList().FirstOrDefault();
            }
            return gradeHeader;
        }

        public async Task<bool> AddGradeHeaderAsync(GradeHeader gradeHeader)
        {
            if (gradeHeader == null) { throw new ArgumentNullException(nameof(gradeHeader)); }
            var entities = await _gradeHeaderRepository.GetByNameAsync(gradeHeader.GradeHeaderName);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Grading Profile with the same name already exists in the system.");
            }

            return await _gradeHeaderRepository.AddAsync(gradeHeader);
        }

        public async Task<bool> EditGradeHeaderAsync(GradeHeader gradeHeader)
        {
            if (gradeHeader == null) { throw new ArgumentNullException(nameof(gradeHeader)); }
            var entities = await _gradeHeaderRepository.GetByNameAsync(gradeHeader.GradeHeaderName);
            if (entities != null && entities.Count > 0)
            {
                List<GradeHeader> gradeHeaders = entities.ToList();
                foreach (GradeHeader gh in gradeHeaders)
                {
                    if (gh.GradeHeaderId != gradeHeader.GradeHeaderId)
                    {
                        throw new Exception("A Grading Profile with the same name already exists in the system.");
                    }
                }
            }

            return await _gradeHeaderRepository.UpdateAsync(gradeHeader);
        }

        public async Task<bool> DeleteGradeHeaderAsync(int gradeHeaderId)
        {
            if (gradeHeaderId < 1) { throw new ArgumentNullException(nameof(gradeHeaderId)); }
            return await _gradeHeaderRepository.DeleteAsync(gradeHeaderId);
        }
        #endregion

        #region Review Grade Details Write Service Methods
        public async Task<bool> AddReviewGradeAsync(ReviewGrade reviewGrade)
        {
            if (reviewGrade == null) { throw new ArgumentNullException(nameof(reviewGrade)); }
            var entities = await _reviewGradeRepository.GetByNameAsync(reviewGrade.ReviewGradeDescription);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Grade with the same name already exists in the system.");
            }

            return await _reviewGradeRepository.AddAsync(reviewGrade);
        }

        public async Task<bool> EditReviewGradeAsync(ReviewGrade reviewGrade)
        {
            if (reviewGrade == null) { throw new ArgumentNullException(nameof(reviewGrade)); }
            var entities = await _reviewGradeRepository.GetByNameAsync(reviewGrade.ReviewGradeDescription);
            if (entities != null && entities.Count > 0)
            {
                List<ReviewGrade> reviewGrades = entities.ToList();
                foreach (ReviewGrade r in reviewGrades)
                {
                    if (r.ReviewGradeId != reviewGrade.ReviewGradeId)
                    {
                        throw new Exception("A Grade with the same name already exists in the system.");
                    }
                }
            }
            return await _reviewGradeRepository.UpdateAsync(reviewGrade);
        }

        public async Task<bool> DeleteReviewGradeAsync(int reviewGradeId)
        {
            if (reviewGradeId < 1) { throw new ArgumentNullException(nameof(reviewGradeId)); }
            return await _reviewGradeRepository.DeleteAsync(reviewGradeId);
        }
        #endregion

        #region Review Grade Details Read Service Methods
        public async Task<List<ReviewGrade>> GetReviewGradesAsync()
        {
            List<ReviewGrade> reviewGradeList = new List<ReviewGrade>();
            var entities = await _reviewGradeRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                reviewGradeList = entities.ToList();
            }
            return reviewGradeList;
        }

        public async Task<List<ReviewGrade>> GetReviewGradesAsync(int gradeHeaderId)
        {
            List<ReviewGrade> reviewGradeList = new List<ReviewGrade>();
            var entities = await _reviewGradeRepository.GetByGradeHeaderIdAsync(gradeHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewGradeList = entities.ToList();
            }
            return reviewGradeList;
        }

        public async Task<ReviewGrade> GetReviewGradeAsync(int ReviewGradeId)
        {
            ReviewGrade reviewGrade = new ReviewGrade();
            var entities = await _reviewGradeRepository.GetByIdAsync(ReviewGradeId);
            if (entities != null && entities.Count > 0)
            {
                reviewGrade = entities.ToList().FirstOrDefault();
            }
            return reviewGrade;
        }

        public async Task<List<ReviewGrade>> GetPerformanceGradesAsync(int gradeHeaderId)
        {
            List<ReviewGrade> reviewGradeList = new List<ReviewGrade>();
            var entities = await _reviewGradeRepository.GetByGradeHeaderIdAsync(gradeHeaderId, ReviewGradeType.Performance);
            if (entities != null && entities.Count > 0)
            {
                reviewGradeList = entities.ToList();
            }
            return reviewGradeList;
        }

        public async Task<List<ReviewGrade>> GetCompetencyGradesAsync(int gradeHeaderId)
        {
            List<ReviewGrade> reviewGradeList = new List<ReviewGrade>();
            var entities = await _reviewGradeRepository.GetByGradeHeaderIdAsync(gradeHeaderId, ReviewGradeType.Competency);
            if (entities != null && entities.Count > 0)
            {
                reviewGradeList = entities.ToList();
            }
            return reviewGradeList;
        }
        #endregion

        #region Appraisal Grade Write Service Methods
        public async Task<bool> AddAppraisalGradeAsync(AppraisalGrade appraisalGrade)
        {
            if (appraisalGrade == null) { throw new ArgumentNullException(nameof(appraisalGrade)); }
            var entities = await _appraisalGradeRepository.GetByNameAsync(appraisalGrade.AppraisalGradeDescription);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Grade with the same name already exists in the system.");
            }

            return await _appraisalGradeRepository.AddAsync(appraisalGrade);
        }

        public async Task<bool> CopyAppraisalGradeAsync(string copiedBy, int reviewSessionId, int gradeTemplateId, ReviewGradeType? gradeType = null)
        {
            if (string.IsNullOrWhiteSpace(copiedBy)) { throw new ArgumentNullException(nameof(copiedBy)); }
            if (reviewSessionId < 1) { throw new ArgumentNullException(nameof(reviewSessionId)); }
            if (gradeTemplateId < 1) { throw new ArgumentNullException(nameof(gradeTemplateId)); }

            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAsync(reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Sorry, this Appraisal Session already has Grades set up in the system.");
            }
            if (gradeType == null)
            {
                return await _appraisalGradeRepository.CopyAsync(copiedBy, reviewSessionId, gradeTemplateId);
            }
            else
            {
                return await _appraisalGradeRepository.CopyAsync(copiedBy, reviewSessionId, gradeTemplateId, gradeType.Value);
            }
        }

        public async Task<bool> EditAppraisalGradeAsync(AppraisalGrade appraisalGrade)
        {
            if (appraisalGrade == null) { throw new ArgumentNullException(nameof(appraisalGrade)); }
            var entities = await _appraisalGradeRepository.GetByNameAsync(appraisalGrade.AppraisalGradeDescription);
            if (entities != null && entities.Count > 0)
            {
                List<AppraisalGrade> appraisalGrades = entities.ToList();
                foreach (AppraisalGrade r in appraisalGrades)
                {
                    if (r.AppraisalGradeId != appraisalGrade.AppraisalGradeId)
                    {
                        throw new Exception("A Grade with the same name already exists in the system.");
                    }
                }
            }
            return await _appraisalGradeRepository.UpdateAsync(appraisalGrade);
        }

        public async Task<bool> DeleteAppraisalGradeAsync(int appraisalGradeId)
        {
            if (appraisalGradeId < 1) { throw new ArgumentNullException(nameof(appraisalGradeId)); }
            return await _appraisalGradeRepository.DeleteAsync(appraisalGradeId);
        }


        #endregion

        #region Appraisal Grade Read Service Methods
        public async Task<List<AppraisalGrade>> GetAppraisalGradesAsync()
        {
            List<AppraisalGrade> appraisalGradeList = new List<AppraisalGrade>();
            var entities = await _appraisalGradeRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                appraisalGradeList = entities.ToList();
            }
            return appraisalGradeList;
        }

        public async Task<List<AppraisalGrade>> GetAppraisalGradesAsync(int reviewSessionId)
        {
            List<AppraisalGrade> appraisalGradeList = new List<AppraisalGrade>();
            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAsync(reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                appraisalGradeList = entities.ToList();
            }
            return appraisalGradeList;
        }

        public async Task<AppraisalGrade> GetAppraisalGradeAsync(int AppraisalGradeId)
        {
            AppraisalGrade appraisalGrade = new AppraisalGrade();
            var entities = await _appraisalGradeRepository.GetByIdAsync(AppraisalGradeId);
            if (entities != null && entities.Count > 0)
            {
                appraisalGrade = entities.ToList().FirstOrDefault();
            }
            return appraisalGrade;
        }

        public async Task<List<AppraisalGrade>> GetAppraisalPerformanceGradesAsync(int reviewSessionId)
        {
            List<AppraisalGrade> appraisalGradeList = new List<AppraisalGrade>();
            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAsync(reviewSessionId, ReviewGradeType.Performance);
            if (entities != null && entities.Count > 0)
            {
                appraisalGradeList = entities.ToList();
            }
            return appraisalGradeList;
        }

        public async Task<List<AppraisalGrade>> GetAppraisalCompetencyGradesAsync(int reviewSessionId)
        {
            List<AppraisalGrade> appraisalGradeList = new List<AppraisalGrade>();
            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAsync(reviewSessionId, ReviewGradeType.Competency);
            if (entities != null && entities.Count > 0)
            {
                appraisalGradeList = entities.ToList();
            }
            return appraisalGradeList;
        }

        public async Task<AppraisalGrade> GetAppraisalGradeAsync(int reviewSessionId, ReviewGradeType gradeType, decimal gradeScore)
        {
            AppraisalGrade appraisalGrade = new AppraisalGrade();
            var entities = await _appraisalGradeRepository.GetByReviewSessionIdAndGradeScoreAsync(reviewSessionId, gradeType, gradeScore); ;
            if (entities != null && entities.Count > 0)
            {
                appraisalGrade = entities.ToList().FirstOrDefault();
            }
            return appraisalGrade;
        }

        #endregion

        #region Session Schedule Write Service Methods
        public async Task<bool> AddSessionScheduleAsync(SessionSchedule sessionSchedule)
        {
            if (sessionSchedule == null) { throw new ArgumentNullException(nameof(sessionSchedule)); }

            if (sessionSchedule.ReviewYearId < 1 && sessionSchedule.ReviewSessionId > 0)
            {
                ReviewSession reviewSession = new ReviewSession();
                var entities = await _reviewSessionRepository.GetByIdAsync(sessionSchedule.ReviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    reviewSession = entities.FirstOrDefault();
                    sessionSchedule.ReviewYearId = reviewSession.ReviewYearId;
                }
            }
            return await _sessionScheduleRepository.AddAsync(sessionSchedule);
        }

        public async Task<bool> CancelSessionScheduleAsync(int sessionScheduleId, string cancelledBy)
        {
            if (string.IsNullOrWhiteSpace(cancelledBy)) { throw new ArgumentNullException(nameof(cancelledBy)); }
            if (sessionScheduleId < 1) { throw new ArgumentNullException(nameof(sessionScheduleId)); }
            return await _sessionScheduleRepository.CancelAsync(sessionScheduleId, cancelledBy);
        }

        public async Task<bool> DeleteSessionScheduleAsync(int sessionScheduleId)
        {
            if (sessionScheduleId < 1) { throw new ArgumentNullException(nameof(sessionScheduleId)); }
            return await _sessionScheduleRepository.DeleteAsync(sessionScheduleId);
        }

        #endregion

        #region Session Schedule Read Service Methods
        public async Task<List<SessionSchedule>> GetSessionSchdulesAsync(int reviewSessionId)
        {
            List<SessionSchedule> sessionScheduleList = new List<SessionSchedule>();
            var entities = await _sessionScheduleRepository.GetByReviewSessionIdAsync(reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                sessionScheduleList = entities.ToList();
            }
            return sessionScheduleList;
        }
        public async Task<SessionSchedule> GetSessionScheduleAsync(int sessionScheduleId)
        {
            SessionSchedule sessionSchedule = new SessionSchedule();
            var entities = await _sessionScheduleRepository.GetByIdAsync(sessionScheduleId);
            if (entities != null && entities.Count > 0)
            {
                sessionSchedule = entities.ToList().FirstOrDefault();
            }
            return sessionSchedule;
        }

        public async Task<ReviewSchedule> GetEmployeePerformanceScheduleAsync(int reviewSessionId, string employeeId)
        {
            ReviewSchedule reviewSchedule = new ReviewSchedule();
            Employee employee = new Employee();
            var employee_entity = await _employeesRepository.GetEmployeeByIdAsync(employeeId);
            if (employee_entity != null) { employee = employee_entity; }
            if (!string.IsNullOrWhiteSpace(employee.FullName))
            {
                // Get schedules for all staff
                var schedule_list_for_all = await _sessionScheduleRepository.GetForAllAsync(reviewSessionId);
                if (schedule_list_for_all != null && schedule_list_for_all.Count > 0)
                {
                    foreach (var item in schedule_list_for_all)
                    {
                        switch (item)
                        {
                            case SessionActivityType.AllActivities:
                                reviewSchedule.AllActivitiesScheduled = true;
                                break;
                            case SessionActivityType.ContractDefinitionOnly:
                                reviewSchedule.ContractDefinitionScheduled = true;
                                break;
                            case SessionActivityType.PerformanceEvaluationOnly:
                                reviewSchedule.PerformanceEvaluationScheduled = true;
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Get schedules for location
                if (employee.LocationID != null && employee.LocationID > 0)
                {
                    var schedule_list_for_location = await _sessionScheduleRepository.GetForLocationAsync(reviewSessionId, employee.LocationID.Value);
                    if (schedule_list_for_location != null && schedule_list_for_location.Count > 0)
                    {
                        foreach (var item in schedule_list_for_location)
                        {
                            switch (item)
                            {
                                case SessionActivityType.AllActivities:
                                    reviewSchedule.AllActivitiesScheduled = true;
                                    break;
                                case SessionActivityType.ContractDefinitionOnly:
                                    reviewSchedule.ContractDefinitionScheduled = true;
                                    break;
                                case SessionActivityType.PerformanceEvaluationOnly:
                                    reviewSchedule.PerformanceEvaluationScheduled = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if ((reviewSchedule.AllActivitiesScheduled) || (reviewSchedule.ContractDefinitionScheduled && reviewSchedule.PerformanceEvaluationScheduled))
                        {
                            return reviewSchedule;
                        }
                    }

                }

                // Get schedules for department
                if (employee.DepartmentID != null && employee.DepartmentID > 0)
                {
                    var schedule_list_for_dept = await _sessionScheduleRepository.GetForDepartmentAsync(reviewSessionId, employee.DepartmentID.Value);
                    if (schedule_list_for_dept != null && schedule_list_for_dept.Count > 0)
                    {
                        foreach (var item in schedule_list_for_dept)
                        {
                            switch (item)
                            {
                                case SessionActivityType.AllActivities:
                                    reviewSchedule.AllActivitiesScheduled = true;
                                    break;
                                case SessionActivityType.ContractDefinitionOnly:
                                    reviewSchedule.ContractDefinitionScheduled = true;
                                    break;
                                case SessionActivityType.PerformanceEvaluationOnly:
                                    reviewSchedule.PerformanceEvaluationScheduled = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if ((reviewSchedule.AllActivitiesScheduled) || (reviewSchedule.ContractDefinitionScheduled && reviewSchedule.PerformanceEvaluationScheduled))
                        {
                            return reviewSchedule;
                        }
                    }
                }

                // Get schedules for unit
                if (employee.UnitID != null && employee.UnitID > 0)
                {
                    var schedule_list_for_unit = await _sessionScheduleRepository.GetForUnitAsync(reviewSessionId, employee.UnitID.Value);
                    if (schedule_list_for_unit != null && schedule_list_for_unit.Count > 0)
                    {
                        foreach (var item in schedule_list_for_unit)
                        {
                            switch (item)
                            {
                                case SessionActivityType.AllActivities:
                                    reviewSchedule.AllActivitiesScheduled = true;
                                    break;
                                case SessionActivityType.ContractDefinitionOnly:
                                    reviewSchedule.ContractDefinitionScheduled = true;
                                    break;
                                case SessionActivityType.PerformanceEvaluationOnly:
                                    reviewSchedule.PerformanceEvaluationScheduled = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if ((reviewSchedule.AllActivitiesScheduled) || (reviewSchedule.ContractDefinitionScheduled && reviewSchedule.PerformanceEvaluationScheduled))
                        {
                            return reviewSchedule;
                        }
                    }
                }

                // Get schedules for employee
                if (!string.IsNullOrWhiteSpace(employeeId))
                {
                    var schedule_list_for_emp = await _sessionScheduleRepository.GetForEmployeeAsync(reviewSessionId, employeeId);
                    if (schedule_list_for_emp != null && schedule_list_for_emp.Count > 0)
                    {
                        foreach (var item in schedule_list_for_emp)
                        {
                            switch (item)
                            {
                                case SessionActivityType.AllActivities:
                                    reviewSchedule.AllActivitiesScheduled = true;
                                    break;
                                case SessionActivityType.ContractDefinitionOnly:
                                    reviewSchedule.ContractDefinitionScheduled = true;
                                    break;
                                case SessionActivityType.PerformanceEvaluationOnly:
                                    reviewSchedule.PerformanceEvaluationScheduled = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        if ((reviewSchedule.AllActivitiesScheduled) || (reviewSchedule.ContractDefinitionScheduled && reviewSchedule.PerformanceEvaluationScheduled))
                        {
                            return reviewSchedule;
                        }
                    }
                }
            }

            return reviewSchedule;
        }
        #endregion

        #region Review Header Service Methods

        //================= Review Header Read Service Methods ======================//
        public async Task<ReviewHeader> GetReviewHeaderAsync(int reviewHeaderId)
        {
            ReviewHeader reviewHeader = new ReviewHeader();
            var entities = await _reviewHeaderRepository.GetByIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewHeader = entities.FirstOrDefault();
            }
            return reviewHeader;
        }

        public async Task<ReviewHeader> GetReviewHeaderAsync(string appraiseeId, int reviewSessionId)
        {
            ReviewHeader reviewHeader = new ReviewHeader();
            var entities = await _reviewHeaderRepository.GetByAppraiseeIdAndReviewSessionIdAsync(appraiseeId, reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                reviewHeader = entities.FirstOrDefault();
            }
            return reviewHeader;
        }

        public async Task<List<ReviewHeader>> GetReviewHeadersAsync(int reviewSessionId, int? reviewStageId = null, int? locationId = null, int? unitId = null, string appraiseeName = null)
        {
            int ReviewSessionID = reviewSessionId;
            int ReviewStageID = reviewStageId ?? 0;
            int LocationID = locationId ?? 0;
            int UnitID = unitId ?? 0;
            
            List<ReviewHeader> reviewHeaderList = new List<ReviewHeader>();

            if (string.IsNullOrWhiteSpace(appraiseeName))
            {
                if (ReviewSessionID > 0 && ReviewStageID > 0 && LocationID > 0 && UnitID > 0)
                {
                    var entities = await _reviewHeaderRepository.GetByReviewSessionIdnStageIdnLocationIdnUnitIdAsync(ReviewSessionID, ReviewStageID, LocationID, UnitID);
                    if (entities != null && entities.Count > 0)
                    {
                        reviewHeaderList = entities.ToList();
                    }
                }
                else if (ReviewSessionID > 0 && ReviewStageID > 0 && LocationID > 0)
                {
                    var entities = await _reviewHeaderRepository.GetByReviewSessionIdnStageIdnLocationIdAsync(ReviewSessionID, ReviewStageID, LocationID);
                    if (entities != null && entities.Count > 0)
                    {
                        reviewHeaderList = entities.ToList();
                    }
                }
                else if (ReviewSessionID > 0 && ReviewStageID > 0)
                {
                    var entities = await _reviewHeaderRepository.GetByReviewSessionIdnStageIdAsync(ReviewSessionID, ReviewStageID);
                    if (entities != null && entities.Count > 0)
                    {
                        reviewHeaderList = entities.ToList();
                    }
                }
                else if (ReviewSessionID > 0)
                {
                    var entities = await _reviewHeaderRepository.GetByReviewSessionIdAsync(ReviewSessionID);
                    if (entities != null && entities.Count > 0)
                    {
                        reviewHeaderList = entities.ToList();
                    }
                }
            }
            else
            {
                if(ReviewSessionID > 0)
                {
                    var entities = await _reviewHeaderRepository.GetByReviewSessionIdnAppraiseeNameAsync(ReviewSessionID, appraiseeName);
                    if (entities != null && entities.Count > 0)
                    {
                        reviewHeaderList = entities.ToList();
                    }
                }
                else
                {
                    var entities = await _reviewHeaderRepository.GetByAppraiseeNameAsync(appraiseeName);
                    if (entities != null && entities.Count > 0)
                    {
                        reviewHeaderList = entities.ToList();
                    }
                }

            }
            return reviewHeaderList;
        }

        //================== Review Header Write Service Methods =====================================================//
        public async Task<bool> AddReviewHeaderAsync(ReviewHeader reviewHeader)
        {
            if (reviewHeader == null) { throw new ArgumentNullException(nameof(reviewHeader)); }
            var entities = await _reviewHeaderRepository.GetByAppraiseeIdAndReviewSessionIdAsync(reviewHeader.AppraiseeId, reviewHeader.ReviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Error: duplicate entry. There is an already existing appraisal record in the system for this staff and for this Session.");
            }

            return await _reviewHeaderRepository.AddAsync(reviewHeader);
        }
        
        public async Task<bool> UpdatePerformanceGoalAsync(int reviewHeaderId, string performanceGoal, string appraiserId)
        {
            if (reviewHeaderId < 1) { throw new ArgumentNullException(nameof(reviewHeaderId)); }
            return await _reviewHeaderRepository.UpdateGoalAsync(reviewHeaderId, performanceGoal, appraiserId);
        }

        public async Task<bool> UpdateReviewHeaderStageAsync(int reviewHeaderId, int nextStageId)
        {
            if (reviewHeaderId < 1) { throw new ArgumentNullException(nameof(reviewHeaderId)); }
            if (nextStageId < 1) { throw new ArgumentNullException(nameof(nextStageId)); }

            bool IsUpdated = await _reviewHeaderRepository.UpdateStageIdAsync(reviewHeaderId, nextStageId);
            if (IsUpdated)
            {
                PmsActivityHistory activityHistory = new PmsActivityHistory();
                activityHistory.ReviewHeaderId = reviewHeaderId;
                activityHistory.ActivityTime = DateTime.UtcNow;
                switch (nextStageId)
                {
                    case 1:
                        activityHistory.ActivityDescription = $"Commenced the defining of Performance Goal(s).";
                        break;
                    case 2:
                        activityHistory.ActivityDescription = $"Completed the defining of Performance Goal(s). And commenced the Setting up of Key Performance Areas (KPAs).";
                        break;
                    case 3:
                        activityHistory.ActivityDescription = $"Completed the setting up of Key Performance Areas (KPAs). And commenced the Setting up of Competencies.";
                        break;
                    case 4:
                        activityHistory.ActivityDescription = $"Completed the setting up of Competencies. And commenced the defining of Career Development Goal(s).";
                        break;
                    case 5:
                        activityHistory.ActivityDescription = $"Completed the defining of Career Development Goal(s). And commenced the Performance Contract Review and Approval step.";
                        break;
                    case 6:
                        activityHistory.ActivityDescription = $"Completed the Performance Contract Review and Approvals step. And commenced the Performance Contract Agreement and Sign-Off step.";
                        break;
                    case 7:
                        activityHistory.ActivityDescription = $"Completed the Performance Contract Agreement and Sign-Off step and the Performance Contract Definition phase.";
                        break;

                    case 8:
                        activityHistory.ActivityDescription = $"Commenced the Performance Evaluation phase with the Self Evaluation step.";
                        break;
                    case 9:
                        activityHistory.ActivityDescription = $"Completed Self Evaluation and commenced submitting for Final Evaluation.";
                        break;
                    case 10:
                        activityHistory.ActivityDescription = $"Completed Final Evaluation and started Evaluation Result approvals.";
                        break;
                    case 11:
                        activityHistory.ActivityDescription = $"Completed all Evaluation Result approvals. To commence Appraisee Acceptance and Sign Off.";
                        break;
                    case 12:
                        activityHistory.ActivityDescription = $"Completed Appraisee Acceptance and Sign Off. Performance Evaluation successfully completed!";
                        break;
                    default:
                        break;
                }
                await _pmsActivityHistoryRepository.AddAsync(activityHistory);
            }
            return IsUpdated;
        }

        public async Task<bool> UpdateAppraiseeFlagAsync(int reviewHeaderId, bool isFlagged, string flaggedReason)
        {
            if (reviewHeaderId < 1) { throw new ArgumentNullException(nameof(reviewHeaderId)); }
            return await _reviewHeaderRepository.UpdateAppraiseeFlagAsync(reviewHeaderId, isFlagged, flaggedReason);
        }

        public async Task<bool> UpdateFeedbackAsync(int reviewHeaderId, string feedbackProblems, string feedbackSolutions)
        {
            if (reviewHeaderId < 1) { throw new ArgumentNullException(nameof(reviewHeaderId)); }
            return await _reviewHeaderRepository.UpdateFeedbackAsync(reviewHeaderId, feedbackProblems, feedbackSolutions);
        }

        public async Task<bool> AddAppraisalRecommendationAsync(ReviewHeaderRecommendation model)
        {
            string recommendedAction = model.RecommendedAction;
            string remarks = model.RecommendationRemarks;
            int reviewHeaderId = model.ReviewHeaderId;
            string recommendedBy = model.RecommendedByName;
            bool recommendationAdded = false;

            switch (model.RecommendationType)
            {
                case "L":
                    recommendationAdded = await _reviewHeaderRepository.UpdateLineManagerRecommendationAsync(reviewHeaderId, recommendedBy, recommendedAction, remarks);
                    break;
                case "U":
                    recommendationAdded = await _reviewHeaderRepository.UpdateUnitHeadRecommendationAsync(reviewHeaderId, recommendedBy, recommendedAction, remarks);
                    break;
                case "D":
                    recommendationAdded = await _reviewHeaderRepository.UpdateDepartmentHeadRecommendationAsync(reviewHeaderId, recommendedBy, recommendedAction, remarks);
                    break;
                case "H":
                    recommendationAdded = await _reviewHeaderRepository.UpdateHrRecommendationAsync(reviewHeaderId, recommendedBy, recommendedAction, remarks);
                    break;
                case "M":
                    recommendationAdded = await _reviewHeaderRepository.UpdateManagementDecisionAsync(reviewHeaderId, recommendedBy, recommendedAction, remarks);
                    break;
                default:
                    break;
            }
            return recommendationAdded;
        }

        #endregion

        #region Review Stage Read Service Methods
        //============= Review Stage Read Service Methods =======================//
        public async Task<List<ReviewStage>> GetReviewStagesAsync()
        {
            List<ReviewStage> reviewStageList = new List<ReviewStage>();
            var entities = await _reviewStageRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                reviewStageList = entities.ToList();
            }
            return reviewStageList;
        }
        public async Task<List<ReviewStage>> GetPreviousReviewStagesAsync(int currentStageId)
        {
            List<ReviewStage> reviewStageList = new List<ReviewStage>();
            var entities = await _reviewStageRepository.GetAllPreviousAsync(currentStageId);
            if (entities != null && entities.Count > 0)
            {
                reviewStageList = entities.ToList();
            }
            return reviewStageList;
        }
        #endregion

        #region Review Metric Service Methods

        //=================== Review Metric Read Service Methods ======================================================//
        public async Task<List<ReviewMetric>> GetReviewMetricsAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetrics = new List<ReviewMetric>();
            var entities = await _reviewMetricRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewMetrics = entities;
            }
            return reviewMetrics;
        }

        public async Task<List<ReviewMetric>> GetKpasAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetrics = new List<ReviewMetric>();
            var entities = await _reviewMetricRepository.GetKpasByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewMetrics = entities;
            }
            return reviewMetrics;
        }

        public async Task<List<ReviewMetric>> GetCompetenciesAsync(int reviewHeaderId)
        {
            List<ReviewMetric> reviewMetrics = new List<ReviewMetric>();
            var entities = await _reviewMetricRepository.GetCmpsByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewMetrics = entities;
            }
            return reviewMetrics;
        }

        public async Task<ReviewMetric> GetReviewMetricAsync(int reviewMetricId)
        {
            ReviewMetric reviewMetric = new ReviewMetric();
            var entities = await _reviewMetricRepository.GetByIdAsync(reviewMetricId);
            if (entities != null && entities.Count > 0)
            {
                reviewMetric = entities.FirstOrDefault();
            }
            return reviewMetric;
        }

        public async Task<int> GetMetricCountAsync(int reviewHeaderId, ReviewMetricType reviewMetricType)
        {
            int metricCount = 0;
            if (reviewMetricType == ReviewMetricType.Competency)
            {
                metricCount = await _reviewMetricRepository.GetCmpCountByReviewHeaderIdAsync(reviewHeaderId);
            }
            else
            {
                metricCount = await _reviewMetricRepository.GetKpaCountByReviewHeaderIdAsync(reviewHeaderId);
            }
            return metricCount;
        }


        //================== Review Metric Write Service Methods =====================================================//
        public async Task<bool> AddReviewMetricAsync(ReviewMetric reviewMetric)
        {
            if (reviewMetric == null) { throw new ArgumentNullException(nameof(reviewMetric)); }
            var entities = await _reviewMetricRepository.GetByMetricDescriptionAsync(reviewMetric.ReviewHeaderId, reviewMetric.ReviewMetricDescription);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Duplicate entry error. This item has already been added.");
            }

            if (reviewMetric.ReviewMetricTypeId == (int)ReviewMetricType.Competency)
            {
                int max_no_cmps = 0;
                var session_entities = await _reviewSessionRepository.GetByIdAsync(reviewMetric.ReviewSessionId);
                if (session_entities != null && session_entities.Count > 0)
                {
                    ReviewSession reviewSession = session_entities.FirstOrDefault();
                    if (reviewSession != null)
                    {
                        max_no_cmps = reviewSession.MaxNoOfCompetencies;
                    }
                }

                var added_entities = await _reviewMetricRepository.GetCmpsByReviewHeaderIdAsync(reviewMetric.ReviewHeaderId);
                if (added_entities != null && added_entities.Count >= max_no_cmps)
                {
                    throw new Exception("Sorry, you have reached the maximum number of Competencies permitted for this Appraisal Session.");
                }
            }
            return await _reviewMetricRepository.AddAsync(reviewMetric);
        }

        public async Task<bool> UpdateReviewMetricAsync(ReviewMetric reviewMetric)
        {
            if (reviewMetric == null) { throw new ArgumentNullException(nameof(reviewMetric)); }
            return await _reviewMetricRepository.UpdateAsync(reviewMetric);
        }

        public async Task<bool> DeleteReviewMetricAsync(int reviewMetricId)
        {
            if (reviewMetricId < 1) { throw new ArgumentNullException(nameof(reviewMetricId)); }
            return await _reviewMetricRepository.DeleteAsync(reviewMetricId);
        }
        #endregion

        #region Competency Service Method
        public async Task<List<Competency>> GetFromCompetencyDictionaryAsync()
        {
            List<Competency> competencies = new List<Competency>();
            var entities = await _competencyRepository.GetByAllAsync();
            if (entities != null && entities.Count > 0)
            {
                competencies = entities;
            }
            return competencies;
        }

        public async Task<List<Competency>> GetFromCompetencyDictionaryByCategoryAsync(int CategoryId)
        {
            List<Competency> competencies = new List<Competency>();
            var entities = await _competencyRepository.GetByCategoryIdAsync(CategoryId);
            if (entities != null && entities.Count > 0)
            {
                competencies = entities;
            }
            return competencies;
        }

        public async Task<List<Competency>> GetFromCompetencyDictionaryByLevelAsync(int LevelId)
        {
            List<Competency> competencies = new List<Competency>();
            var entities = await _competencyRepository.GetByLevelIdAsync(LevelId);
            if (entities != null && entities.Count > 0)
            {
                competencies = entities;
            }
            return competencies;
        }

        public async Task<List<Competency>> SearchFromCompetencyDictionaryAsync(int CategoryId, int LevelId)
        {
            List<Competency> competencies = new List<Competency>();
            if (CategoryId > 0)
            {
                if (LevelId > 0)
                {
                    var category_level_entities = await _competencyRepository.GetByCategoryIdAndLevelIdAsync(CategoryId, LevelId);
                    if (category_level_entities != null && category_level_entities.Count > 0)
                    {
                        competencies = category_level_entities;
                    }
                }
                else
                {
                    var category_entities = await _competencyRepository.GetByCategoryIdAsync(CategoryId);
                    if (category_entities != null && category_entities.Count > 0)
                    {
                        competencies = category_entities;
                    }
                }
            }
            else
            {
                if (LevelId > 0)
                {
                    var level_entities = await _competencyRepository.GetByLevelIdAsync(LevelId);
                    if (level_entities != null && level_entities.Count > 0)
                    {
                        competencies = level_entities;
                    }
                }
                else
                {
                    var entities = await _competencyRepository.GetByAllAsync();
                    if (entities != null && entities.Count > 0)
                    {
                        competencies = entities;
                    }
                }
            }
            return competencies;
        }

        public async Task<Competency> GetFromCompetencyDictionaryByIdAsync(int CompetencyId)
        {
            Competency competency = new Competency();
            var entities = await _competencyRepository.GetByIdAsync(CompetencyId);
            if (entities != null && entities.Count > 0)
            {
                competency = entities.FirstOrDefault();
            }
            return competency;
        }
        
        public async Task<bool> AddCompetencyAsync(Competency competency)
        {
            if (competency == null || string.IsNullOrWhiteSpace(competency.Title)) { throw new ArgumentNullException(nameof(competency)); }
            var entities = await _competencyRepository.GetByTitleAsync(competency.Title);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("Duplicate Entry! Sorry, a Competency with the same title already exists in the system.");
            }

            return await _competencyRepository.AddAsync(competency);
        }

        public async Task<bool> UpdateCompetencyAsync(Competency competency)
        {
            if (competency == null || string.IsNullOrWhiteSpace(competency.Title)) { throw new ArgumentNullException(nameof(competency)); }
            var entities = await _competencyRepository.GetByTitleAsync(competency.Title);
            if (entities != null && entities.Count > 0)
            {
                List<Competency> competencies = entities.ToList();
                foreach (Competency cmp in competencies)
                {
                    if (cmp.Id != competency.Id)
                    {
                        throw new Exception("Duplicate Entry! Sorry, a Competency with the same title already exists in the system.");
                    }
                }
            }
            return await _competencyRepository.UpdateAsync(competency);
        }

        public async Task<bool> DeleteCompetencyAsync(int competencyId)
        {
            if (competencyId < 1) { throw new ArgumentNullException(nameof(competencyId)); }
            return await _competencyRepository.DeleteAsync(competencyId);
        }



        //======= Competency Categories Service Methods =========//
        public async Task<List<CompetencyCategory>> GetCompetencyCategoriesAsync()
        {
            List<CompetencyCategory> categories = new List<CompetencyCategory>();
            var entities = await _pmsSystemRepository.GetAllCompetencyCategoriesAsync();
            if (entities != null && entities.Count > 0)
            {
                categories = entities;
            }
            return categories;
        }

        public async Task<CompetencyCategory> GetCompetencyCategoryAsync(int competencyCategoryId)
        {
            return await _competencyRepository.GetCompetencyCategoryByIdAsync(competencyCategoryId);
        }

        public async Task<bool> AddCompetencyCategoryAsync(string competencyCategoryDescription)
        {
            if (string.IsNullOrEmpty(competencyCategoryDescription)) { throw new ArgumentNullException(nameof(competencyCategoryDescription)); }
            var entity = await _competencyRepository.GetCompetencyCategoryByDescriptionAsync(competencyCategoryDescription);
            if (entity != null && entity.Id > 0)
            {
                throw new Exception("Duplicate Entry! Sorry, a Competency Category with the same name already exists in the system.");
            }

            return await _competencyRepository.AddCompetencyCategoryAsync(competencyCategoryDescription);
        }

        public async Task<bool> UpdateCompetencyCategoryAsync(int competencyCategoryId, string competencyCategoryDescription)
        {
            if (string.IsNullOrWhiteSpace(competencyCategoryDescription)) { throw new ArgumentNullException(nameof(competencyCategoryDescription)); }
            if (competencyCategoryId < 1) { throw new ArgumentNullException(nameof(competencyCategoryId)); }

            CompetencyCategory competencyCategory = await _competencyRepository.GetCompetencyCategoryByDescriptionAsync(competencyCategoryDescription);
            if (competencyCategory != null)
            {
                    if (competencyCategory.Id != competencyCategoryId && competencyCategory.Id > 0)
                    {
                        throw new Exception("Duplicate Entry! Sorry, a Competency Category with the same name already exists in the system.");
                    }
            }

            return await _competencyRepository.UpdateCompetencyCategoryAsync(competencyCategoryId, competencyCategoryDescription);
        }

        public async Task<bool> DeleteCompetencyCategoryAsync(int competencyCategoryId)
        {
            if (competencyCategoryId < 1) { throw new ArgumentNullException(nameof(competencyCategoryId)); }
            return await _competencyRepository.DeleteCompetencyCategoryAsync(competencyCategoryId);
        }



        //======= Competency Level Service Methods ==============//
        public async Task<List<CompetencyLevel>> GetCompetencyLevelsAsync()
        {
            List<CompetencyLevel> levels = new List<CompetencyLevel>();
            var entities = await _pmsSystemRepository.GetAllCompetencyLevelsAsync();
            if (entities != null && entities.Count > 0)
            {
                levels = entities;
            }
            return levels;
        }

        public async Task<CompetencyLevel> GetCompetencyLevelAsync(int competencyLevelId)
        {
            return await _competencyRepository.GetCompetencyLevelByIdAsync(competencyLevelId);
        }

        public async Task<bool> AddCompetencyLevelAsync(string competencyLevelDescription)
        {
            if (string.IsNullOrEmpty(competencyLevelDescription)) { throw new ArgumentNullException(nameof(competencyLevelDescription)); }
            var entity = await _competencyRepository.GetCompetencyCategoryByDescriptionAsync(competencyLevelDescription);
            if (entity != null && entity.Id > 0)
            {
                throw new Exception("Duplicate Entry! Sorry, a Competency Level with the same name already exists in the system.");
            }

            return await _competencyRepository.AddCompetencyLevelAsync(competencyLevelDescription);
        }

        public async Task<bool> UpdateCompetencyLevelAsync(int competencyLevelId, string competencyLevelDescription)
        {
            if (string.IsNullOrWhiteSpace(competencyLevelDescription)) { throw new ArgumentNullException(nameof(competencyLevelDescription)); }
            if (competencyLevelId < 1) { throw new ArgumentNullException(nameof(competencyLevelId)); }

            CompetencyLevel competencyLevel = await _competencyRepository.GetCompetencyLevelByDescriptionAsync(competencyLevelDescription);
            if (competencyLevel != null)
            {
                if (competencyLevel.Id != competencyLevelId && competencyLevel.Id > 0)
                {
                    throw new Exception("Duplicate Entry! Sorry, a Competency Level with the same name already exists in the system.");
                }
            }

            return await _competencyRepository.UpdateCompetencyLevelAsync(competencyLevelId, competencyLevelDescription);
        }

        public async Task<bool> DeleteCompetencyLevelAsync(int competencyLevelId)
        {
            if (competencyLevelId < 1) { throw new ArgumentNullException(nameof(competencyLevelId)); }
            return await _competencyRepository.DeleteCompetencyLevelAsync(competencyLevelId);
        }

        #endregion

        #region Review CDG Service Methods
        public async Task<List<ReviewCDG>> GetReviewCdgsAsync(int reviewHeaderId)
        {
            List<ReviewCDG> reviewCdgs = new List<ReviewCDG>();
            var entities = await _reviewCDGRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewCdgs = entities;
            }
            return reviewCdgs;
        }

        public async Task<ReviewCDG> GetReviewCdgAsync(int reviewCdgId)
        {
            ReviewCDG reviewCDG = new ReviewCDG();
            var entities = await _reviewCDGRepository.GetByIdAsync(reviewCdgId);
            if (entities != null && entities.Count > 0)
            {
                reviewCDG = entities.FirstOrDefault();
            }
            return reviewCDG;
        }

        public async Task<bool> AddReviewCdgAsync(ReviewCDG reviewCdg)
        {
            if (reviewCdg == null) { throw new ArgumentNullException(nameof(reviewCdg)); }
            return await _reviewCDGRepository.AddAsync(reviewCdg);
        }

        public async Task<bool> UpdateReviewCdgAsync(ReviewCDG reviewCdg)
        {
            if (reviewCdg == null) { throw new ArgumentNullException(nameof(reviewCdg)); }
            return await _reviewCDGRepository.UpdateAsync(reviewCdg);
        }

        public async Task<bool> DeleteReviewCdgAsync(int reviewCdgId)
        {
            if (reviewCdgId < 1) { throw new ArgumentNullException(nameof(reviewCdgId)); }
            return await _reviewCDGRepository.DeleteAsync(reviewCdgId);
        }

        #endregion

        #region PMS Utility Service Methods
        public async Task<MoveToNextStageModel> ValidateMoveRequestAsync(int reviewHeaderId, string appraiserId = null)
        {
            MoveToNextStageModel model = new MoveToNextStageModel();
            model.ErrorMessages = new List<string>();
            ReviewHeader reviewHeader = new ReviewHeader();
            ReviewSession reviewSession = new ReviewSession();
            //int nextStepId = 
            var entities = await _reviewHeaderRepository.GetByIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewHeader = entities.FirstOrDefault();
                model.ReviewHeaderID = reviewHeader.ReviewHeaderId;
                model.AppraiseeID = reviewHeader.AppraiseeId;
                model.CurrentStageID = reviewHeader.ReviewStageId;
                model.ReviewSessionID = reviewHeader.ReviewSessionId;
                model.PrincipalAppraiserID = reviewHeader.PrimaryAppraiserId ?? string.Empty;

                switch (model.CurrentStageID)
                {
                    case 1:
                        if (!string.IsNullOrWhiteSpace(reviewHeader.PerformanceGoal))
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 2;
                        }
                        else
                        {
                            model.ErrorMessages.Add("You have not defined any Performance Goal for this performance period. Please defined at least one Performance Goal before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    case 2:
                        var revision_entities = await _reviewSessionRepository.GetByIdAsync(reviewHeader.ReviewSessionId);
                        reviewSession = revision_entities.ToList().FirstOrDefault();
                        decimal required_Kpa_Weightage = reviewSession.TotalKpaScore;
                        decimal cumulative_kpa_weightage = await _reviewMetricRepository.GetTotalKpaWeightageByReviewHeaderIdAsync(model.ReviewHeaderID);
                        if (required_Kpa_Weightage == cumulative_kpa_weightage)
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 3;
                        }
                        else if (required_Kpa_Weightage > cumulative_kpa_weightage)
                        {
                            model.ErrorMessages.Add($"The recommended total weightage of all KPAs is {required_Kpa_Weightage}. But yours is {cumulative_kpa_weightage} which falls short of the requirement. You may need to increase the weightages of some of your KPAs to get the total to {required_Kpa_Weightage}.");
                            model.IsQualifiedToMove = false;
                        }
                        else if (required_Kpa_Weightage < cumulative_kpa_weightage)
                        {
                            model.ErrorMessages.Add($"The recommended total weightage of all KPAs is {required_Kpa_Weightage}. But yours is {cumulative_kpa_weightage} which exceeds the requirement. You may need to reduce the weightages of some of your KPAs in order to get the total to {required_Kpa_Weightage}.");
                            model.IsQualifiedToMove = false;
                        }
                        else
                        {
                            model.ErrorMessages.Add($"The recommended total weightage of all KPAs is {required_Kpa_Weightage}. But yours is {cumulative_kpa_weightage} you may need to make the necessary changes to the weightages of some of your KPAs in order to get the total to {required_Kpa_Weightage}.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    case 3:
                        var rev_entities = await _reviewSessionRepository.GetByIdAsync(reviewHeader.ReviewSessionId);
                        reviewSession = rev_entities.ToList().FirstOrDefault();
                        int min_cmp_count = reviewSession.MinNoOfCompetencies;
                        int max_cmp_count = reviewSession.MaxNoOfCompetencies;
                        int cmp_count = await _reviewMetricRepository.GetCmpCountByReviewHeaderIdAsync(model.ReviewHeaderID);
                        if (cmp_count >= min_cmp_count && cmp_count <= max_cmp_count)
                        {
                            decimal required_cmp_Weightage = reviewSession.TotalCompetencyScore;
                            decimal cumulative_cmp_weightage = await _reviewMetricRepository.GetTotalCmpWeightageByReviewHeaderIdAsync(model.ReviewHeaderID);
                            if (required_cmp_Weightage == cumulative_cmp_weightage)
                            {
                                model.IsQualifiedToMove = true;
                                model.NextStageID = 4;
                            }
                            else if (required_cmp_Weightage > cumulative_cmp_weightage)
                            {
                                model.ErrorMessages.Add($"The recommended total weightage of all Competencies is {required_cmp_Weightage}. But yours is {cumulative_cmp_weightage} which falls short of the requirement. You may need to ensure all your competencies have the recommended weightages to bring the total to {required_cmp_Weightage}.");
                                model.IsQualifiedToMove = false;
                            }
                            else if (required_cmp_Weightage < cumulative_cmp_weightage)
                            {
                                model.ErrorMessages.Add($"The recommended total weightage of all Competencies is {required_cmp_Weightage}. But yours is {cumulative_cmp_weightage} which exceeds the requirement. You may need to ensure all your competencies have the recommended weightages to bring the total to {required_cmp_Weightage}.");
                                model.IsQualifiedToMove = false;
                            }
                            else
                            {
                                model.ErrorMessages.Add($"The recommended total weightage of all Competencies is {required_cmp_Weightage}. But yours is {cumulative_cmp_weightage}. You may want to ensure all your competencies have the recommended weightages to bring the total to {required_cmp_Weightage}.");
                                model.IsQualifiedToMove = false;
                            }
                        }
                        else if (cmp_count < min_cmp_count)
                        {
                            model.ErrorMessages.Add($"The recommended minimum number of Competencies is {min_cmp_count}. But yours is {cmp_count} which is less than the required number. You may need to add more Competencies to get the total number to at least {min_cmp_count}.");
                            model.IsQualifiedToMove = false;
                        }
                        else if (cmp_count > max_cmp_count)
                        {
                            model.ErrorMessages.Add($"The recommended maximum number of Competencies is {max_cmp_count}. But yours is {cmp_count} which is more than the required number. You may need to delete some Competencies to get the total number to {max_cmp_count} at most.");
                            model.IsQualifiedToMove = false;
                        }
                        else
                        {
                            model.ErrorMessages.Add($"The recommended minimum number of Competencies is {min_cmp_count}, and the recommended maximum number is {max_cmp_count}. Make sure your total number of e Competencies falls within this range.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    case 4:
                        List<ReviewCDG> reviewCdgList = new List<ReviewCDG>();
                        var reviewCDG_entities = await _reviewCDGRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
                        if (reviewCDG_entities != null && reviewCDG_entities.Count > 0)
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 5;
                        }
                        else
                        {
                            model.ErrorMessages.Add("You have not defined any Career Development Goal for this performance period. Please defined at least one Career Development Goal before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    case 5:
                        var approval_entities = await _reviewApprovalRepository.GetByReviewHeaderIdAsync(reviewHeaderId, 1);
                        var must_approve_contract_roles = await _approvalRoleRepository.GetMustApproveContractsAsync();

                        if (must_approve_contract_roles != null && must_approve_contract_roles.Count < 1)
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 6;
                        }
                        else
                        {
                            if (approval_entities != null && approval_entities.Count < 1)
                            {
                                model.IsQualifiedToMove = false;
                                model.ErrorMessages.Add("There is no record of any approval for this Appraisal. You need to confirm that all the necessary approvals have been gotten before you can proceed to the next step. ");
                            }
                            else
                            {
                                int no_omitted = 0;
                                foreach (var r in must_approve_contract_roles)
                                {
                                    int no_matches = 0;
                                    foreach (var a in approval_entities)
                                    {
                                        if (r.ApprovalRoleId == a.ApproverRoleId)
                                        {
                                            no_matches++;
                                        }
                                    }
                                    if (no_matches == 0) { no_omitted++; }
                                }

                                if (no_omitted > 0)
                                {
                                    model.IsQualifiedToMove = false;
                                    model.ErrorMessages.Add("It appears all necessary approvals are not yet gotten. You need to confirm that all the necessary approvals have been gotten before you can proceed to the next step. ");
                                }
                                else
                                {
                                    model.IsQualifiedToMove = true;
                                    model.NextStageID = 6;
                                }
                            }
                        }
                        break;
                    case 7:
                        if (reviewHeader.ContractIsAccepted != null && reviewHeader.ContractIsAccepted == true)
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 8;
                        }
                        else
                        {
                            model.IsQualifiedToMove = false;
                            model.ErrorMessages.Add("The Appraisee has not yet signed off on the Performance Contract. You have not signed off on the Performance Contract. You cannot proceed to the Evaluation Phase without signing off on the Performance Contract. ");
                        }
                        break;
                    case 8:
                        var kpa_entities = await _reviewMetricRepository.GetUnevaluatedByMetricTypeIdAsync(reviewHeaderId, appraiserId, (int)ReviewMetricType.KPA);
                        if (kpa_entities != null && kpa_entities.Count > 0)
                        {
                            int no_kpa = kpa_entities.Count;
                            model.ErrorMessages.Add($"You still have {no_kpa} KPAs that have not been evaluated. All KPAs must be evaluated before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        else if (kpa_entities != null && kpa_entities.Count < 1)
                        {
                            var cmp_entities = await _reviewMetricRepository.GetUnevaluatedByMetricTypeIdAsync(reviewHeaderId, appraiserId, (int)ReviewMetricType.Competency);
                            if (cmp_entities != null && cmp_entities.Count > 0)
                            {
                                int no_cmp = cmp_entities.Count;
                                model.ErrorMessages.Add($"You still have {no_cmp} Competencies that have not been evaluated. All Competencies must be evaluated before you can be allowed to move to the next step.");
                                model.IsQualifiedToMove = false;
                            }
                            else if (cmp_entities != null && cmp_entities.Count < 1)
                            {
                                model.IsQualifiedToMove = true;
                                model.NextStageID = 9;
                            }
                            else
                            {
                                int no_cmp = cmp_entities.Count;
                                model.ErrorMessages.Add($"An error was encountered while attempting to retrieve your evaluation result. No evaluation result was found. Please try again.");
                                model.IsQualifiedToMove = false;
                            }
                        }
                        else
                        {
                            int no_kpa = kpa_entities.Count;
                            model.ErrorMessages.Add($"An error was encountered while attempting to retrieve your evaluation result. No evaluation result was found. Please try again.");
                            model.IsQualifiedToMove = false;
                        }
                        break;
                    case 9:
                        var selfEvaluationKpaResultEntities = await _reviewResultRepository.GetByAppraiserIdAndMetricTypeId(model.ReviewHeaderID, model.AppraiseeID, (int)ReviewMetricType.KPA);
                        var selfEvaluationCmpResultEntities = await _reviewResultRepository.GetByAppraiserIdAndMetricTypeId(model.ReviewHeaderID, model.AppraiseeID, (int)ReviewMetricType.Competency);

                        var finalEvaluationKpaResultEntities = await _reviewResultRepository.GetIntitalByThirdPartyAsync(model.ReviewHeaderID, model.PrincipalAppraiserID, (int)ReviewMetricType.KPA);
                        var finalEvaluationCmpResultEntities = await _reviewResultRepository.GetIntitalByThirdPartyAsync(model.ReviewHeaderID, model.PrincipalAppraiserID, (int)ReviewMetricType.Competency);

                        if (finalEvaluationKpaResultEntities == null || finalEvaluationKpaResultEntities.Count < 1)
                        {
                            model.ErrorMessages.Add($"No KPA evaluation result was found for the Principal Appraiser. Please make sure the Principal Appraiser has completed the final evaluation of your KPAs before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        else if (finalEvaluationCmpResultEntities == null || finalEvaluationCmpResultEntities.Count < 1)
                        {
                            model.ErrorMessages.Add($"No Competency evaluation result was found for the Principal Appraiser. Please make sure the Principal Appraiser has completed the final evaluation of your Competencies before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        else if (selfEvaluationKpaResultEntities.Count != finalEvaluationKpaResultEntities.Count)
                        {
                            model.ErrorMessages.Add($"The result of the KPA evaluation by your Principal Appraiser appears to be incomplete. Please ensure all KPAs are fully evaluated by your Principal Appraiser before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        else if (selfEvaluationCmpResultEntities.Count != finalEvaluationCmpResultEntities.Count)
                        {
                            model.ErrorMessages.Add($"The result of the Competencies evaluation by your Principal Appraiser appears to be incomplete. Please ensure all Competencies are fully evaluated by your Principal Appraiser before attempting to move to the next step.");
                            model.IsQualifiedToMove = false;
                        }
                        else
                        {
                            if (model.ErrorMessages != null && model.ErrorMessages.Count > 0)
                            {
                                model.IsQualifiedToMove = false;
                            }
                            else
                            {
                                model.IsQualifiedToMove = true;
                                model.NextStageID = 10;
                            }
                        }
                        break;
                    case 10:
                        var result_approval_entities = await _reviewApprovalRepository.GetByReviewHeaderIdAsync(reviewHeaderId, (int)ReviewApprovalType.ApproveEvaluationResult);
                        var must_approve_result_roles = await _approvalRoleRepository.GetMustApproveEvaluationsAsync();

                        if (must_approve_result_roles != null && must_approve_result_roles.Count < 1)
                        {
                            model.IsQualifiedToMove = true;
                            model.NextStageID = 11;
                        }
                        else
                        {
                            if (result_approval_entities != null && result_approval_entities.Count < 1)
                            {
                                model.IsQualifiedToMove = false;
                                model.ErrorMessages.Add("There is no record of any approval for this Appraisal. You need to confirm that all the necessary approvals have been gotten before you can proceed to the next step. ");
                            }
                            else
                            {
                                int no_omitted = 0;
                                foreach (var r in must_approve_result_roles)
                                {
                                    int no_matches = 0;
                                    foreach (var a in result_approval_entities)
                                    {
                                        if (r.ApprovalRoleId == a.ApproverRoleId)
                                        {
                                            no_matches++;
                                        }
                                    }
                                    if (no_matches == 0) { no_omitted++; }
                                }

                                if (no_omitted > 0)
                                {
                                    model.IsQualifiedToMove = false;
                                    model.ErrorMessages.Add("It appears all necessary approvals are not yet gotten. You need to confirm that all the necessary approvals have been gotten before you can proceed to the next step. ");
                                }
                                else
                                {
                                    model.IsQualifiedToMove = true;
                                    model.NextStageID = 11;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                model.ErrorMessages.Add("No record was found for this appraisal session. Please ensure you have created at least one performance goal before attempting to move to the next step. Thank you. ");
                model.IsQualifiedToMove = false;
            }
            return model;
        }
        public async Task<List<PmsActivityHistory>> GetPmsActivityHistory(int reviewHeaderId)
        {
            List<PmsActivityHistory> activityList = new List<PmsActivityHistory>();
            var entities = await _pmsActivityHistoryRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                activityList = entities.ToList();
            }
            return activityList;
        }
        public async Task<bool> AddPmsActivityHistoryAsync(PmsActivityHistory pmsActivityHistory)
        {
            if (pmsActivityHistory != null)
            {
                return await _pmsActivityHistoryRepository.AddAsync(pmsActivityHistory);
            }
            return false;
        }
        #endregion

        #region Performance Settings Read Service Methods
        public async Task<List<ReviewType>> GetReviewTypesAsync()
        {
            List<ReviewType> reviewTypeList = new List<ReviewType>();
            var entities = await _performanceSettingsRepository.GetAllReviewTypesAsync();
            if (entities != null && entities.Count > 0)
            {
                reviewTypeList = entities.ToList();
            }
            return reviewTypeList;
        }
        public async Task<List<AppraisalRecommendation>> GetAppraisalRecommendationsAsync()
        {
            List<AppraisalRecommendation> appraisalRecommendationList = new List<AppraisalRecommendation>();
            var entities = await _performanceSettingsRepository.GetAllRecommendationsAsync();
            if (entities != null && entities.Count > 0)
            {
                appraisalRecommendationList = entities.ToList();
            }
            return appraisalRecommendationList;
        }
        #endregion

        #region Review Submission Service Methods

        public async Task<bool> AddReviewSubmissionAsync(ReviewSubmission reviewSubmission)
        {
            if (reviewSubmission == null) { throw new ArgumentNullException(nameof(reviewSubmission)); }
            return await _reviewSubmissionRepository.AddAsync(reviewSubmission);
        }

        public async Task<bool> UpdateReviewSubmissionAsync(int reviewSubmissionId)
        {
            if (reviewSubmissionId < 1) { throw new ArgumentNullException(nameof(reviewSubmissionId)); }
            return await _reviewSubmissionRepository.UpdateAsync(reviewSubmissionId);
        }

        public async Task<bool> DeleteReviewSubmissionAsync(int reviewSubmissionId)
        {
            if (reviewSubmissionId < 1) { throw new ArgumentNullException(nameof(reviewSubmissionId)); }
            return await _reviewSubmissionRepository.DeleteAsync(reviewSubmissionId);
        }

        public async Task<ReviewSubmission> GetReviewSubmissionByIdAsync(int reviewSubmissionId)
        {
            ReviewSubmission reviewSubmission = new ReviewSubmission();
            var entities = await _reviewSubmissionRepository.GetByIdAsync(reviewSubmissionId);
            if (entities != null && entities.Count > 0)
            {
                reviewSubmission = entities.FirstOrDefault();
            }
            return reviewSubmission;
        }

        public async Task<List<ReviewSubmission>> GetReviewSubmissionsByApproverIdAsync(string reviewerId, int? reviewSessionId = null)
        {
            List<ReviewSubmission> reviewSubmissions = new List<ReviewSubmission>();
            if (reviewSessionId != null && reviewSessionId > 0)
            {
                var entities = await _reviewSubmissionRepository.GetByReviewerIdAndReviewSessionIdAsync(reviewerId, reviewSessionId.Value);
                if (entities != null && entities.Count > 0)
                {
                    reviewSubmissions = entities;
                }
            }
            else
            {
                var entities = await _reviewSubmissionRepository.GetByReviewerIdAsync(reviewerId);
                if (entities != null && entities.Count > 0)
                {
                    reviewSubmissions = entities;
                }
            }
            return reviewSubmissions;
        }

        public async Task<List<ReviewSubmission>> GetReviewSubmissionsByReviewHeaderIdAsync(int reviewHeaderId, int? submissionPurposeId = null, string submittedToEmployeeId = null, int? submittedToEmployeeRoleId = null)
        {
            List<ReviewSubmission> reviewSubmissions = new List<ReviewSubmission>();

            if ((submissionPurposeId == null || submissionPurposeId < 1) && string.IsNullOrWhiteSpace(submittedToEmployeeId))
            {
                var entities = await _reviewSubmissionRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
                if (entities != null && entities.Count > 0)
                {
                    reviewSubmissions = entities;
                }
            }
            else if ((submissionPurposeId != null && submissionPurposeId > 0) && string.IsNullOrWhiteSpace(submittedToEmployeeId))
            {
                var entities = await _reviewSubmissionRepository.GetByReviewHeaderIdAndSubmissionPurposeIdAsync(reviewHeaderId, submissionPurposeId.Value);
                if (entities != null && entities.Count > 0)
                {
                    reviewSubmissions = entities;
                }
            }
            else if ((submissionPurposeId != null && submissionPurposeId > 0) && !string.IsNullOrWhiteSpace(submittedToEmployeeId))
            {
                var entities = await _reviewSubmissionRepository.GetByReviewHeaderIdAndSubmissionPurposeIdAsync(reviewHeaderId, submissionPurposeId.Value, submittedToEmployeeId);
                if (entities != null && entities.Count > 0)
                {
                    reviewSubmissions = entities;
                }
            }
            else if ((submissionPurposeId != null && submissionPurposeId > 0) && !string.IsNullOrWhiteSpace(submittedToEmployeeId) && (submittedToEmployeeRoleId != null && submittedToEmployeeRoleId > 0))
            {
                var entities = await _reviewSubmissionRepository.GetByReviewHeaderIdAndSubmissionPurposeIdAsync(reviewHeaderId, submissionPurposeId.Value, submittedToEmployeeId, submittedToEmployeeRoleId.Value);
                if (entities != null && entities.Count > 0)
                {
                    reviewSubmissions = entities;
                }
            }
            else
            {
                reviewSubmissions = null;
            }
            return reviewSubmissions;
        }

        #endregion

        #region Review Message Service Methods
        public async Task<List<ReviewMessage>> GetReviewMessagesAsync(int reviewHeaderId)
        {
            List<ReviewMessage> reviewMessages = new List<ReviewMessage>();
            var entities = await _reviewMessageRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                reviewMessages = entities;
            }
            return reviewMessages;
        }

        public async Task<ReviewMessage> GetReviewMessageAsync(int reviewMessageId)
        {
            ReviewMessage reviewMessage = new ReviewMessage();
            return await _reviewMessageRepository.GetByIdAsync(reviewMessageId); ;
        }

        public async Task<bool> AddReviewMessageAsync(ReviewMessage reviewMessage)
        {
            if (reviewMessage == null) { throw new ArgumentNullException(nameof(reviewMessage)); }
            return await _reviewMessageRepository.AddAsync(reviewMessage);
        }

        public async Task<bool> UpdateReviewMessageAsync(ReviewMessage reviewMessage)
        {
            if (reviewMessage == null) { throw new ArgumentNullException(nameof(reviewMessage)); }
            return await _reviewMessageRepository.UpdateAsync(reviewMessage);
        }

        public async Task<bool> DeleteReviewMessageAsync(int reviewMessageId)
        {
            if (reviewMessageId < 1) { throw new ArgumentNullException(nameof(reviewMessageId)); }
            return await _reviewMessageRepository.DeleteAsync(reviewMessageId);
        }

        #endregion

        #region Review Approval Service Methods
        public async Task<bool> ReturnContractToAppraisee(int nextStageId, int reviewSubmissionId, ReviewMessage reviewMessage = null)
        {
            if (reviewMessage != null) { await _reviewMessageRepository.AddAsync(reviewMessage); }

            bool appraisalIsUpdated = await _reviewHeaderRepository.UpdateStageIdAsync(reviewMessage.ReviewHeaderId, nextStageId);
            if (appraisalIsUpdated)
            {
                bool submissionIsUpdated = await _reviewSubmissionRepository.UpdateAsync(reviewSubmissionId);
                return submissionIsUpdated;
            }
            return false;
        }
        public async Task<bool> ApproveContractToAppraisee(ReviewApproval reviewApproval, int? reviewSubmissionId)
        {
            if (reviewApproval == null) { throw new Exception("Error: the required parameter [ReviewApproval] cannot be null."); }
            bool approvalAdded = false;
            approvalAdded = await _reviewApprovalRepository.AddAsync(reviewApproval);
            if (approvalAdded)
            {
                bool submissionIsUpdated = false;
                if (reviewSubmissionId != null && reviewSubmissionId > 0)
                {
                    var submission_entities = await _reviewSubmissionRepository.GetByIdAsync(reviewSubmissionId.Value);
                    if (submission_entities != null && submission_entities.Count > 0)
                    {
                        submissionIsUpdated = await _reviewSubmissionRepository.UpdateAsync(reviewSubmissionId.Value);
                    }
                }
                else
                {
                    var submission_entities = await _reviewSubmissionRepository.GetByReviewHeaderIdAndSubmissionPurposeIdAsync(reviewApproval.ReviewHeaderId, reviewApproval.SubmissionPurposeId, reviewApproval.ApproverId);
                    if (submission_entities != null && submission_entities.Count > 0)
                    {
                        submissionIsUpdated = await _reviewSubmissionRepository.UpdateAsync(reviewApproval.ReviewHeaderId, reviewApproval.ApproverId, reviewApproval.SubmissionPurposeId);
                    }
                    else
                    {
                        var evaluation_submission_entities = await _reviewSubmissionRepository.GetByReviewHeaderIdAndSubmissionPurposeIdAsync(reviewApproval.ReviewHeaderId, (int)ReviewSubmissionPurpose.FinalEvaluation, reviewApproval.ApproverId);
                        if (evaluation_submission_entities != null && evaluation_submission_entities.Count > 0)
                        {
                            ReviewSubmission reviewSubmission = evaluation_submission_entities.FirstOrDefault();
                            submissionIsUpdated = await _reviewSubmissionRepository.UpdateAsync(reviewSubmission.ReviewSubmissionId);
                            if (submissionIsUpdated)
                            {
                                reviewSubmission.SubmissionPurposeId = (int)ReviewSubmissionPurpose.ResultApproval;
                                reviewSubmission.IsActioned = true;
                                reviewSubmission.TimeActioned = DateTime.UtcNow;
                                await _reviewSubmissionRepository.AddAsync(reviewSubmission);
                            }
                        }
                        else
                        {
                            submissionIsUpdated = true;
                        }
                    }
                    //submissionIsUpdated = await _reviewSubmissionRepository.UpdateAsync(reviewApproval.ReviewHeaderId, reviewApproval.ApproverId, reviewApproval.SubmissionPurposeId);
                }

                if (!submissionIsUpdated)
                {
                    await _reviewApprovalRepository.DeleteAsync(reviewApproval);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> AcceptContractByAppraisee(int reviewHeaderId)
        {
            if (reviewHeaderId < 1) { throw new Exception("Error: the required parameter [ReviewHeaderID] cannot be null."); }
            bool acceptanceIsAdded = false;
            bool reviewStageUpdated = false;
            acceptanceIsAdded = await _reviewHeaderRepository.UpdateContractAcceptanceAsync(reviewHeaderId, true);
            if (acceptanceIsAdded)
            {
                reviewStageUpdated = await _reviewHeaderRepository.UpdateStageIdAsync(reviewHeaderId, 7);
            }
            return reviewStageUpdated;
        }
        public async Task<bool> AcceptEvaluationByAppraisee(int reviewHeaderId)
        {
            if (reviewHeaderId < 1) { throw new Exception("Error: the required parameter [ReviewHeaderID] cannot be null."); }
            bool acceptanceIsAdded = false;
            bool reviewStageUpdated = false;
            acceptanceIsAdded = await _reviewHeaderRepository.UpdateEvaluationAcceptanceAsync(reviewHeaderId, true);
            if (acceptanceIsAdded)
            {
                reviewStageUpdated = await _reviewHeaderRepository.UpdateStageIdAsync(reviewHeaderId, 12);
            }
            return reviewStageUpdated;
        }
        public async Task<List<ReviewApproval>> GetReviewApprovalsAsync(int reviewHeaderId, int? approvalTypeId = null, int? approverRoleId = null)
        {
            int ApprovalTypeId = approvalTypeId ?? 0;
            int ApproverRoleId = approverRoleId ?? 0;
            List<ReviewApproval> reviewApprovals = new List<ReviewApproval>();
            if(ApprovalTypeId > 0 && ApproverRoleId > 0)
            {
                var entities = await _reviewApprovalRepository.GetByReviewHeaderIdAsync(reviewHeaderId, ApprovalTypeId, ApproverRoleId);
                if (entities != null && entities.Count > 0)
                {
                    reviewApprovals = entities.ToList();
                }
            }
            else if(ApprovalTypeId > 0 && approverRoleId < 1)
            {
                var entities = await _reviewApprovalRepository.GetByReviewHeaderIdAsync(reviewHeaderId, ApprovalTypeId);
                if (entities != null && entities.Count > 0)
                {
                    reviewApprovals = entities.ToList();
                }
            }
            else
            {
                var entities = await _reviewApprovalRepository.GetByReviewHeaderIdAsync(reviewHeaderId);
                if (entities != null && entities.Count > 0)
                {
                    reviewApprovals = entities.ToList();
                }
            }

            return reviewApprovals;
        }
        public async Task<List<ReviewApproval>> GetReviewApprovalsApprovedAsync(int reviewHeaderId, int? approvalTypeId = null, int? approverRoleId = null)
        {
            int ApprovalTypeId = approvalTypeId ?? 0;
            int ApproverRoleId = approverRoleId ?? 0;
            List<ReviewApproval> reviewApprovals = new List<ReviewApproval>();
            if (ApprovalTypeId > 0 && ApproverRoleId > 0)
            {
                var entities = await _reviewApprovalRepository.GetApprovedByReviewHeaderIdAsync(reviewHeaderId, ApprovalTypeId, ApproverRoleId);
                if (entities != null && entities.Count > 0)
                {
                    reviewApprovals = entities.ToList();
                }
            }
            else if (ApprovalTypeId > 0 && approverRoleId < 1)
            {
                var entities = await _reviewApprovalRepository.GetApprovedByReviewHeaderIdAsync(reviewHeaderId, ApprovalTypeId);
                if (entities != null && entities.Count > 0)
                {
                    reviewApprovals = entities.ToList();
                }
            }
            else
            {
                var entities = await _reviewApprovalRepository.GetApprovedByReviewHeaderIdAsync(reviewHeaderId);
                if (entities != null && entities.Count > 0)
                {
                    reviewApprovals = entities.ToList();
                }
            }

            return reviewApprovals;
        }

        #endregion

        #region Review Result Service Methods
        public async Task<List<ReviewResult>> GetInitialReviewResultKpasAsync(int reviewHeaderId, string appraiserId)
        {
            List<ReviewResult> reviewResults = new List<ReviewResult>();
            var entities = await _reviewResultRepository.GetIntitalByMetricTypeIdAsync(reviewHeaderId, appraiserId, (int)ReviewMetricType.KPA);
            if (entities != null && entities.Count > 0)
            {
                reviewResults = entities.ToList();
            }
            return reviewResults;
        }

        public async Task<List<ReviewResult>> GetInitialReviewResultAsync(int reviewHeaderId, string appraiserId, int reviewMetricId)
        {
            List<ReviewResult> reviewResults = new List<ReviewResult>();
            var entities = await _reviewResultRepository.GetIntitalByMetricIdAsync(reviewHeaderId, appraiserId, reviewMetricId);
            if (entities != null && entities.Count > 0)
            {
                reviewResults = entities.ToList();
            }
            return reviewResults;
        }

        public async Task<List<ReviewResult>> GetInitialReviewResultCmpsAsync(int reviewHeaderId, string appraiserId)
        {
            List<ReviewResult> reviewResults = new List<ReviewResult>();
            var entities = await _reviewResultRepository.GetIntitalByMetricTypeIdAsync(reviewHeaderId, appraiserId, (int)ReviewMetricType.Competency);
            if (entities != null && entities.Count > 0)
            {
                reviewResults = entities.ToList();
            }
            return reviewResults;
        }

        public async Task<List<ReviewResult>> GetReviewResultByAppraiserIdAndReviewMetricIdAsync(int reviewHeaderId, string appraiserId, int reviewMetricId)
        {
            List<ReviewResult> reviewResults = new List<ReviewResult>();
            var entities = await _reviewResultRepository.GetByAppraiserIdAndMetricId(reviewHeaderId, appraiserId, reviewMetricId);
            if (entities != null && entities.Count > 0)
            {
                reviewResults = entities.ToList();
            }
            return reviewResults;
        }

        public async Task<List<ReviewResult>> GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(int reviewHeaderId, string appraiserId, int? reviewMetricTypeId = null)
        {
            List<ReviewResult> reviewResults = new List<ReviewResult>();
            if (reviewMetricTypeId == null)
            {
                var first_entities = await _reviewResultRepository.GetByAppraiserIdAndReviewHeaderId(reviewHeaderId, appraiserId);
                if (first_entities != null && first_entities.Count > 0)
                {
                    reviewResults = first_entities.ToList();
                }
            }
            else
            {
                var entities = await _reviewResultRepository.GetByAppraiserIdAndMetricTypeId(reviewHeaderId, appraiserId, reviewMetricTypeId.Value);
                if (entities != null && entities.Count > 0)
                {
                    reviewResults = entities.ToList();
                }
            }
            return reviewResults;
        }

        public async Task<ScoreSummary> GetScoreSummaryAsync(int reviewHeaderId, string appraiserId)
        {
            ScoreSummary scoreSummary = new ScoreSummary();
            if (reviewHeaderId > 0 && !string.IsNullOrWhiteSpace(appraiserId))
            {
                var entities = await _reviewResultRepository.GetScoresByReviewHeaderIdAndAppraiserIdAsync(reviewHeaderId, appraiserId);
                if (entities != null)
                {
                    decimal kpaScore = 0.00M;
                    decimal cmpScore = 0.00M;
                    foreach (var item in entities)
                    {
                        if (item.ReviewMetricTypeId == (int)ReviewMetricType.KPA)
                        {
                            kpaScore = item.TotalScore;
                        }
                        else if (item.ReviewMetricTypeId == (int)ReviewMetricType.Competency)
                        {
                            cmpScore = item.TotalScore;
                        }
                    }
                    scoreSummary.QualitativeScore = cmpScore;
                    scoreSummary.QuantitativeScore = kpaScore;
                    scoreSummary.TotalPerformanceScore = cmpScore + kpaScore;
                    scoreSummary.AppraiserId = appraiserId;
                    scoreSummary.ReviewHeaderId = reviewHeaderId;
                }
            }
            else
            {
                scoreSummary = null;
            }
            return scoreSummary;
        }

        public async Task<List<string>> GetAppraisersAsync(int reviewHeaderId)
        {
            return await _reviewResultRepository.GetAppraisersByReviewHeaderId(reviewHeaderId);
        }

        public async Task<List<AppraiserDetail>> GetAppraiserDetailsAsync(int reviewHeaderId)
        {
            List<AppraiserDetail> reviewDetails = new List<AppraiserDetail>();
            var entities = await _reviewResultRepository.GetAppraisersDetailsByReviewHeaderId(reviewHeaderId);
            if (entities != null && entities.Count > 0)
            {
                foreach (var item in entities)
                {
                    item.AppraiserFullDescription = $"{item.AppraiserName} ({item.AppraiserTypeDescription})";
                }
                reviewDetails = entities.ToList();
            }
            return reviewDetails;
        }

        public async Task<int> GetEvaluatedMetricCountAsync(int reviewHeaderId, string appraiserId, ReviewMetricType reviewMetricType)
        {
            int metricCount = 0;
            int reviewMetricTypeId = (int)reviewMetricType;

            if (reviewMetricType == ReviewMetricType.Competency)
            {
                metricCount = await _reviewResultRepository.GetMetricCountByAppraiserIdAndMetricTypeId(reviewHeaderId, appraiserId, reviewMetricTypeId);
            }
            else
            {
                metricCount = await _reviewResultRepository.GetMetricCountByAppraiserIdAndMetricTypeId(reviewHeaderId, appraiserId, reviewMetricTypeId);
            }
            return metricCount;
        }


        //========================== Result Summary Read Service Methods =======================================//
        public async Task<List<ResultSummary>> GetResultSummaryForReportsAsync(string reportToId, int reviewSessionId, string appraiseeId = null)
        {
            List<ResultSummary> resultSummaries = new List<ResultSummary>();
            if (appraiseeId == null)
            {
                var entities = await _reviewResultRepository.GetSummaryByReportToId(reportToId, reviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    resultSummaries = entities.ToList();
                }
            }
            else
            {
                var entities = await _reviewResultRepository.GetSummaryByReportToIdAndAppraiseeId(reportToId, reviewSessionId, appraiseeId);
                if (entities != null && entities.Count > 0)
                {
                    resultSummaries = entities.ToList();
                }
            }
            return resultSummaries;
        }

        public async Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAsync(int reviewSessionId)
        {
            List<ResultSummary> resultSummaries = new List<ResultSummary>();
            var entities = await _reviewResultRepository.GetSummaryByReviewSessionId(reviewSessionId);
            if (entities != null && entities.Count > 0)
            {
                resultSummaries = entities.ToList();
            }
            return resultSummaries;
        }

        public async Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndDepartmentCodeAsync(int reviewSessionId, int departmentId)
        {
            List<ResultSummary> resultSummaries = new List<ResultSummary>();
            var entities = await _reviewResultRepository.GetSummaryByReviewSessionIdAndDepartmentCode(reviewSessionId, departmentId);
            if (entities != null && entities.Count > 0)
            {
                resultSummaries = entities.ToList();
            }
            return resultSummaries;
        }

        public async Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndUnitCodeAsync(int reviewSessionId, int unitId)
        {
            List<ResultSummary> resultSummaries = new List<ResultSummary>();
            var entities = await _reviewResultRepository.GetSummaryByReviewSessionIdAndUnitCode(reviewSessionId, unitId);
            if (entities != null && entities.Count > 0)
            {
                resultSummaries = entities.ToList();
            }
            return resultSummaries;
        }

        public async Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndAppraiseeNameAsync(int reviewSessionId, string appraiseeName)
        {
            List<ResultSummary> resultSummaries = new List<ResultSummary>();
            var entities = await _reviewResultRepository.GetSummaryByReviewSessionIdAndAppraiseeName(reviewSessionId, appraiseeName);
            if (entities != null && entities.Count > 0)
            {
                resultSummaries = entities.ToList();
            }
            return resultSummaries;
        }


        //========================== Result Details Read Service Methods =======================================//
        public async Task<List<ResultDetail>> GetPrincipalResultDetailAsync(int reviewSessionId, int? locationId = null, int? departmentId = null, int? unitId = null)
        {
            List<ResultDetail> resultDetails = new List<ResultDetail>();

            if (unitId != null && unitId > 0)
            {
                var entities = await _reviewResultRepository.GetPrincipalResultDetailByUnitCodeAndReviewSessionIdAsync(reviewSessionId, unitId.Value);
                if (entities != null && entities.Count > 0)
                {
                    resultDetails = entities.ToList();
                }
            }
            else if (departmentId != null && departmentId > 0)
            {
                var entities = await _reviewResultRepository.GetPrincipalResultDetailByDepartmentCodeAndReviewSessionIdAsync(reviewSessionId, departmentId.Value);
                if (entities != null && entities.Count > 0)
                {
                    resultDetails = entities.ToList();
                }
            }
            else if (locationId != null && locationId > 0)
            {
                var entities = await _reviewResultRepository.GetPrincipalResultDetailByLocationIdAndReviewSessionIdAsync(reviewSessionId, locationId.Value);
                if (entities != null && entities.Count > 0)
                {
                    resultDetails = entities.ToList();
                }
            }
            else
            {
                var entities = await _reviewResultRepository.GetPrincipalResultDetailByReviewSessionIdAsync(reviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    resultDetails = entities.ToList();
                }
            }
            return resultDetails;
        }

        public async Task<List<ResultDetail>> GetRejectedPrincipalResultDetailAsync(int reviewSessionId, int? locationId = null)
        {
            List<ResultDetail> resultDetails = new List<ResultDetail>();

            if (locationId != null && locationId > 0)
            {
                var entities = await _reviewResultRepository.GetRejectedPrincipalResultDetailByLocationIdAndReviewSessionIdAsync(reviewSessionId, locationId.Value);
                if (entities != null && entities.Count > 0)
                {
                    resultDetails = entities.ToList();
                }
            }
            else
            {
                var entities = await _reviewResultRepository.GetRejectedPrincipalResultDetailByReviewSessionIdAsync(reviewSessionId);
                if (entities != null && entities.Count > 0)
                {
                    resultDetails = entities.ToList();
                }
            }
            return resultDetails;
        }


        //================== Review Result Write Service Methods =====================================================//

        public async Task<bool> UploadResults(int reviewHeaderId)
        {
            if (reviewHeaderId < 1) { return false; }
            ReviewHeader reviewHeader = new ReviewHeader();
            ResultSummary resultSummary = new ResultSummary();

            var reviewHeaderList = await _reviewHeaderRepository.GetByIdAsync(reviewHeaderId);

            if (reviewHeaderList == null || reviewHeaderList.Count != 1) { return false; }
            reviewHeader = reviewHeaderList.FirstOrDefault();
            resultSummary.ReviewSessionId = reviewHeader.ReviewSessionId;
            resultSummary.ReviewYearId = reviewHeader.ReviewYearId;
            resultSummary.AppraiseeId = reviewHeader.AppraiseeId;
            string PrimaryAppraiserId = reviewHeader.PrimaryAppraiserId;

            var appraiserIds = await _reviewResultRepository.GetAppraisersByReviewHeaderId(reviewHeaderId);
            if (appraiserIds == null || appraiserIds.Count < 1) { return false; }

            int no_inserted = 0;
            int no_results = appraiserIds.Count;
            foreach (string appraiserId in appraiserIds)
            {
                if (appraiserId == PrimaryAppraiserId) { resultSummary.IsMain = true; }

                var result_entities = await _reviewResultRepository.GetByAppraiserIdAndReviewHeaderId(reviewHeaderId, appraiserId);

                if (result_entities != null && result_entities.Count > 0)
                {
                    ReviewResult reviewResult = result_entities.FirstOrDefault();
                    resultSummary.AppraiserName = reviewResult.AppraiserName;
                    resultSummary.AppraiserRoleDescription = reviewResult.AppraiserRoleName;
                    resultSummary.AppraiserTypeDescription = reviewResult.AppraiserTypeDescription;
                    resultSummary.AppraiserId = appraiserId;
                    resultSummary.ReviewHeaderId = reviewHeaderId;

                    var entities = await _reviewResultRepository.GetScoresByReviewHeaderIdAndAppraiserIdAsync(reviewHeaderId, appraiserId);
                    if (entities != null)
                    {
                        decimal kpaScore = 0.00M;
                        decimal cmpScore = 0.00M;
                        foreach (var item in entities)
                        {
                            if (item.ReviewMetricTypeId == (int)ReviewMetricType.KPA)
                            {
                                kpaScore = item.TotalScore;
                            }
                            else if (item.ReviewMetricTypeId == (int)ReviewMetricType.Competency)
                            {
                                cmpScore = item.TotalScore;
                            }
                        }
                        resultSummary.KpaScoreObtained = kpaScore;
                        resultSummary.CompetencyScoreObtained = cmpScore;
                        resultSummary.CombinedScoreObtained = cmpScore + kpaScore;

                        var appraisal_grade_entities = await _appraisalGradeRepository.GetByReviewSessionIdAndGradeScoreAsync(resultSummary.ReviewSessionId, ReviewGradeType.Performance, resultSummary.CombinedScoreObtained);

                        if (appraisal_grade_entities != null && appraisal_grade_entities.Count > 0)
                        {
                            AppraisalGrade appraisalGrade = appraisal_grade_entities.FirstOrDefault();
                            if (appraisalGrade != null)
                            {
                                resultSummary.PerformanceRating = appraisalGrade.AppraisalGradeDescription;
                                resultSummary.ScoreRankDescription = appraisalGrade.GradeRankDescription;
                                resultSummary.ScoreRank = appraisalGrade.GradeRank;

                                var existing_result_summary = await _reviewResultRepository.GetSummaryByAppraiserIdAndReviewHeaderId(reviewHeaderId, appraiserId);

                                if (existing_result_summary != null && existing_result_summary.Count > 0)
                                {
                                    await _reviewResultRepository.UpdateSummaryAsync(resultSummary);
                                    no_inserted++;
                                }
                                else
                                {
                                    await _reviewResultRepository.AddSummaryAsync(resultSummary);
                                    no_inserted++;
                                }
                            }
                        }

                    }
                }
            }

            if (no_inserted == no_results)
            {
                return true;
            }
            else
            {
                await _reviewResultRepository.DeleteSummaryAsync(reviewHeaderId);
                return false;
            }
        }

        public async Task<bool> AddReviewResultAsync(ReviewResult reviewResult)
        {
            if (reviewResult == null) { throw new ArgumentNullException(nameof(reviewResult)); }
            var entities = await _reviewResultRepository.GetByAppraiserIdAndMetricId(reviewResult.ReviewHeaderId, reviewResult.AppraiserId, reviewResult.ReviewMetricId);
            if (entities != null && entities.Count > 0)
            {
                return await _reviewResultRepository.UpdateAsync(reviewResult);
            }
            return await _reviewResultRepository.AddAsync(reviewResult);
        }

        public async Task<bool> UpdateReviewResultAsync(ReviewResult reviewResult)
        {
            if (reviewResult == null) { throw new ArgumentNullException(nameof(reviewResult)); }
            return await _reviewResultRepository.UpdateAsync(reviewResult);
        }

        public async Task<bool> AddResultSummaryAsync(ResultSummary resultSummary)
        {
            if (resultSummary == null) { throw new ArgumentNullException(nameof(resultSummary)); }
            var entities = await _reviewResultRepository.GetSummaryByAppraiserIdAndReviewHeaderId(resultSummary.ReviewHeaderId, resultSummary.AppraiserId);
            if (entities != null && entities.Count > 0)
            {
                return await _reviewResultRepository.UpdateSummaryAsync(resultSummary);
            }
            return await _reviewResultRepository.AddSummaryAsync(resultSummary);
        }

        #endregion      
    }
}
