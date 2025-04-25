using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskEvaluationHeader
    {
        public long Id { get; set; }
        public long TaskFolderId { get; set; }
        public string TaskFolderName { get; set; }
        public string EvaluatorId { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorDesignation { get; set; }
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
        public string TaskOwnerDesignation { get; set; }
        public int TaskOwnerUnitId { get; set; }
        public string TaskOwnerUnitName { get; set; }
        public int TaskOwnerDeptId { get; set; }
        public string TaskOwnerDeptName { get; set; }
        public int TaskOwnerLocationId { get; set; }
        public string TaskOwnerLocationName { get; set; }
        public long TotalNumberOfTasks { get; set; }
    }
}
