using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeReportLine
    {
        public int ReportingLineID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string ReportsToEmployeeID { get; set; }
        public string ReportsToEmployeeName { get; set; }
        public string ReportsToEmployeeRole { get; set; }
        public DateTime? ReportStartDate { get; set; }
        public DateTime? ReportEndDate { get; set; }
        public string TeamID { get; set; }
        public string TeamName { get; set; }
        public int? UnitID { get; set; }
        public string UnitName { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
    }
}
