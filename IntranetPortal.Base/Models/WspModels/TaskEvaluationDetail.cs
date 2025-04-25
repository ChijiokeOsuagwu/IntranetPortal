using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskEvaluationDetail
    {
        public long TaskEvaluationDetailId { get; set; }
        public long TaskEvaluationHeaderId { get; set; }
        public long TaskFolderId { get; set; }
        public string TaskFolderName { get; set; }
        public long TaskItemId { get; set; }
        public string TaskItemNo { get; set; }
        public string TaskItemDescription { get; set; }
        public string TaskItemMoreInfo { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public long CompletionScore { get; set; }
        public long QualityScore { get; set; }
        public string EvaluatorsComment { get; set; }
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
        public string TaskEvaluatorId { get; set; }
        public string TaskEvaluatorName { get; set; }
        public int TaskOwnerUnitId { get; set; }
        public string TaskOwnerUnitName { get; set; }
        public int TaskOwnerDeptId { get; set; }
        public string TaskOwnerDeptName { get; set; }
        public int TaskOwnerLocationId { get; set; }
        public string TaskOwnerLocationName { get; set; }
    }
}
