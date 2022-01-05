using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.EmployeeRecords.Models
{
    public class PersonInfoViewModel : BaseViewModel
    {
        public string PersonID { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [Display(Name = "Surname")]
        [MaxLength(50, ErrorMessage = "Surname must not exceed 50 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "First Name must not exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Other Names is required.")]
        [Display(Name = "Other Names")]
        [MaxLength(50, ErrorMessage = "Other Names must not exceed 50 characters.")]
        public string OtherNames { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [Display(Name = "Gender")]
        public string Sex { get; set; }

        [Display(Name = "Phone Number")]
        [MaxLength(50, ErrorMessage = "Phone Number must not exceed 50 characters.")]
        public string PhoneNo1 { get; set; }

        [Display(Name = "Alt. Phone Number")]
        [MaxLength(50, ErrorMessage = "Alt. Phone Number must not exceed 50 characters.")]
        public string PhoneNo2 { get; set; }

        [Display(Name = "Email Address")]
        [MaxLength(200, ErrorMessage = "Email Address must not exceed 200 characters.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Address")]
        [MaxLength(250, ErrorMessage = "Address must not exceed 250 characters.")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Display(Name ="Upload Image")]
        public IFormFile Image { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }
        public int? BirthDay { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthYear { get; set; }
        public string DateOfBirth { get; set; }

        public Person ConvertToPerson() => new Person
        {
            Address = Address,
            Email = Email,
            FirstName = FirstName,
            FullName = $"{FirstName} {OtherNames} {Surname}",
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
        };

    }
}
