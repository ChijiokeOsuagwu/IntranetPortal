using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.GlobalSettingsModels
{
    public class LocationGroupMember
    {
        public int LocationGroupMemberId { get; set; }
        public int LocationGroupId { get; set; }
        public string LocationGroupName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }
}
