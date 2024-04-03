using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ShowFullResultViewModel : BaseViewModel
    {
        public EvaluationResultViewModel EvaluationSummaryResult { get; set; }
        public EvaluationListViewModel KpaFullResult { get; set; }
        public EvaluationListViewModel CmpFullResult { get; set; }

    }

}
