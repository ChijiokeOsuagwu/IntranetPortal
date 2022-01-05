using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.EmployeeRecords.Models
{
    public class EmployeeBasicInfoViewModel : BaseViewModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNo1 { get; set; }

        [Display(Name = "Custom Code")]
        [MaxLength(10, ErrorMessage = "Custom Code must not exceed 10 characters.")]
        public string EmployeeNo2 { get; set; }

        [Display(Name = "Company")]
        [Required(ErrorMessage = "Company is required.")]
        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Location is required.")]
        public int? LocationID { get; set; }
        public string LocationName { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [Display(Name = "Unit")]
        public int? UnitID { get; set; }
        public string UnitName { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="Start Date is required.")]
        public DateTime? StartUpDate { get; set; }

        [Display(Name = "Start Designation")]
        public string StartUpDesignation { get; set; }

        [Display(Name = "Job Grade")]
        public string JobGrade { get; set; }

        [Display(Name = "Reports To")]
        public string ReportsTo1_EmployeeID { get; set; }
        public string ReportsTo1_EmployeeName { get; set; }

        [Display(Name = "Alt. Reports To")]
        public string ReportsTo2_EmployeeID { get; set; }
        public string ReportsTo2_EmployeeName { get; set; }

        [Display(Name = "Alt. Reports To")]
        public string ReportsTo3_EmployeeID { get; set; }
        public string ReportsTo3_EmployeeName { get; set; }

        [Display(Name = "Employment Status")]
        [Required(ErrorMessage ="Employment Status is required")]
        public string EmploymentStatus { get; set; }

        [Display(Name = "Official Email")]
        [DataType(DataType.EmailAddress)]
        public string OfficialEmail { get; set; }

        [Display(Name = "State of Origin")]
        public string StateOfOrigin { get; set; }

        [Display(Name = "LGA of Origin")]
        public string LgaOfOrigin { get; set; }

        [Display(Name = "Religion")]
        public string Religion { get; set; }

        [Display(Name = "Geo Political Region")]
        public string GeoPoliticalRegion { get; set; }

        public string EmployeeModifiedBy { get; set; }
        public string EmployeeModifiedDate { get; set; }
        public string EmployeeCreatedBy { get; set; }
        public string EmployeeCreatedDate { get; set; }

        public EmployeeBasicInfo ConvertToEmployeeBasicInfo() => new EmployeeBasicInfo
        {
            CompanyCode = CompanyCode,
            CompanyName = CompanyName,
            DepartmentID = DepartmentID,
            DepartmentName = DepartmentName,
            EmployeeCreatedBy = EmployeeCreatedBy,
            EmployeeCreatedDate = EmployeeCreatedDate,
            EmployeeID = EmployeeID,
            EmployeeModifiedBy = EmployeeModifiedBy,
            EmployeeModifiedDate = EmployeeModifiedDate,
            EmployeeName = EmployeeName,
            EmployeeNo1 = EmployeeNo1,
            EmployeeNo2 = EmployeeNo2,
            EmploymentStatus = EmploymentStatus,
            GeoPoliticalRegion = GeoPoliticalRegion,
            JobGrade = JobGrade,
            LgaOfOrigin = LgaOfOrigin,
            LocationID = LocationID,
            LocationName = LocationName,
            OfficialEmail = OfficialEmail,
            Religion = Religion,
            ReportsTo1_EmployeeID = ReportsTo1_EmployeeID,
            ReportsTo1_EmployeeName = ReportsTo1_EmployeeName,
            ReportsTo2_EmployeeID = ReportsTo2_EmployeeID,
            ReportsTo2_EmployeeName = ReportsTo2_EmployeeName,
            ReportsTo3_EmployeeID = ReportsTo3_EmployeeID,
            ReportsTo3_EmployeeName = ReportsTo3_EmployeeName,
            StateOfOrigin = StateOfOrigin,
            StartUpDate = StartUpDate,
            StartUpDesignation = StartUpDesignation,
            UnitID = UnitID,
            UnitName = UnitName,
        };
    }
}
