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
        public int LeavePlanStatusId { get; set; }
        public string LeavePlanStatusDescription { get; set; }
        public DateTime? LeavePlanStartDate { get; set; }
        public DateTime? LeavePlanEndDate { get; set; }
        public DateTime? LeavePlanResumptionDate { get; set; }
        public int LeavePlanDurationInDays { get; set; }
        public int LeavePlanDuration { get; set; }
        public int LeavePlanDurationTypeId { get; set; }
        public string LeavePlanDurationTypeDescription { get; set; }
        public string LeavePlanDurationDescription { get; set; }
    }
}
