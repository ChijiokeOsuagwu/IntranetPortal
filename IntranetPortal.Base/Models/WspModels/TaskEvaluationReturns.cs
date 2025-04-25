using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskEvaluationReturns
    {
        public long EvaluationReturnId { get; set; }
        public long TaskEvaluationHeaderId { get; set; }
        public long TaskEvaluationDetailId { get; set; }
        public long TaskItemId { get; set; }
        public string TaskItemNumber { get; set; }
        public string TaskItemDescription { get; set; }
        public long TaskFolderId { get; set; }
        public string TaskFolderName { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string ReturnedBy { get; set; }
        public string ReasonType { get; set; }
        public string ReasonDetails { get; set; }
        public bool ExemptFromEvaluation { get; set; }
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
        public int? TaskOwnerUnitId { get; set; }
        public string TaskOwnerUnitName { get; set; }
        public int? TaskOwnerDepartmentId { get; set; }
        public string TaskOwnerDepartmentName { get; set; }
        public int? TaskOwnerLocationId { get; set; }
        public string TaskOwnerLocationName { get; set; }
    }
}
