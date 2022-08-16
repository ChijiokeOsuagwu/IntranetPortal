using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.GlobalSettingsModels
{
    public class Team
    {
        public string TeamID { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public int? TeamLocationID { get; set; }
        public string TeamLocationName { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
