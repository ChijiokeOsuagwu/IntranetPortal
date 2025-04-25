using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskEvaluationSummary
    {
        public long Id { get; set; }
        public long TaskEvaluationHeaderId { get; set; }
        public long TaskFolderId { get; set; }
        public string TaskFolderName { get; set; }
        public string TaskEvaluatorId { get; set; }
        public string TaskEvaluatorName { get; set; }
        public string TaskEvaluatorDesignation { get; set; }
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
        public int TaskOwnerUnitId { get; set; }
        public int TaskOwnerDepartmentId { get; set; }
        public int TaskOwnerLocationId { get; set; }
        public string TaskOwnerUnitName { get; set; }
        public string TaskOwnerDepartmentName { get; set; }
        public string TaskOwnerLocationName { get; set; }
        public long TotalNoOfTasks { get; set; }
        public long TotalNoOfCompletedTasks { get; set; }
        public long TotalNoOfUncompletedTasks { get; set; }
        public long TotalCompletionScore { get; set; }
        public long TotalQualityScore { get; set; }
        public decimal AverageCompletionScore { get; set; }
        public decimal AverageQualityScore { get; set; }
        public DateTime? EvaluationDate { get; set; }
    }
}
