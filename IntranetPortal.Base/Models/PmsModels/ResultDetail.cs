using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ResultDetail
    {
        public int ResultSummaryId { get; set; }
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }
        public string AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }
        public string AppraiserId { get; set; }
        public string AppraiserName { get; set; }
        public string AppraiserDesignation { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public decimal KpaScoreTotal { get; set; }
        public decimal KpaScoreObtained { get; set; }
        public decimal CompetencyScoreTotal { get; set; }
        public decimal CompetencyScoreObtained { get; set; }
        public decimal CombinedScoreTotal { get; set; }
        public decimal CombinedScoreObtained { get; set; }
        public int ScoreRank { get; set; }
        public string ScoreRankDescription { get; set; }
        public string PerformanceRating { get; set; }
        public string AppraiserTypeDescription { get; set; }
        public string AppraiserRoleDescription { get; set; }
        public string EmployeeNo { get; set; }
        public bool IsMain { get; set; }
        public string AppraiseeDesignation { get; set; }
        public int? PrimaryAppraiserId { get; set; }
        public string PrimaryAppraiserName { get; set; }
        public string PrimaryAppraiserDesignation { get; set; }
        public string FeedbackProblems { get; set; }
        public string FeedbackSolutions { get; set; }
        public string LineManagerComments { get; set; }
        public string LineManagerName { get; set; }
        public string LineManagerRecommendation { get; set; }
        public string UnitHeadComments { get; set; }
        public string UnitHeadName { get; set; }
        public string UnitHeadRecommendation { get; set; }
        public string DepartmentHeadComments { get; set; }
        public string DepartmentHeadName { get; set; }
        public string DepartmentHeadRecommendation { get; set; }
        public string HrComments { get; set; }
        public string HrName { get; set; }
        public string HrRecommendation { get; set; }
        public string ManagementComments { get; set; }
        public string ManagementName { get; set; }
        public string ManagementDecision { get; set; }
        public bool IsFlagged { get; set; }
        public string FlaggedBy { get; set; }
        public DateTime? FlaggedTime { get; set; }
        public string PerformanceGoal { get; set; }
    }
}
