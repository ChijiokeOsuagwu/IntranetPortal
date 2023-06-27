using IntranetPortal.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IntranetPortal.Areas.ERM.Models
{
    public class EmployeeProfileViewModel : BaseViewModel
    {
        public string PersonID { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Gender")]
        public string Sex { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNo1 { get; set; }

        [Display(Name = "Alt. Phone Number")]
        public string PhoneNo2 { get; set; }

        [Display(Name = "Personal Email")]
        public string Email { get; set; }

        [Display(Name = "Home Address")]
        public string Address { get; set; }

        public string ImagePath { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }

        [Display(Name = "Birth Day")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Start Up Date")]
        public string StartUpDateFormatted { get; set; }

        [Display(Name = "Job Grade")]
        public string JobGrade { get; set; }

        [Display(Name = "Designation")]
        public string CurrentDesignation { get; set; }

        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; }

        [Display(Name = "Official Email")]
        [DataType(DataType.EmailAddress)]
        public string OfficialEmail { get; set; }

        public string EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Employee No")]
        public string EmployeeNo1 { get; set; }

        [Display(Name = "Custom Code")]
        public string EmployeeNo2 { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [Display(Name = "Place of Engagement")]
        public string PlaceOfEngagement { get; set; }

        [Display(Name = "State of Origin")]
        public string StateOfOrigin { get; set; }

        [Display(Name = "LGA of Origin")]
        public string LgaOfOrigin { get; set; }

        [Display(Name="Region")]
        public string GeoPoliticalRegion { get; set; }

        [Display(Name="Religion")]
        public string Religion { get; set; }

        [Display(Name="Confirmation Date")]
        public string ConfirmationDateFormatted { get; set; }

        [Display(Name="Length Of Service")]
        public string LengthOfServiceFormatted { get; set; }

        [Display(Name="Date Last Promoted")]
        public string DateOfLastPromotionFormatted { get; set; }
    }
}