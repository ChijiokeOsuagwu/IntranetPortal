using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ShowSelectedResultViewModel : BaseViewModel
    {
        public int id { get; set; }
        public string ad { get; set; }
        public EvaluationResultViewModel EvaluationSummaryResult { get; set; }
        public EvaluationListViewModel KpaFullResult { get; set; }
        public EvaluationListViewModel CmpFullResult { get; set; }
        public ReviewHeader ReviewHeaderInfo { get; set; }
        public List<ReviewCDG> ReviewCDGs { get; set; }
    }

}
