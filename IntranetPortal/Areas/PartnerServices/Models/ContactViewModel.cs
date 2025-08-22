using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.PartnerServicesModels;
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
        public long ContactID { get; set; }
        public string BusinessID { get; set; }

        [Display(Name ="Customer Name")]
        [Required]
        public string BusinessName { get; set; }

        [Required]
        [Display(Name ="Designation")]
        [MaxLength(100, ErrorMessage ="Designation must not be more than 100 characters.")]
        public string ContactDesignation { get; set; }

        [Required]
        [Display(Name = "Contact Name")]
        [MaxLength(150, ErrorMessage = "Name must not be more than 100 characters.")]
        public string ContactName { get; set; }

        [Display(Name = "Gender")]
        public string ContactSex { get; set; }

        [Display(Name = "Phone No.")]
        [MaxLength(30, ErrorMessage = "Phone No must not be more than 30 characters.")]
        public string ContactPhone1 { get; set; }

        [Display(Name = "Alt Phone No.")]
        [MaxLength(30, ErrorMessage = "Alt Phone No must not be more than 30 characters.")]
        public string ContactPhone2 { get; set; }

        [Display(Name = "Email")]
        [MaxLength(250, ErrorMessage = "Email must not be more than 250 characters.")]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail1 { get; set; }

        [Display(Name = "Alt. Email")]
        [MaxLength(250, ErrorMessage = "Alt. Email must not be more than 250 characters.")]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail2 { get; set; }

        [Display(Name = "Address")]
        [MaxLength(500, ErrorMessage = "Address must not be more than 500 characters.")]
        public string ContactAddress { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ImagePath { get; set; }

        public BusinessContact ConvertToPerson()
        {
            return new BusinessContact {
                ContactAddress = ContactAddress,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                ContactEmail1 = ContactEmail1,
                ContactEmail2 = ContactEmail2,
                ContactName = ContactName,
                Designation = ContactDesignation,
                ContactPhone1 = ContactPhone1,
                ContactPhone2 = ContactPhone2,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                Sex = ContactSex,
            };
        }

        public BusinessContact ConvertToBusinessContact()
        {
            return new BusinessContact
            {
                ContactAddress = ContactAddress,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                ContactEmail1 = ContactEmail1,
                ContactEmail2 = ContactEmail2,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                ContactPhone1 = ContactPhone1,
                ContactPhone2 = ContactPhone2,
                Sex = ContactSex,
                ContactID = ContactID,
                BusinessID = BusinessID,
                Designation = ContactDesignation,
                BusinessName = BusinessName,
                ContactName = ContactName,
            };
        }
    }
}
