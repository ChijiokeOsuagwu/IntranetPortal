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
    public class BusinessPartnerViewModel : BaseViewModel
    {
        public string BusinessID { get; set; }

        [Display(Name = "Customer Number")]
        public string BusinessNumber { get; set; }
        [Required]
        [Display(Name ="Customer Name")]
        [MaxLength(200, ErrorMessage="Customer Name must not exceed 200 characters!")]
        public string BusinessName { get; set; }

        [Display(Name ="Industry Type")]
        public string BusinessType { get; set; }

        [Display(Name ="Station")]
        public int? BusinessStationID { get; set; }

        [Display(Name = "Station")]
        public string BusinessStationName { get; set; }

        [Display(Name = "Address")]
        [MaxLength(250, ErrorMessage = "Address must not exceed 250 characters!")]
        public string BusinessAddress { get; set; }

        [Display(Name ="City/State")]
        public string State { get; set; }

        [Display(Name ="Country")]
        public string Country { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsAgent { get; set; }

        [Display(Name ="Phone No")]
        public string PhoneNo1 { get; set; }

        [Display(Name ="Alt. Phone No")]
        public string PhoneNo2 { get; set; }

        [Display(Name ="Email")]
        public string Email1 { get; set; }

        [Display(Name ="Alt. Email")]
        public string Email2 { get; set; }

        [Display(Name ="Weblink 1")]
        public string WebLink1 { get; set; }

        [Display(Name ="Weblink 2")]
        public string WebLink2 { get; set; }
        public string ModifiedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string ImagePath { get; set; }


        public string ContactID { get; set; }
        [Display(Name ="Contact First Name")]
        [MaxLength(150, ErrorMessage ="First Name must not exceed 150 characters.")]
        public string ContactFirstName { get; set; }

        [Display(Name = "Contact Surname")]
        [MaxLength(150, ErrorMessage = "Surname must not exceed 150 characters.")]
        public string ContactSurname { get; set; }

        [Display(Name = "Contact Other Names")]
        [MaxLength(150, ErrorMessage = "OtherNames must not exceed 150 characters.")]
        public string OtherNames { get; set; }

        [Display(Name ="Contact Designation")]
        public string Designation { get; set; }

        [Display(Name = "Gender")]
        public string ContactGender { get; set; }

        [Display(Name = "Contact Phone")]
        [MaxLength(50, ErrorMessage = "Contact Phone must not exceed 50 characters.")]
        public string ContactPhone1 { get; set; }

        [Display(Name = "Contact Alt. Phone")]
        [MaxLength(50, ErrorMessage = "Contact Alt Phone No must not exceed 50 characters.")]
        public string ContactPhone2 { get; set; }

        [Display(Name = "Contact Email")]
        [MaxLength(250, ErrorMessage = "Contact Email must not exceed 250 characters.")]
        public string ContactEmail { get; set; }

        public Business ConvertToBusiness()
        {
            return new Business
            {
                BusinessAddress = BusinessAddress,
                BusinessID = BusinessID ?? Guid.NewGuid().ToString(),
                BusinessName = BusinessName,
                BusinessNumber = BusinessNumber,
                BusinessStationID = BusinessStationID,
                BusinessStationName = BusinessStationName,
                BusinessType = BusinessType,
                Country = Country,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                Email1 = Email1,
                Email2 = Email2,
                ImagePath = ImagePath,
                IsAgent = IsAgent,
                IsCustomer = IsCustomer,
                IsSupplier = IsSupplier,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                PhoneNo1 = PhoneNo1,
                PhoneNo2 = PhoneNo2,
                State = State,
                WebLink1 = WebLink1,
                WebLink2 = WebLink2,
            };
        }

        public Person FromModel_RetrieveContactInfo()
        {
            return new Person
            {
                FirstName = ContactFirstName,
                Surname = ContactSurname,
                OtherNames = OtherNames,
                FullName = $"{ContactFirstName} {OtherNames} {ContactSurname}",
                PhoneNo1 = ContactPhone1,
                PhoneNo2 = ContactPhone2,
                Email = ContactEmail,
                PersonID = ContactID,
            };
        }

        public BusinessContact FromModel_RetrieveBusinessContact()
        {
            return new BusinessContact { 
                PersonID = ContactID,
                BusinessID = BusinessID,
                PersonRole = Designation,
            };
        }
    }
}
