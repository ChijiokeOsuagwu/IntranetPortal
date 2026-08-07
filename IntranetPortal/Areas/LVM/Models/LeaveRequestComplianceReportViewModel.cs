using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System.Collections.Generic;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveRequestComplianceReportViewModel:BaseViewModel
    {
        public int yr { get; set; }
        public string pm { get; set; }
        public string ReportHeaderTitle { get; set; }
        public List<LeaveRequestCompliance> LeaveRequestComplianceList { get; set; }
    }
}
