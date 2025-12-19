using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeavePlan
    {
        public long LeavePlanId { get; set; }
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }
        public int LeaveUnitId { get; set; }
        public string LeaveUnitName { get; set; }
        public int LeaveDepartmentId { get; set; }
        public string LeaveDepartmentName { get; set; }
        public int LeaveLocationId { get; set; }
        public string LeaveLocationName { get; set; }
        public int LeaveYear { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public string LeaveReason { get; set; }
        public string LeavePlanStatus { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public DateTime? ProposedStartDate { get; set; }
        public DateTime? ProposedEndDate { get; set; }
        public DateTime? ProposedResumptionDate { get; set; }
        public int ProposedDurationInDays { get; set; }
        public int ProposedDuration { get; set; }
        public string ProposedDurationType { get; set; }
        public string ProposedDurationDescription { get; set; }
        public DateTime? ApprovedStartDate { get; set; }
        public DateTime? ApprovedEndDate { get; set; }
        public DateTime? ApprovedResumptionDate { get; set; }
        public int ApprovedDurationInDays { get; set; }
        public int ApprovedDuration { get; set; }
        public string ApprovedDurationType { get; set; }
        public string ApprovedDurationDescription { get; set; }
    }
}
