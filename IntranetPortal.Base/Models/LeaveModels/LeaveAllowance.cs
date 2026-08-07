using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveAllowance
    {
        public long LeaveAllowanceId { get; set;}
        public long? LeaveRequestId { get; set; }
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }
        public int LeaveYear { get; set; }
        public int PaymentYear { get; set; }
        public int PaymentMonth { get; set; }
        public DateTime RequestedTime { get; set; }
        public bool IsApproved { get; set; }
        public int LeaveUnitId { get; set; }
        public string LeaveUnitName { get; set; }
        public int LeaveDepartmentId { get; set; }
        public string LeaveDepartmentName { get; set; }
        public int LeaveLocationId { get; set; }
        public string LeaveLocationName { get; set; }
        public DateTime RecordedTime { get; set; }
        public string RecordedBy { get; set; }
    }
}
