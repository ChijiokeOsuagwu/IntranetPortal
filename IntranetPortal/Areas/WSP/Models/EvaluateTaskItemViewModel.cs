using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class EvaluateTaskItemViewModel:BaseViewModel
    {
        public long TaskEvaluationDetailID { get; set; }

        [Required]
        public long TaskEvaluationHeaderID { get; set; }

        [Required]
        public long? TaskFolderID { get; set; }

        [Required]
        public long TaskItemID { get; set; }

        [Display(Name="Task No:")]
        public string TaskItemNo { get; set; }

        [Display(Name="Task Description")]
        public string TaskItemDescription { get; set; }

        [Display(Name="More Information")]
        public string TaskItemMoreInfo { get; set; }

        [Required]
        public long CompletionScore { get; set; }

        [Required]
        [Display(Name = "Quality Rating*")]
        public long QualityScore { get; set; }

        [Display(Name="Comment")]
        [MaxLength(500)]
        public string EvaluatorsComment { get; set; }

        [Display(Name="Task Owner")]
        public string TaskOwnerName { get; set; }
        public string TaskOwnerID { get; set; }
        public int? TaskOwnerUnitID { get; set; }
        public string TaskOwnerUnitName { get; set; }
        public int? TaskOwnerDepartmentID { get; set; }
        public string TaskOwnerDepartmentName { get; set; }
        public int? TaskOwnerLocationID { get; set; }
        public string TaskOwnerLocationName { get; set; }
        public string TaskEvaluatorID { get; set; }
        public string TaskEvaluatorName { get; set; }

        public TaskEvaluationDetail Convert()
        {
            return new TaskEvaluationDetail
            {
                EvaluatorsComment = EvaluatorsComment,
                CompletionScore = CompletionScore,
                QualityScore = QualityScore,
                TaskEvaluationHeaderId = TaskEvaluationHeaderID,
                TaskEvaluationDetailId = TaskEvaluationDetailID,
                TaskItemMoreInfo = TaskItemMoreInfo,
                TaskItemDescription = TaskItemDescription,
                TaskItemId = TaskItemID,
                TaskFolderId = TaskFolderID ?? 0,
                TaskOwnerId = TaskOwnerID,
                TaskOwnerName = TaskOwnerName,
                TaskOwnerUnitId = TaskOwnerUnitID ?? 0,
                TaskOwnerUnitName = TaskOwnerUnitName,
                TaskOwnerDeptId = TaskOwnerDepartmentID ?? 0,
                TaskOwnerDeptName = TaskOwnerDepartmentName,
                TaskOwnerLocationId = TaskOwnerLocationID ?? 0,
                TaskOwnerLocationName = TaskOwnerLocationName,
                TaskEvaluatorId = TaskEvaluatorID,
                TaskEvaluatorName = TaskEvaluatorName,
            };
        }

        public EvaluateTaskItemViewModel Convert(TaskEvaluationDetail evaluationDetail)
        {
            return new EvaluateTaskItemViewModel
            {
                EvaluatorsComment = evaluationDetail.EvaluatorsComment,
                CompletionScore = evaluationDetail.CompletionScore,
                QualityScore = evaluationDetail.QualityScore,
                TaskEvaluationHeaderID = evaluationDetail.TaskEvaluationHeaderId,
                TaskEvaluationDetailID = evaluationDetail.TaskEvaluationDetailId,
                TaskItemMoreInfo = evaluationDetail.TaskItemMoreInfo,
                TaskItemDescription = evaluationDetail.TaskItemDescription,
                TaskItemID = evaluationDetail.TaskItemId,
                TaskFolderID = evaluationDetail.TaskFolderId,
                TaskOwnerID = evaluationDetail.TaskOwnerId,
                TaskOwnerName = evaluationDetail.TaskOwnerName,
                TaskOwnerUnitID = evaluationDetail.TaskOwnerUnitId,
                TaskOwnerUnitName = evaluationDetail.TaskOwnerUnitName,
                TaskOwnerDepartmentID = evaluationDetail.TaskOwnerDeptId,
                TaskOwnerDepartmentName = evaluationDetail.TaskOwnerDeptName,
                TaskOwnerLocationID = evaluationDetail.TaskOwnerLocationId,
                TaskOwnerLocationName = evaluationDetail.TaskOwnerLocationName,
                TaskEvaluatorID = evaluationDetail.TaskEvaluatorId,
                TaskEvaluatorName = evaluationDetail.TaskEvaluatorName,
            };
        }
    }
}
