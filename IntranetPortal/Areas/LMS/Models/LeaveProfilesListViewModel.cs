using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeaveProfilesListViewModel:BaseListViewModel
    {
        public List<LeaveProfile> LeaveProfilesList { get; set; }
    }
}
