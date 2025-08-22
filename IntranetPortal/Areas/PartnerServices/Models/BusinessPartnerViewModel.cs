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

        [Display(Name = "Number")]
        public string BusinessNumber { get; set; }
        [Required]
        [Display(Name ="Name")]
        [MaxLength(200, ErrorMessage="Name must not exceed 150 characters!")]
        public string BusinessName { get; set; }

        [Display(Name ="Type")]
        public string BusinessType { get; set; }

        [Display(Name = "Type")]
        public int? BusinessTypeID { get; set; }

        [Display(Name = "Sector")]
        public int? IndustrySectorID { get; set; }

        [Display(Name = "Sector")]
        public string IndustrySector { get; set; }


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
        public DateTime? ModifiedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string ImagePath { get; set; }


        public long ContactID { get; set; }
        [Display(Name ="Contact Name")]
        [MaxLength(150, ErrorMessage ="Name must not exceed 150 characters.")]
        public string ContactName { get; set; }

        [Display(Name ="Contact Designation")]
        public string ContactDesignation { get; set; }

        [Display(Name = "Gender")]
        public string ContactSex { get; set; }

        [Display(Name = "Phone")]
        [MaxLength(50, ErrorMessage = "Contact Phone must not exceed 50 characters.")]
        public string ContactPhone1 { get; set; }

        [Display(Name = "Alt. Phone")]
        [MaxLength(50, ErrorMessage = "Contact Alt Phone No must not exceed 50 characters.")]
        public string ContactPhone2 { get; set; }

        [Display(Name = "Contact Email")]
        [MaxLength(150, ErrorMessage = "Contact Email must not exceed 250 characters.")]
        public string ContactEmail1 { get; set; }

        [Display(Name = "Contact Alt. Email")]
        [MaxLength(150, ErrorMessage = "Contact Alternate Email must not exceed 250 characters.")]
        public string ContactEmail2 { get; set; }


        public Business ConvertToBusiness()
        {
            return new Business
            {
                BusinessAddress = BusinessAddress == null ? string.Empty : BusinessAddress.ToUpper(),
                BusinessID = BusinessID ?? Guid.NewGuid().ToString(),
                BusinessName = BusinessName.ToUpper(),
                BusinessNumber = BusinessNumber,
                BusinessStationId = BusinessStationID,
                BusinessStationName = BusinessStationName == null ? string.Empty : BusinessStationName.ToUpper(),
                BusinessTypeId = BusinessTypeID,
                BusinessType = BusinessType == null ? string.Empty : BusinessType.ToUpper(),
                IndustrySectorId = IndustrySectorID,
                IndustrySector = IndustrySector == null ? string.Empty : IndustrySector.ToUpper(),
                Country = Country == null ? string.Empty : Country.ToUpper(),
                CreatedBy = CreatedBy == null ? string.Empty : CreatedBy.ToUpper(),
                CreatedTime = CreatedTime,
                Email1 = Email1,
                Email2 = Email2,
                ImagePath = ImagePath,
                IsAgent = IsAgent,
                IsCustomer = IsCustomer,
                IsSupplier = IsSupplier,
                ModifiedBy = ModifiedBy == null ? string.Empty : ModifiedBy.ToUpper(),
                ModifiedTime = ModifiedTime,
                PhoneNo1 = PhoneNo1,
                PhoneNo2 = PhoneNo2,
                State = State == null ? string.Empty : State.ToUpper(),
                WebLink1 = WebLink1,
                WebLink2 = WebLink2,
            };
        }

        public BusinessContact FromModel_RetrieveBusinessContact()
        {
            return new BusinessContact { 
                ContactID = ContactID,
                BusinessID = BusinessID,
                ContactName = ContactName,
                Designation = ContactDesignation,
                Sex = ContactSex,
                ContactPhone1 = ContactPhone1,
                ContactPhone2 = ContactPhone2,
                ContactEmail1 = ContactEmail1,
                ContactEmail2 = ContactEmail2,
            };
        }
    }
}
