using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class CreateEmployeeViewModel : BaseViewModel
    {
        public string PersonID { get; set; }

        [Display(Name="Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [Display(Name = "Last Name*")]
        [MaxLength(100, ErrorMessage = "Surname must not exceed 50 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name*")]
        [MaxLength(100, ErrorMessage = "First Name must not exceed 50 characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Other Names")]
        [MaxLength(100, ErrorMessage = "Other Names must not exceed 50 characters.")]
        public string OtherNames { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [Display(Name = "Gender*")]
        public string Sex { get; set; }

        [Display(Name = "Phone Number")]
        [MaxLength(50, ErrorMessage = "Phone Number must not exceed 50 characters.")]
        public string PhoneNo1 { get; set; }

        [Display(Name = "Alt. Phone Number")]
        [MaxLength(50, ErrorMessage = "Alt. Phone Number must not exceed 50 characters.")]
        public string PhoneNo2 { get; set; }

        [Display(Name = "Personal Email")]
        [MaxLength(250, ErrorMessage = "Personal Email Address must not exceed 250 characters.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Home Address")]
        [MaxLength(250, ErrorMessage = "Address must not exceed 250 characters.")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile Image { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }

        [Range(1, 31)]
        public int? BirthDay { get; set; }

        [Range(1,12)]
        public int? BirthMonth { get; set; }

        public int? BirthYear { get; set; }

        [Display(Name = "Birth Day")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Start Up Date")]
        [DataType(DataType.Date)]
        public DateTime? StartUpDate { get; set; }

        [Display(Name = "Job Grade")]
        public string JobGrade { get; set; }

        [Display(Name = "Start Up Designation")]
        [MaxLength(100, ErrorMessage = "Start Up Designation must not exceed 50 characters.")]
        public string StartUpDesignation { get; set; }

        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; }

        [Display(Name = "Official Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(250, ErrorMessage = "Official Email must not exceed 250 characters.")]
        public string OfficialEmail { get; set; }

        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNo1 { get; set; }

        [Display(Name = "Custom Code")]
        [MaxLength(20, ErrorMessage = "Custom Code must not exceed 20 characters.")]
        public string EmployeeNo2 { get; set; }

        [Display(Name = "Company*")]
        [Required(ErrorMessage = "Company is required.")]
        public string CompanyCode { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Location*")]
        [Required(ErrorMessage = "Location is required.")]
        public int? LocationID { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [Display(Name = "Unit*")]
        public int? UnitID { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [Display(Name = "Place of Engagement")]
        [MaxLength(100, ErrorMessage = "Place of Engagement must not exceed 50 characters.")]
        public string PlaceOfEngagement { get; set; }

        [Display(Name = "State of Origin")]
        public string StateOfOrigin { get; set; }

        [Display(Name = "LGA of Origin")]
        [MaxLength(100, ErrorMessage = "LGA of Origin must not exceed 50 characters.")]
        public string LgaOfOrigin { get; set; }

        [Display(Name="Date of Last Promotion")]
        [DataType(DataType.Date)]
        public DateTime? DateOfLastPromotion { get; set; }

        [Display(Name = "Confirmation Date")]
        [DataType(DataType.Date)]
        public DateTime? ConfirmationDate { get; set; }

        [Display(Name="Current Designation")]
        [MaxLength(100, ErrorMessage = "Current Designation must not exceed 250 characters.")]
        public string CurrentDesignation { get; set; }

        [Display(Name = "Years of Experience")]
        public int? YearsOfExperience { get; set; }

        [Display(Name = "Religion")]
        public string Religion { get; set; }

        [Display(Name="Geo-Political Region")]
        public string GeoPoliticalRegion { get; set; }
        internal string ImagePath { get; set; }

        public Person ConvertToPerson() => new Person
        {
            Address = Address,
            Email = Email,
            FirstName = FirstName,
            FullName = $"{Title} {FirstName} {OtherNames} {Surname}",
            ModifiedTime = $"{DateTime.UtcNow.ToLongDateString()} {DateTime.UtcNow.ToLongTimeString()}",
            PersonID = PersonID,
            PhoneNo1 = PhoneNo1,
            PhoneNo2 = PhoneNo2,
            OtherNames = OtherNames,
            Sex = Sex,
            Surname = Surname,
            MaritalStatus = MaritalStatus,
            DateOfBirth = DateOfBirth,
            BirthDay = BirthDay,
            BirthMonth = BirthMonth,
            BirthYear = BirthYear,
            Title = Title,
        };

        public Employee ConvertToEmployee()
        {
            return new Employee
            {
                EmployeeID = EmployeeID ?? PersonID,
                FullName = $"{Title} {FirstName} {OtherNames} {Surname}",
                EmployeeNo1 = EmployeeNo1,
                EmployeeNo2 = EmployeeNo2,
                CompanyID = CompanyCode,
                CompanyName = CompanyName,
                LocationID = LocationID,
                DepartmentID = DepartmentID,
                UnitID = UnitID,
                JobGrade = JobGrade,
                StartUpDate = StartUpDate,
                StartUpDesignation = StartUpDesignation,
                EmploymentStatus = EmploymentStatus,
                LgaOfOrigin = LgaOfOrigin,
                StateOfOrigin = StateOfOrigin,
                Email = Email,
                Address = Address,
                MaritalStatus = MaritalStatus,
                OfficialEmail = OfficialEmail,
                PlaceOfEngagement = PlaceOfEngagement,
                ConfirmationDate = ConfirmationDate,
                CurrentDesignation = CurrentDesignation,
                DateOfLastPromotion = DateOfLastPromotion,
                YearsOfExperience = YearsOfExperience,
                Religion = Religion,
                BirthDay = BirthDay,
                BirthMonth = BirthMonth,
                BirthYear = BirthYear,
                DateOfBirth = DateOfBirth,
                FirstName = FirstName,
                GeoPoliticalRegion = GeoPoliticalRegion,
                ImagePath = ImagePath,
                OtherNames = OtherNames,
                PersonID = PersonID  ?? EmployeeID,
                PhoneNo1 = PhoneNo1,
                PhoneNo2 = PhoneNo2,
                Sex = Sex,
                Surname = Surname,
                Title = Title,
                UnitName = UnitName,
            };
        }
    }
}
