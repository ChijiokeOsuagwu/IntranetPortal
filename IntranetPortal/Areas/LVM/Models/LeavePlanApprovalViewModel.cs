using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeavePlanApprovalViewModel:LeavePlanViewModel
    {
        public long LeaveSubmissionId { get; set; }
        public string DeclineReason { get; set; }

    }
}
