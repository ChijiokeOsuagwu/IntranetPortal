using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class ReturnTaskItemViewModel:BaseViewModel
    {
        public long TaskEvaluationReturnID { get; set; }
        [Required]
        public long TaskFolderID { get; set; }
        public string TaskFolderName { get; set; }
        [Required]
        public long TaskItemID { get; set; }
        public long FolderSubmissionID { get; set; }
        public string TaskOwnerID { get; set; }
        [Required]
        [Display(Name = "Reason Type")]
        public string ReasonType { get; set; }
        [Required]
        [Display(Name = "Reason Details")]
        public string ReasonDetails { get; set; }
        [Required]
        public string FromEmployeeName { get; set; }
        public string FromEmployeeID { get; set; }
        public long TaskEvaluationHeaderID { get; set; }
        public long TaskEvaluationDetailID { get; set; }
        [Display(Name = "Quality Rating")]
        public int? QualityRating { get; set; }
        public bool ExemptFromEvaluation { get; set; }

        public TaskEvaluationReturns Convert()
        {
            TaskEvaluationReturns e = new TaskEvaluationReturns();
            e.EvaluationReturnId = TaskEvaluationReturnID;
            e.TaskEvaluationHeaderId = TaskEvaluationHeaderID;
            e.TaskEvaluationDetailId = TaskEvaluationDetailID;
            e.ReasonDetails = ReasonDetails;
            e.ReasonType = ReasonType;
            e.ReturnedBy = FromEmployeeName;
            e.ReturnedDate = DateTime.Now;
            e.TaskFolderId = TaskFolderID;
            e.TaskFolderName = TaskFolderName;
            e.TaskItemId = TaskItemID;
            e.TaskOwnerId = TaskOwnerID;
            return e;
        }
        public ReturnTaskItemViewModel Convert(TaskEvaluationReturns e)
        {
            ReturnTaskItemViewModel model = new ReturnTaskItemViewModel();
            model.TaskEvaluationReturnID = e.EvaluationReturnId;
            model.TaskEvaluationHeaderID = e.TaskEvaluationHeaderId;
            model.TaskEvaluationDetailID = e.TaskEvaluationDetailId;
            model.ReasonDetails = e.ReasonDetails;
            model.ReasonType = e.ReasonType;
            model.FromEmployeeName = e.ReturnedBy;
            model.TaskFolderID = e.TaskFolderId;
            model.TaskFolderName = e.TaskFolderName;
            model.TaskItemID = e.TaskItemId;
            model.TaskOwnerID = e.TaskOwnerId;
            return model;
        }
    }
}
