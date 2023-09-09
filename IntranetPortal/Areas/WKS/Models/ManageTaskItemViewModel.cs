using IntranetPortal.Base.Enums;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ManageTaskItemViewModel:BaseViewModel
    {
        public string Source { get; set; }
        public long? SubmissionID { get; set; }

        public long? Id { get; set; }

        [Required]
        [Display(Name = "Task Description*")]
        [MaxLength(250)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Owner")]
        public string TaskOwnerId { get; set; }

        [Display(Name = "Task Owner")]
        [MaxLength(150)]
        public string TaskOwnerName { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string DeletedBy { get; set; }

        [Required]
        [Display(Name="Task No.*")]
        public string Number { get; set; }

        [Display(Name="More Information")]
        public string Deliverable { get; set; }
        public long? MasterTaskId { get; set; }
        public string MasterTaskDescription { get; set; }

        [Required]
        public int TaskListId { get; set; }
        [Display(Name="Task Folder")]
        public string TaskListName { get; set; }
        public string LinkProjectNumber { get; set; }
        public string LinkProjectTitle { get; set; }
        public string LinkProgramCode { get; set; }
        public string LinkProgramName { get; set; }
        public DateTime? LinkProgramDate { get; set; }
        public string AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime? AssignedTime { get; set; }
        public WorkItemStatus TaskStatus { get; set; }
        public string TaskStatusDescription { get; set; }
        public WorkItemProgressStatus ProgressStatus { get; set; }
        public string ProgressStatusDescription { get; set; }
        public ApprovalStatus TaskApprovalStatus { get; set; }
        public string ApprovalStatusDescription { get; set; }
        public DateTime? TimeApproved { get; set; }
        public string ApprovedBy { get; set; }

        [Required]
        [Display(Name = "Expected Start Date*")]
        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ActualStartTime { get; set; }

        [Required]
        [Display(Name="Expected Due Date*")]
        public DateTime? ExpectedDueTime { get; set; }
        public DateTime? ActualDueTime { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? TimeCancelled { get; set; }
        public string CancelledBy { get; set; }
        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public bool CompletionConfirmed { get; set; }
        public string CompletionConfirmedBy { get; set; }
        public DateTime? CompletionConfirmedTime { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedTime { get; set; }
    }
}
