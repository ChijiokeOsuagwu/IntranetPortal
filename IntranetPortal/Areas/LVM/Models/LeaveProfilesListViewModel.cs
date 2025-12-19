using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveProfilesListViewModel:BaseListViewModel
    {
        public List<LeaveProfile> LeaveProfilesList { get; set; }
    }
}
