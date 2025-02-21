using IntranetPortal.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskItem
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string MoreInformation { get; set; }
        public long? MasterTaskId { get; set; }
        public string MasterTaskDescription { get; set; }
        public long? WorkFolderId { get; set; }
        public string WorkFolderName { get; set; }
        public string LinkProjectNumber { get; set; }
        public string LinkProgramCode { get; set; }
        public DateTime? LinkProgramDate { get; set; }
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
    }
}
