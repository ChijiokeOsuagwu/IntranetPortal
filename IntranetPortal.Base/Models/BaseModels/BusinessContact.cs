using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public class BusinessContact : Person
    {
        public int BusinessContactID { get; set; }
        public string BusinessID { get; set; }
        public string PersonRole { get; set; }
        public string BusinessName { get; set; }
        public string BusinessNo { get; set; }
        public string StationID { get; set; }
        public string StationName { get; set; }
    }
}
