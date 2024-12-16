using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeavePlanViewModel:BaseViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Display(Name ="Full Name")]
        public string EmployeeFullName { get; set; }

        [Display(Name ="Leave Year")]
        public int LeaveYear { get; set; }

        [Required]
        [Display(Name ="Leave Type")]
        public string LeaveTypeCode { get; set; }

        [Display(Name = "Leave Type")]
        public string LeaveTypeName { get; set; }

        [Display(Name = "Reason for Leave")]
        public string LeaveReason { get; set; }

        [Display(Name = "Leave Status")]
        public string LeaveStatus { get; set; }

        [Required]
        [Display(Name = "Leave Start Date")]
        public DateTime LeaveStartDate { get; set; }

        [Display(Name = "Leave End Date")]
        public DateTime? LeaveEndDate { get; set; }

        [Required]
        [Display(Name = "Leave Duration")]
        public int Duration { get; set; }

        [Required]
        [Display(Name = "Duration Type")]
        public int DurationTypeId { get; set; }

        [Display(Name = "Duration Type")]
        public string DurationTypeDescription { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsPlan { get; set; } = true;
    }
}
