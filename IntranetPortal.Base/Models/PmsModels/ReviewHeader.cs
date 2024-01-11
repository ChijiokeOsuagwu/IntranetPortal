using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ReviewHeader
    {
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public string AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }
        public string AppraiseeDesignation { get; set; }
        public int ReviewStageId { get; set; }
        public string ReviewStageDescription { get; set; }
        public string ReviewStageActionDescription { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }
        public string PrimaryAppraiserId { get; set; }
        public string PrimaryAppraiserName { get; set; }
        public string PrimaryAppraiserDesignation { get; set; }
        public string FeedbackProblems { get; set; }
        public string FeedbackSolutions { get; set; }
        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
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
        public bool? ContractIsAccepted { get; set; }
        public DateTime? TimeContractAccepted { get; set; }
        public bool? EvaluationIsAccepted { get; set; }
        public DateTime? TimeEvaluationAccepted { get; set; }
        public bool IsFlagged { get; set; }
        public string FlaggedBy { get; set; }
        public DateTime? FlaggedTime { get; set; }
        public string PerformanceGoal { get; set; }
    }
}
