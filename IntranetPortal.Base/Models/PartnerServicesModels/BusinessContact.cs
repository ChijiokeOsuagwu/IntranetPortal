using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PartnerServicesModels
{
    public class BusinessContact
    {
        public long ContactID { get; set; }
        public string ContactName { get; set; }
        public string Sex { get; set; }
        public string Designation { get; set; }
        public string ContactPhone1 { get; set; }
        public string ContactPhone2 { get; set; }
        public string ContactEmail1 { get; set; }
        public string ContactEmail2 { get; set; }
        public string ContactAddress { get; set; }
        public string BusinessID { get; set; }
        public string BusinessName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
