using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveProfileDetailsListViewModel:BaseListViewModel
    {
        public int LeaveProfileId { get; set; }
        public string LeaveProfileName { get; set; }
        public List<LeaveProfileDetail> LeaveProfileDetailList { get; set; }

    }
}
