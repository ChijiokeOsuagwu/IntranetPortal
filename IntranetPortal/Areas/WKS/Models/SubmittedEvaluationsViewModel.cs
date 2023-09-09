using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class SubmittedEvaluationsViewModel:BaseViewModel
    {
        public int TaskListID { get; set; }
        public long TaskSubmissionID { get; set; }
        public string SubmittedToEmployeeID { get; set; }
        public string SubmittedToEmployeeName { get; set; }
        public int TaskEvaluationHeaderID { get; set; }
        public string EvaluatorID { get; set; }
        public string TaskOwnerID { get; set; }
        public WorkItemSubmissionType PurposeOfSubmission { get; set; }
        public List<TaskItemEvaluation> TaskItemEvaluations { get; set; }
    }
}
