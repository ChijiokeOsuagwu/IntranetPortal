using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class EvaluateTaskItemViewModel:BaseViewModel
    {
        public long TaskEvaluationDetailID { get; set; }

        [Required]
        public int TaskEvaluationHeaderID { get; set; }

        [Required]
        public int TaskListID { get; set; }

        [Required]
        public long TaskItemID { get; set; }

        [Display(Name="Task No:")]
        public string TaskItemNo { get; set; }

        [Display(Name="Task Description")]
        public string TaskItemDescription { get; set; }

        [Display(Name="More Information")]
        public string TaskItemDeliverable { get; set; }

        [Required]
        [Display(Name="% Completion*")]
        public int PercentageCompletion { get; set; }

        [Required]
        [Display(Name = "Quality Rating*")]
        public int QualityRating { get; set; }

        [Display(Name="Comment")]
        [MaxLength(500)]
        public string EvaluatorsComment { get; set; }
        public string Source { get; set; }

        [Display(Name="Task Owner")]
        public string TaskOwnerName { get; set; }
        public string TaskOwnerID { get; set; }
        public string EvaluatorID { get; set; }
        public string EvaluatorName { get; set; }

        public TaskEvaluationDetail ConvertToTaskEvaluationDetail()
        {
            return new TaskEvaluationDetail
            {
                EvaluatorsComment = EvaluatorsComment,
                PercentageCompletion = PercentageCompletion,
                QualityRating = QualityRating,
                TaskEvaluationHeaderId = TaskEvaluationHeaderID,
                TaskEvaluationDetailId = TaskEvaluationDetailID,
                TaskItemDeliverable = TaskItemDeliverable,
                TaskItemDescription = TaskItemDescription,
                TaskItemId = TaskItemID,
                TaskListId = TaskListID,
                TaskOwnerName = TaskOwnerName,
                EvaluatorName = EvaluatorName,
            };
        }
   }
}
