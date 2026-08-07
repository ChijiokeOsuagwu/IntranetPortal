using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class MyTeamsLeaveRequestsListViewModel:BaseListViewModel
    {
        public int yr { get; set; }
        public int mm { get; set; }
        public string ed { get; set; }
        public int? st { get; set; }
        public string td { get; set; }
        public List<LeaveRequest> LeaveRequestList { get; set; }
    }
}
