using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.PmsModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IPerformanceService
    {
        #region Performance Year Service Methods
        Task<bool> AddPerformanceYearAsync(PerformanceYear performanceYear);
        Task<bool> DeletePerformanceYearAsync(int performanceYearId);
        Task<bool> EditPerformanceYearAsync(PerformanceYear performanceYear);
        Task<PerformanceYear> GetPerformanceYearAsync(int PerformanceYearId);
        Task<List<PerformanceYear>> GetPerformanceYearsAsync();
        #endregion

        #region Approval Roles Service Methods
        Task<List<ApprovalRole>> GetApprovalRolesAsync();
        Task<ApprovalRole> GetApprovalRoleAsync(int ApprovalRoleId);
        Task<bool> AddApprovalRoleAsync(ApprovalRole approvalRole);
        Task<bool> EditApprovalRoleAsync(ApprovalRole approvalRole);
        Task<bool> DeleteApprovalRoleAsync(int approvalRoleId);
        #endregion

        #region Review Session Service Methods
        Task<List<ReviewSession>> GetReviewSessionsAsync();
        Task<List<ReviewSession>> GetReviewSessionsAsync(int PerformanceYearId);
        Task<ReviewSession> GetReviewSessionAsync(int ReviewSessionId);
        Task<bool> AddReviewSessionAsync(ReviewSession reviewSession);
        Task<bool> EditReviewSessionAsync(ReviewSession reviewSession);
        Task<bool> DeleteReviewSessionAsync(int ReviewSessionId);
        #endregion

        #region Grade Header Service Methods
        Task<List<GradeHeader>> GetGradeHeadersAsync();
        Task<GradeHeader> GetGradeHeaderAsync(int GradeHeaderId);
        Task<bool> AddGradeHeaderAsync(GradeHeader gradeHeader);
        Task<bool> EditGradeHeaderAsync(GradeHeader gradeHeader);
        Task<bool> DeleteGradeHeaderAsync(int gradeHeaderId);
        #endregion

        #region Review Grade Details Service Methods

        Task<List<ReviewGrade>> GetReviewGradesAsync();
        Task<List<ReviewGrade>> GetReviewGradesAsync(int gradeHeaderId);
        Task<ReviewGrade> GetReviewGradeAsync(int ReviewGradeId);
        Task<List<ReviewGrade>> GetPerformanceGradesAsync(int gradeHeaderId);
        Task<List<ReviewGrade>> GetCompetencyGradesAsync(int gradeHeaderId);
        Task<bool> AddReviewGradeAsync(ReviewGrade reviewGrade);
        Task<bool> EditReviewGradeAsync(ReviewGrade reviewGrade);
        Task<bool> DeleteReviewGradeAsync(int reviewGradeId);

        #endregion

        #region Appraisal Grade Service Methods

        Task<List<AppraisalGrade>> GetAppraisalGradesAsync();
        Task<List<AppraisalGrade>> GetAppraisalGradesAsync(int reviewSessionId);
        Task<AppraisalGrade> GetAppraisalGradeAsync(int AppraisalGradeId);
        Task<List<AppraisalGrade>> GetAppraisalPerformanceGradesAsync(int reviewSessionId);
        Task<List<AppraisalGrade>> GetAppraisalCompetencyGradesAsync(int reviewSessionId);
        Task<AppraisalGrade> GetAppraisalGradeAsync(int reviewSessionId, ReviewGradeType gradeType, decimal gradeScore);

        Task<bool> AddAppraisalGradeAsync(AppraisalGrade appraisalGrade);
        Task<bool> CopyAppraisalGradeAsync(string copiedBy, int reviewSessionId, int gradeTemplateId, ReviewGradeType? gradeType = null);
        Task<bool> EditAppraisalGradeAsync(AppraisalGrade appraisalGrade);
        Task<bool> DeleteAppraisalGradeAsync(int appraisalGradeId);

        #endregion

        #region Session Schedule Service Method
        Task<bool> AddSessionScheduleAsync(SessionSchedule sessionSchedule);
        Task<bool> CancelSessionScheduleAsync(int sessionScheduleId, string cancelledBy);
        Task<bool> DeleteSessionScheduleAsync(int sessionScheduleId);

        Task<List<SessionSchedule>> GetSessionSchdulesAsync(int reviewSessionId);
        Task<SessionSchedule> GetSessionScheduleAsync(int sessionScheduleId);
        Task<ReviewSchedule> GetEmployeePerformanceScheduleAsync(int reviewSessionId, string employeeId);
        #endregion

        #region Review Header Service Method
        Task<ReviewHeader> GetReviewHeaderAsync(int reviewHeaderId);
        Task<ReviewHeader> GetReviewHeaderAsync(string appraiseeId, int reviewSessionId);
        Task<List<ReviewHeader>> GetReviewHeadersAsync(int reviewSessionId, int? reviewStageId = null, int? locationId = null, int? unitId = null, string appraiseeName = null);
        Task<bool> AddReviewHeaderAsync(ReviewHeader reviewHeader);
        Task<bool> RollBackReviewHeaderAsync(ReviewHeader reviewHeader);
        Task<bool> UpdatePerformanceGoalAsync(int reviewHeaderId, string performanceGoal, string appraiserId);
        Task<bool> UpdateReviewHeaderStageAsync(int reviewHeaderId, int nextStageId);
        Task<bool> UpdateAppraiseeFlagAsync(int reviewHeaderId, bool isFlagged, string flaggedReason);
        Task<bool> UpdateFeedbackAsync(int reviewHeaderId, string feedbackProblems, string feedbackSolutions);
        Task<bool> AddAppraisalRecommendationAsync(ReviewHeaderRecommendation model);
        Task<bool> UpdatePrincipalAppraiserAsync(int changeTypeId, int reviewSessionId, string newPrincipalAppraiserId, string appraiseeId = null, int? unitId = null);
        #endregion

        #region Review Stage Service Methods
        Task<List<ReviewStage>> GetReviewStagesAsync();
        Task<List<ReviewStage>> GetPreviousReviewStagesAsync(int currentStageId);

        #endregion

        #region Review Metric Service Method
        Task<List<ReviewMetric>> GetReviewMetricsAsync(int reviewHeaderId);
        Task<List<ReviewMetric>> GetKpasAsync(int reviewHeaderId);
        Task<List<ReviewMetric>> GetCompetenciesAsync(int reviewHeaderId);
        Task<ReviewMetric> GetReviewMetricAsync(int reviewMetricId);
        Task<int> GetMetricCountAsync(int reviewHeaderId, ReviewMetricType reviewMetricType);
        Task<bool> AddReviewMetricAsync(ReviewMetric reviewMetric);
        Task<bool> UpdateReviewMetricAsync(ReviewMetric reviewMetric);
        Task<bool> DeleteReviewMetricAsync(int reviewMetricId);

        #endregion

        #region Competency Service Methods

        //====== Competency Service Methods =====//
        Task<List<Competency>> GetFromCompetencyDictionaryAsync();
        Task<Competency> GetFromCompetencyDictionaryByIdAsync(int CompetencyId);
        Task<List<Competency>> GetFromCompetencyDictionaryByCategoryAsync(int CategoryId);
        Task<List<Competency>> GetFromCompetencyDictionaryByLevelAsync(int LevelId);
        Task<List<Competency>> SearchFromCompetencyDictionaryAsync(int CategoryId, int LevelId);
        Task<bool> AddCompetencyAsync(Competency competency);
        Task<bool> UpdateCompetencyAsync(Competency competency);
        Task<bool> DeleteCompetencyAsync(int competencyId);



        //====== Competency Category Service Methods =========//
        Task<List<CompetencyCategory>> GetCompetencyCategoriesAsync();
        Task<CompetencyCategory> GetCompetencyCategoryAsync(int competencyCategoryId);
        Task<bool> AddCompetencyCategoryAsync(string competencyCategoryDescription);
        Task<bool> UpdateCompetencyCategoryAsync(int competencyCategoryId, string competencyCategoryDescription);
        Task<bool> DeleteCompetencyCategoryAsync(int competencyCategoryId);

        //====== Competency Level Service Methods =========//
        Task<List<CompetencyLevel>> GetCompetencyLevelsAsync();
        Task<CompetencyLevel> GetCompetencyLevelAsync(int competencyLevelId);
        Task<bool> AddCompetencyLevelAsync(string competencyLevelDescription);
        Task<bool> UpdateCompetencyLevelAsync(int competencyLevelId, string competencyLevelDescription);
        Task<bool> DeleteCompetencyLevelAsync(int competencyLevelId);

        #endregion

        #region Review CDG Service Methods
        Task<List<ReviewCDG>> SearchReviewCdgsAsync(int reviewSessionId, int? locationId = null, int? departmentId = null, int? unitId = null, string employeeName = "");
        Task<List<ReviewCDG>> GetReviewCdgsAsync(int reviewHeaderId);
        Task<ReviewCDG> GetReviewCdgAsync(int reviewCdgId);
        Task<bool> AddReviewCdgAsync(ReviewCDG reviewCdg);
        Task<bool> UpdateReviewCdgAsync(ReviewCDG reviewCdg);
        Task<bool> DeleteReviewCdgAsync(int reviewCdgId);

        #endregion

        #region PMS Utility Service Methods
        Task<MoveToNextStageModel> ValidateMoveRequestAsync(int reviewHeaderId, string appraiserId = null);
        Task<List<PmsActivityHistory>> GetPmsActivityHistory(int reviewHeaderId);
        Task<bool> AddPmsActivityHistoryAsync(PmsActivityHistory pmsActivityHistory);

        #endregion

        #region Performance Settings Service Methods
        Task<List<ReviewType>> GetReviewTypesAsync();
        Task<List<AppraisalRecommendation>> GetAppraisalRecommendationsAsync();
        #endregion

        #region Review Submission Service Methods
        Task<bool> AddReviewSubmissionAsync(ReviewSubmission reviewSubmission);
        Task<bool> UpdateReviewSubmissionAsync(int reviewSubmissionId);
        Task<bool> DeleteReviewSubmissionAsync(int reviewSubmissionId);
        Task<ReviewSubmission> GetReviewSubmissionByIdAsync(int reviewSubmissionId);
        Task<List<ReviewSubmission>> GetReviewSubmissionsByApproverIdAsync(string reviewerId, int? reviewSessionId = null);
        Task<List<ReviewSubmission>> GetReviewSubmissionsByReviewHeaderIdAsync(int reviewHeaderId, int? submissionPurposeId = null, string submittedToEmployeeId = null, int? submittedToEmployeeRoleId = null);
        #endregion

        #region Review Message Service Methods
        Task<List<ReviewMessage>> GetReviewMessagesAsync(int reviewHeaderId);
        Task<ReviewMessage> GetReviewMessageAsync(int reviewMessageId);
        Task<bool> AddReviewMessageAsync(ReviewMessage reviewMessage);
        Task<bool> UpdateReviewMessageAsync(ReviewMessage reviewMessage);
        Task<bool> DeleteReviewMessageAsync(int reviewMessageId);
        #endregion

        #region Review Approval Service Methods
        Task<bool> ReturnContractToAppraisee(int nextStageId, int reviewSubmissionId, ReviewMessage reviewMessage = null);
        Task<bool> ApproveContractToAppraisee(ReviewApproval reviewApproval, int? reviewSubmissionId);
        Task<bool> AcceptContractByAppraisee(int reviewHeaderId);
        Task<bool> AcceptEvaluationByAppraisee(int reviewHeaderId);
        Task<List<ReviewApproval>> GetReviewApprovalsAsync(int reviewHeaderId, int? approvalTypeId = null, int? approverRoleId = null);
        Task<List<ReviewApproval>> GetReviewApprovalsApprovedAsync(int reviewHeaderId, int? approvalTypeId = null, int? approverRoleId = null);
        Task<bool> DeleteAllApprovals(int reviewHeaderId);
        Task<bool> DeleteApprovalByType(int reviewHeaderId, ReviewApprovalType reviewApprovalType);
        #endregion

        #region Review Result Service Methods
        //===================== Review Result Read Service Methods =========================================//
        Task<List<EvaluationHeader>> GetEvaluationHeadersAsync(int reviewHeaderId);
        Task<int> GetEvaluatedMetricCountAsync(int reviewHeaderId, string appraiserId, ReviewMetricType reviewMetricType);
        Task<List<ReviewResult>> GetInitialReviewResultKpasAsync(int reviewHeaderId, string appraiserId);
        Task<List<ReviewResult>> GetInitialReviewResultAsync(int reviewHeaderId, string appraiserId, int reviewMetricId);

        Task<List<ReviewResult>> GetInitialReviewResultCmpsAsync(int reviewHeaderId, string appraiserId);

        Task<List<ReviewResult>> GetReviewResultByAppraiserIdAndReviewHeaderIdAsync(int reviewHeaderId, string appraiserId);
        Task<List<ReviewResult>> GetReviewResultByAppraiserIdAndReviewMetricIdAsync(int reviewHeaderId, string appraiserId, int reviewMetricId);
        Task<List<ReviewResult>> GetReviewResultByAppraiserIdAndReviewMetricTypeIdAsync(int reviewHeaderId, string appraiserId, int? reviewMetricTypeId = null);

        Task<List<string>> GetAppraisersAsync(int reviewHeaderId);
        Task<List<AppraiserDetail>> GetAppraiserDetailsAsync(int reviewHeaderId);
        Task<ScoreSummary> GetScoreSummaryAsync(int reviewHeaderId, string appraiserId);
        Task<List<ResultSummary>> GetResultSummaryForReportsAsync(string reportToId, int reviewSessionId, string appraiseeId = null);

        Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAsync(int reviewSessionId);
        Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndDepartmentCodeAsync(int reviewSessionId, int departmentId);
        Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndUnitCodeAsync(int reviewSessionId, int unitId);
        Task<List<ResultSummary>> GetResultSummaryByReviewSessionIdAndAppraiseeNameAsync(int reviewSessionId, string appraiseeName);


        //===================== Review Result Write Service Methods =========================================//
        Task<bool> AddReviewResultAsync(ReviewResult reviewResult);
        Task<bool> UpdateReviewResultAsync(ReviewResult reviewResult);
        Task<bool> DeleteEvaluationsAsync(int reviewHeaderId, bool includeSelfEvaluation = false);

        //==================== Result Summary Service Methods ================================================//
        Task<bool> AddResultSummaryAsync(ResultSummary resultSummary);
        Task<bool> UploadResults(int reviewHeaderId);
        Task<bool> DeleteResultSummaryAsync(int reviewHeaderId, bool includeSelfEvaluationResult = false);

        //=================== Result Details Service Methods ==================================================//
        Task<List<ResultDetail>> GetPrincipalResultDetailAsync(int reviewSessionId, int? locationId = null, int? departmentId = null, int? unitId = null);
        Task<List<ResultDetail>> GetRejectedPrincipalResultDetailAsync(int reviewSessionId, int? locationId = null);
        #endregion

        #region Participation Service Methods
        Task<List<Employee>> GetAppraisalNonParticipants(int ReviewSessionId, int? LocationId = null, int? UnitId = null, int? DepartmentId = null);
        Task<long> GetAppraisalParticipantsCount(int ReviewSessionId, int? LocationId = null, int? DepartmentId = null, int? UnitId = null);
        Task<List<ParticipationSummary>> GetAppraisalParticipationSummary(int ReviewSessionId, int? LocationId = null, int? DepartmentId = null, int? UnitId = null);
        #endregion
    }

}
