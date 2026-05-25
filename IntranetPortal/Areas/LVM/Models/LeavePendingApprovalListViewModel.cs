using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeavePendingApprovalListViewModel:BaseListViewModel
    {
        public int yr { get; set; }
        public string nm { get; set; }
        public string ei { get; set; }
        public List<LeaveSubmission> LeaveSubmissionList { get; set; }
    }
}
