using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class MyReviewResultsViewModel:BaseViewModel
    {
        public string id { get; set; }
        public string sn { get; set; }
        public DateTime? sd { get; set; }
        public DateTime? ed { get; set; }
        public List<TaskEvaluationSummary> EvaluationSummaryList { get; set; }

    }
}
