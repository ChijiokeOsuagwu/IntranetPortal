using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class MyLeaveRecordsViewModel:BaseViewModel
    {
        public int yr { get; set; }
        public string nm { get; set; }
        public string ei { get; set; }
        public List<LeavePlan> LeavePlanList { get; set; }
        public List<LeaveRequest> LeaveRequestList { get; set; }
        public LeaveBalances CurrentLeaveBalances { get; set; }

    }
}
