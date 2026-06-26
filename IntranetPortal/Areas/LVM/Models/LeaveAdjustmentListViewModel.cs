using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveAdjustmentListViewModel:BaseListViewModel
    {
        public LeaveRequest LeaveRequestDetail { get; set; }
        public List<LeaveAdjustment> LeaveAdjustmentList { get; set; }
    }
}
