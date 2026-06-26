using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveAdjustment
    {
        public long LeaveAdjustmentId { get; set; }
        public long LeaveRequestId { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }
        public int LeaveYear { get; set; }
        public string AdjustmentType { get; set; }
        public string AdjustmentJustification { get; set; }
        public int NumberOfDays { get; set; }
        public string DurationDescription { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public string AdjustmentAddedBy { get; set; }
        public int LeaveUnitId { get; set; }
        public string LeaveUnitName { get; set; }
        public int LeaveDepartmentId { get; set; }
        public string LeaveDepartmentName { get; set; }
        public int LeaveLocationId { get; set; }
        public string LeaveLocationName { get; set; }
    }
}
