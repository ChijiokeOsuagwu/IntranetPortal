using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class DirectReportEvaluationsViewModel : BaseViewModel
    {
        public string sd { get; set; }
        public int id { get; set; }
        public List<ResultSummary> ReportsResultSummaryList { get; set; }
    }

}
