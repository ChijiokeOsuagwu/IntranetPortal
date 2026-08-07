using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveRequestCompliance
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public long TotalNumberOfStaff { get; set; }
        public long NumberWithLeaveRequests { get; set; }
        public long NumberWithoutLeaveRequests { get; set; }
        public decimal PercentageCompliance { get; set; }
        public string PercentageComplianceFormatted { get; set; }
    }
}
