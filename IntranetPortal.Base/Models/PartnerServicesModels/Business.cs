using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PartnerServicesModels
{
    public class Business
    {
        public string BusinessID { get; set; }
        public string BusinessNumber { get; set; }
        public string BusinessName { get; set; }
        public string BusinessType { get; set; }
        public int? BusinessStationID { get; set; }
        public string BusinessStationName { get; set; }
        public string BusinessAddress { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsAgent { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string WebLink1 { get; set; }
        public string WebLink2 { get; set; }
        public string ModifiedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string ImagePath { get; set; }
    }
}
