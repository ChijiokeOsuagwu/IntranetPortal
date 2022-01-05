using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.GlobalSettingsModels
{
    public class Location
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string LocationType { get; set; }
        public string LocationDescription { get; set; }
        public string LocationHeadID1 { get; set; }
        public string LocationHeadID2 { get; set; }
        public string LocationCountry { get; set; }
        public string LocationState { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
