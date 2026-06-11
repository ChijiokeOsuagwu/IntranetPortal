using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveSubmittedToHrListViewModel:BaseListViewModel
    {
        public int yr { get; set; }
        public int mn { get; set; }
        public int ld { get; set; }
        public int ud { get; set; }
        public List<LeaveSubmission> LeaveSubmissionList { get; set; }
    }
}
