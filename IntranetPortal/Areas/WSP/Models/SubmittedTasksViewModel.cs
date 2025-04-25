using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;

namespace IntranetPortal.Areas.WSP.Models
{
    public class SubmittedTasksViewModel:BaseViewModel
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
        public WorkItemSubmissionType PurposeOfSubmission { get; set; }
        public List<TaskItem> TaskItems { get; set; }
    }
}
