using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class AnnualLeaveSummary
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNumber { get; set; }
        public string OfficialEmail { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int LeaveYear { get; set; }
        public long NumberOfAnnualLeaveDaysDue { get; set; }
        public long NumberOfDaysUsed { get; set; }
        public long NumberOfDaysAdded { get; set; }
        public long NumberOfDaysDeducted { get; set; }
        public long NumberOfDaysUnused { get; set; }
        public long PreviousYearsBalanceBroughtFoward { get; set; }

    }
}
