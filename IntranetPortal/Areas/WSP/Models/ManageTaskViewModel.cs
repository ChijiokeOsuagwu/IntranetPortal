using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class ManageTaskViewModel:BaseViewModel
    {
        public long Id { get; set; }
        [Required]
        [MaxLength(20)]
        [Display(Name = "Task No.")]
        public string Number { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name ="Task Description")]
        public string Description { get; set; }

        [MaxLength(500)]
        [Display(Name = "More Information")]
        public string MoreInformation { get; set; }

        public long? MasterTaskId { get; set; }
        public string MasterTaskDescription { get; set; }

        public long? WorkFolderId { get; set; }
        public string WorkFolderName { get; set; }
        public string LinkProjectNumber { get; set; }
        public string LinkProgramCode { get; set; }
        public DateTime? LinkProgramDate { get; set; }

        [Required]
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
        public string AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime? AssignedTime { get; set; }
        public TaskItemStage Stage { get; set; }
        public int StageId { get; set; }
        public string StageDescription { get; set; }
        public WorkItemProgressStatus ProgressStatus { get; set; }
        public int ProgressStatusId { get; set; }
        public string ProgressStatusDescription { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public int ApprovalStatusId { get; set; }
        public string ApprovalStatusDescription { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledTime { get; set; }
        public string CancelledBy { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedTime { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ExpectedDueTime { get; set; }
        public DateTime? ActualDueTime { get; set; }
        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public bool CompletionConfirmed { get; set; }
        public string CompletionConfirmedBy { get; set; }
        public DateTime? CompletionConfirmedTime { get; set; }
        public bool IsCarriedOver { get; set; }
        public bool IsLocked { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public long? AssignmentId { get; set; }
        public long? FolderSubmissionId { get; set; }
        public TaskItem Convert()
        {
            return new TaskItem
            {
                ActualDueTime = ActualDueTime,
                ActualStartTime = ActualStartTime,
                ApprovalStatus = ApprovalStatus,
                ApprovalStatusDescription = ApprovalStatusDescription,
                ApprovalStatusId = ApprovalStatusId,
                ApprovedBy = ApprovedBy,
                ApprovedTime = ApprovedTime,
                AssignedTime = AssignedTime,
                AssignedToId = AssignedToId,
                AssignedToName = AssignedToName,
                AssignmentId = AssignmentId,
                CancelledBy = CancelledBy,
                CancelledTime = CancelledTime,
                ClosedBy = ClosedBy,
                ClosedTime = ClosedTime,
                CompletionConfirmed = CompletionConfirmed,
                CompletionConfirmedBy = CompletionConfirmedBy,
                CompletionConfirmedTime = CompletionConfirmedTime,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                DepartmentId = DepartmentId,
                DepartmentName = DepartmentName,
                Description = Description,
                ExpectedDueTime = ExpectedDueTime,
                ExpectedStartTime = ExpectedStartTime,
                Id = Id,
                
                IsClosed = IsClosed,
                IsCancelled = IsCancelled,
                IsCarriedOver = IsCarriedOver,
                IsLocked = IsLocked,
                LastModifiedBy = LastModifiedBy,
                LastModifiedTime = LastModifiedTime,
                LinkProgramCode = LinkProgramCode,
                LinkProgramDate = LinkProgramDate,
                LinkProjectNumber = LinkProjectNumber,
                LocationId = LocationId,
                LocationName = LocationName,
                MasterTaskDescription = MasterTaskDescription,
                MasterTaskId = MasterTaskId,
                MoreInformation = MoreInformation,
                Number = Number,
                ProgressStatus = ProgressStatus,
                ProgressStatusDescription = ProgressStatusDescription,
                ProgressStatusId = ProgressStatusId,
                Stage = Stage,
                StageDescription = StageDescription,
                StageId = StageId,
                TaskOwnerId = TaskOwnerId,
                TaskOwnerName = TaskOwnerName,
                UnitId = UnitId,
                UnitName = UnitName,
                WorkFolderId = WorkFolderId,
                WorkFolderName = WorkFolderName,
                
            };
        }
    
        public ManageTaskViewModel Convert(TaskItem task)
        {
            return new ManageTaskViewModel
            {
                ActualDueTime = task.ActualDueTime,
                ActualStartTime = task.ActualStartTime,
                ApprovalStatus = task.ApprovalStatus,
                ApprovalStatusDescription = task.ApprovalStatusDescription,
                ApprovalStatusId = task.ApprovalStatusId,
                ApprovedBy = task.ApprovedBy,
                ApprovedTime = task.ApprovedTime,
                AssignedTime = task.AssignedTime,
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedToName,
                AssignmentId = task.AssignmentId,
                CancelledBy = task.CancelledBy,
                CancelledTime = task.CancelledTime,
                ClosedBy = task.ClosedBy,
                ClosedTime = task.ClosedTime,
                CompletionConfirmed = task.CompletionConfirmed,
                CompletionConfirmedBy = task.CompletionConfirmedBy,
                CompletionConfirmedTime = task.CompletionConfirmedTime,
                CreatedBy = task.CreatedBy,
                CreatedTime = task.CreatedTime,
                DepartmentId = task.DepartmentId,
                DepartmentName = task.DepartmentName,
                Description = task.Description,
                ExpectedDueTime = task.ExpectedDueTime,
                ExpectedStartTime = task.ExpectedStartTime,
                Id = task.Id,
                IsClosed = task.IsClosed,
                IsCancelled = task.IsCancelled,
                IsCarriedOver = task.IsCarriedOver,
                IsLocked = task.IsLocked,
                LastModifiedBy = task.LastModifiedBy,
                LastModifiedTime = task.LastModifiedTime,
                LinkProgramCode = task.LinkProgramCode,
                LinkProgramDate = task.LinkProgramDate,
                LinkProjectNumber = task.LinkProjectNumber,
                LocationId = task.LocationId,
                LocationName = task.LocationName,
                MasterTaskDescription = task.MasterTaskDescription,
                MasterTaskId = task.MasterTaskId,
                MoreInformation = task.MoreInformation,
                Number = task.Number,
                ProgressStatus = task.ProgressStatus,
                ProgressStatusDescription = task.ProgressStatusDescription,
                ProgressStatusId = task.ProgressStatusId,
                Stage = task.Stage,
                StageDescription = task.StageDescription,
                StageId = task.StageId,
                TaskOwnerId = task.TaskOwnerId,
                TaskOwnerName = task.TaskOwnerName,
                UnitId = task.UnitId,
                UnitName = task.UnitName,
                WorkFolderId = task.WorkFolderId,
                WorkFolderName = task.WorkFolderName,
            };
        }
    }
}
