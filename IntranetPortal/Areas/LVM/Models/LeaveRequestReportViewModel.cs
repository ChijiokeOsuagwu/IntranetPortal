using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System.Collections.Generic;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveRequestReportViewModel:BaseViewModel
    {
        public int yr { get; set; }
        public int mn { get; set; }
        public int ld { get; set; }
        public int dd { get; set; }
        public int ud { get; set; }
        public string ReportHeaderTitle { get; set; }
        public List<LeaveRequest> LeaveRequestList { get; set; }

    }
}
