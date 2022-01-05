using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeBasicInfo
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNo1 { get; set; }
        public string EmployeeNo2 { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int? LocationID { get; set; }
        public string LocationName { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int? UnitID { get; set; }
        public string UnitName { get; set; }
        public DateTime? StartUpDate { get; set; }
        public string StartUpDesignation { get; set; }
        public string JobGrade { get; set; }
        public string ReportsTo1_EmployeeID { get; set; }
        public string ReportsTo1_EmployeeName { get; set; }
        public string ReportsTo2_EmployeeID { get; set; }
        public string ReportsTo2_EmployeeName { get; set; }
        public string ReportsTo3_EmployeeID { get; set; }
        public string ReportsTo3_EmployeeName { get; set; }
        public string EmploymentStatus { get; set; }
        public string OfficialEmail { get; set; }
        public string StateOfOrigin { get; set; }
        public string LgaOfOrigin { get; set; }
        public string Religion { get; set; }
        public string GeoPoliticalRegion { get; set; }
        public string EmployeeModifiedBy { get; set; }
        public string EmployeeModifiedDate { get; set; }
        public string EmployeeCreatedBy { get; set; }
        public string EmployeeCreatedDate { get; set; }

    }
}
