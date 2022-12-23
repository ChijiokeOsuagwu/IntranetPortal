using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class EmployeeReportLineViewModel : BaseViewModel
    {
        public int? ReportingLineID { get; set; }

        [Required]
        public string EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        public string ReportsToEmployeeID { get; set; }

        [Required]
        [Display(Name = "Reports To:")]
        public string ReportsToEmployeeName { get; set; }

        [Display(Name = "Team Role")]
        public string ReportsToEmployeeRole { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Starting From:")]
        public DateTime? ReportStartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ending On:")]
        public DateTime? ReportEndDate { get; set; }

        [Display(Name = "Team (optional):")]
        public string TeamID { get; set; }

        [Display(Name="Team")]
        public string TeamName { get; set; }

        [Display(Name="Unit")]
        public int? UnitID { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }

        public EmployeeReportLine ConvertToEmployeeReportLine()
        {
            return new EmployeeReportLine
            {
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                DepartmentID = DepartmentID == null ? (int?)null : DepartmentID.Value,
                DepartmentName = DepartmentName,
                EmployeeID = EmployeeID,
                EmployeeName = EmployeeName,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                ReportEndDate = ReportEndDate == null ? (DateTime?)null : ReportEndDate.Value,
                ReportingLineID = ReportingLineID == null ? 0 : ReportingLineID.Value,
                ReportStartDate = ReportStartDate == null ? (DateTime?)null : ReportStartDate.Value,
                ReportsToEmployeeID = ReportsToEmployeeID,
                ReportsToEmployeeName = ReportsToEmployeeName,
                ReportsToEmployeeRole = ReportsToEmployeeRole,
                TeamID = TeamID,
                TeamName = TeamName,
                UnitID = UnitID == null ? (int?)null : UnitID.Value,
                UnitName = UnitName,
            };
        }
    }
}
