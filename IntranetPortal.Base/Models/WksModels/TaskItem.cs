using IntranetPortal.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WksModels
{
    public class TaskItem
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Deliverable { get; set; }
        public long? MasterTaskId { get; set; }
        public string MasterTaskDescription { get; set; }
        public int TaskListId { get; set; }
        public string TaskListName { get; set; }
        public string LinkProjectNumber { get; set; }
        public string LinkProgramCode { get; set; }
        public DateTime? LinkProgramDate { get; set; }
        public string TaskOwnerId { get; set; }
        public string TaskOwnerName { get; set; }
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
        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
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
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
    }
}
