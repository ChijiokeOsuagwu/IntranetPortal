using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class EngReportListViewModel:BaseListViewModel
    {
        public long AssignmentID { get; set; }
        public string AssignmentTitle { get; set; }
        public List<AssignmentEngReport> EngReportList { get; set; }
    }
}
