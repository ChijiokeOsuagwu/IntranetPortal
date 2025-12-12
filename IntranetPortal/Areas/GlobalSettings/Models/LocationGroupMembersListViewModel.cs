using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class LocationGroupMembersListViewModel:BaseListViewModel
    {
        public int LocationGroupId { get; set; }
        public int LocationGroupName { get; set; }
        public List<LocationGroupMember> LocationGroupMembersList { get; set; }
    }
}
