using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeaveApprovalListViewModel:BaseListViewModel
    {
        public long LeaveID { get; set; }
        public int LeaveYear { get; set; }
        public string SourcePage { get; set; }
        public List<LeaveApproval> LeaveApprovalList { get; set; }
    }
}
