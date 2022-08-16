using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PartnerServices.Models
{
    public class ContactViewModel:BaseViewModel
    {
        public int BusinessContactID { get; set; }
        public string BusinessID { get; set; }

        [Display(Name ="Customer Name")]
        [Required]
        public string BusinessName { get; set; }

        [Required]
        [Display(Name ="Designation")]
        [MaxLength(100, ErrorMessage ="Designation must not be more than 100 characters.")]
        public string PersonRole { get; set; }
        public string PersonID { get; set; }

        [Display(Name = "Title")]
        [MaxLength(50, ErrorMessage = "Title must not be more than 50 characters.")]
        public string Title { get; set; }

        [Display(Name = "Surname")]
        [MaxLength(100, ErrorMessage = "Surname must not be more than 100 characters.")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(100, ErrorMessage = "First Name must not be more than 100 characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Other Names")]
        [MaxLength(100, ErrorMessage = "Other Names must not be more than 100 characters.")]
        public string OtherNames { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        [Display(Name = "Gender")]
        public string Sex { get; set; }

        [Display(Name = "Phone No.")]
        [MaxLength(30, ErrorMessage = "Phone No must not be more than 30 characters.")]
        public string PhoneNo1 { get; set; }

        [Display(Name = "Alt Phone No.")]
        [MaxLength(30, ErrorMessage = "Alt Phone No must not be more than 30 characters.")]
        public string PhoneNo2 { get; set; }

        [Display(Name = "Email")]
        [MaxLength(250, ErrorMessage = "Email must not be more than 250 characters.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Address")]
        [MaxLength(500, ErrorMessage = "Address must not be more than 500 characters.")]
        public string Address { get; set; }

        [Display(Name = "Marital Status")]
        [MaxLength(50, ErrorMessage = "Marital Status must not be more than 50 characters.")]
        public string MaritalStatus { get; set; }

        [Display(Name = "Birth Day")]
        public int? BirthDay { get; set; }

        [Display(Name = "Birth Month")]
        public int? BirthMonth { get; set; }

        [Display(Name = "Birth Year")]
        public int? BirthYear { get; set; }
        public string DateOfBirth { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
        public string ImagePath { get; set; }

        public Person ConvertToPerson()
        {
            return new Person {
                Address = Address,
                BirthDay = BirthDay,
                BirthMonth = BirthMonth,
                BirthYear = BirthYear,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                DateOfBirth = DateOfBirth,
                Email = Email,
                FirstName = FirstName,
                FullName = $"{Title} {FirstName} {OtherNames} {Surname}",
                ImagePath = ImagePath,
                MaritalStatus = MaritalStatus,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                OtherNames = OtherNames,
                PersonID = PersonID,
                PhoneNo1 = PhoneNo1,
                PhoneNo2 = PhoneNo2,
                Sex = Sex,
                Surname = Surname,
                Title = Title,
            };
        }

        public BusinessContact ConvertToBusinessContact()
        {
            return new BusinessContact
            {
                Address = Address,
                BirthDay = BirthDay,
                BirthMonth = BirthMonth,
                BirthYear = BirthYear,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                DateOfBirth = DateOfBirth,
                Email = Email,
                FirstName = FirstName,
                FullName = $"{Title} {FirstName} {OtherNames} {Surname}",
                ImagePath = ImagePath,
                MaritalStatus = MaritalStatus,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                OtherNames = OtherNames,
                PersonID = PersonID,
                PhoneNo1 = PhoneNo1,
                PhoneNo2 = PhoneNo2,
                Sex = Sex,
                Surname = Surname,
                Title = Title,
                BusinessContactID =BusinessContactID,
                BusinessID = BusinessID,
                PersonRole = PersonRole,
            };
        }

    }
}
