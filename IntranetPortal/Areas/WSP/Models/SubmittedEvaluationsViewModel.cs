using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class SubmittedEvaluationsViewModel:BaseViewModel
    {
        public long FolderID { get; set; }
        public string FolderName { get; set; }
        public long FolderSubmissionID { get; set; }
        public string FolderOwnerID { get; set; }
        public string FolderOwnerName { get; set; }
        public string FolderOwnerDesignation { get; set; }
        public string FolderOwnerUnitName { get; set; }
        public string FolderOwnerLocationName { get; set; }

        public string SubmittedToEmployeeID { get; set; }
        public string SubmittedToEmployeeName { get; set; }
        public long TaskEvaluationHeaderID { get; set; }
        public string EvaluatorID { get; set; }
        public string TaskOwnerID { get; set; }
        public WorkItemSubmissionType PurposeOfSubmission { get; set; }
        public List<TaskItemEvaluation> TaskItemEvaluations { get; set; }
    }
}
