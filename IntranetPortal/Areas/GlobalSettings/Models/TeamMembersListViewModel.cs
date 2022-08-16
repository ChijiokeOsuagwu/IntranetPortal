using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class TeamMembersListViewModel : BaseListViewModel
    {
        public string TeamID { get; set; }
        public List<TeamMember> TeamMembersList { get; set; }
    }
}
