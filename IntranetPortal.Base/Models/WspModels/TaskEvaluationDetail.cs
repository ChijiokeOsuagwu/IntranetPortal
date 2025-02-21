using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskEvaluationDetail
    {
        public long TaskEvaluationDetailId { get; set; }
        public int TaskEvaluationHeaderId { get; set; }
        public int TaskListId { get; set; }
        public string TaskListName { get; set; }
        public long TaskItemId { get; set; }
        public string TaskItemNo { get; set; }
        public string TaskItemDescription { get; set; }
        public string TaskItemDeliverable { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public int PercentageCompletion { get; set; }
        public int QualityRating { get; set; }
        public string EvaluatorsComment { get; set; }
        public string TaskOwnerName { get; set; }
        public string EvaluatorName { get; set; }
    }
}
