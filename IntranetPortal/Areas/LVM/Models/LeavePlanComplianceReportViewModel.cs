using DocumentFormat.OpenXml.Office2010.ExcelAc;
using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System.Collections.Generic;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeavePlanComplianceReportViewModel:BaseViewModel
    {
        public int yr { get; set; }
        public string pm {  get; set; }
        public string ReportHeaderTitle { get; set; }
        public List<LeavePlanCompliance> LeavePlanComplianceList { get; set; }
    }
}
