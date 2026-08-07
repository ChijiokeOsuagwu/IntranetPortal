using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class AnnualLeaveSummaryReportViewModel:BaseViewModel
    {
        public int yr { get; set; }
        public int ld { get; set; }
        public int dd { get; set; }
        public int ud { get; set; }
        public string sn { get; set; }
        public string ReportHeaderTitle { get; set; }
        public List<AnnualLeaveSummary> AnnualLeaveSummaryList { get; set; }

    }
}
