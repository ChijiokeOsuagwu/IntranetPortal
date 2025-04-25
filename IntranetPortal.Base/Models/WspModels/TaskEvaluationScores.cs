using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskEvaluationScores
    {
        public long TaskEvaluationHeaderId { get; set; }
        public long TaskFolderId { get; set; }
        public string EvaluatorId { get; set; }
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
        public long TotalNumberOfTasks { get; set; }
        public long NoOfCompletedTasks { get; set; }
        public long TotalCompletionScore { get; set; }
        public long TotalQualityScore { get; set; }
        public decimal AverageCompletionScore { get; set; }
        public decimal AverageQualityScore { get; set; }
        public long NoOfUncompletedTasks { get; set; }
        public decimal PercentageCompletion { get; set; }
    }
}
