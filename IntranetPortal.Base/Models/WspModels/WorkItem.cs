using IntranetPortal.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class WorkItem
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? MasterWorkItemID { get; set; }
        public string MasterWorkItemTitle { get; set; }
        public int? MasterProjectID { get; set; }
        public string MasterProjectTitle { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string AssignedToID { get; set; }
        public string AssignedToName { get; set; }
        public DateTime? AssignedTime { get; set; }
        public WorkItemProgressStatus ProgressStatus { get; set; }
        public string ProgressStatusDescription { get; set; }
        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ExpectedDueTime { get; set; }
        public long ExpectedDurationInMinutes { get; set; }
        public long ExpectedDurationInHours { get; set; }
        public int ExpectedDurationInDays { get; set; }
        public long TotalExpectedDurationInMinutes { get; set; }
        public long TotalActualDurationInMinutes { get; set; }
        public long ActualDurationInHours { get; set; }
        public int ActualDurationInDays { get; set; }
        public long ActualDurationInMinutes { get; set; }
        public WorkItemPriority Priority { get; set; }
        public WorkItemType ItemType { get; set; }
        public long EstimatedCostForeignCurrency { get; set; }
        public long ActualCostForeignCurrency { get; set; }
        public string ForeignCurrencyCode { get; set; }
        public long EstimatedCostLocalCurrency { get; set; }
        public long ActualCostLocalCurrency { get; set; }
        public int PercentageCompleted { get; set; }
        public string Deliverables { get; set; }
        public int? UnitID { get; set; }
        public string UnitName { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int? LocationID { get; set; }
        public string LocationName { get; set; }
        public bool IsAtRisk { get; set; }
        public string Instructions { get; set; }
        public int FolderID { get; set; }
        public string FolderTitle { get; set; }
        public int WorkspaceID { get; set; }
        public string WorkspaceTitle { get; set; }
        public bool HasTeam { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string DeletedBy { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedTime { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
