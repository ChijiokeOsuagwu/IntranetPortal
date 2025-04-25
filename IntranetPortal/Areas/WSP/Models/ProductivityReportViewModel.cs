using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class ProductivityReportViewModel:BaseViewModel
    {
        public string id { get; set; }
        public string sn { get; set; }
        public int? ld { get; set; }
        public string LocationName { get; set; }
        public int? dd { get; set; }
        public string DepartmentName { get; set; }
        public int? ud { get; set; }
        public string UnitName { get; set; }
        public DateTime? sd { get; set; }
        public DateTime? ed { get; set; }
        public List<TaskEvaluationSummary> EvaluationSummaryList { get; set; }
        public List<TaskEvaluationScores> EvaluationScoresList { get; set; }
    }
}
