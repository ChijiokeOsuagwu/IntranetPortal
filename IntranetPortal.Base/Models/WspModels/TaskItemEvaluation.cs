using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskItemEvaluation : TaskItem
    {
        public long EvaluationDetailId { get; set; }
        public long EvaluationHeaderId { get; set; }
        public string EvaluatorId { get; set; }
        public string EvaluatorName { get; set; }
        public DateTime? EvaluationDate { get; set; } 
        public long CompletionScore { get; set; }
        public long QualityScore { get; set; }
        public string EvaluatorComments { get; set; }
    }
}
