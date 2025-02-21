using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskItemEvaluation : TaskItem
    {
        public long EvaluationDetailId { get; set; }
        public int EvaluationHeaderId { get; set; }
        public string EvaluatorId { get; set; }
        public DateTime? EvaluationDate { get; set; } 
        public int PercentageCompletion { get; set; }
        public int QualityRating { get; set; }
        public string EvaluatorComments { get; set; }
    }
}
