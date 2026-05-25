using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveActivitiesViewModel:BaseViewModel
    {
        public long? LeavePlanId { get; set; }
        public long? LeaveRequestId { get; set; }
        public List<LeaveActivityLog> LeaveActivityList { get; set; }
    }
}
