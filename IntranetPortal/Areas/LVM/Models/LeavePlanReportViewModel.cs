using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System.Collections.Generic;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeavePlanReportViewModel:BaseViewModel
    {
        public int yr { get; set; }
        public int mn { get; set; }
        public int ld { get; set; }
        public int dd { get; set; }
        public int ud { get; set; }
        public string ReportHeaderTitle { get; set; }
        public List<LeavePlan> LeavePlanList { get; set; }
    }
}
