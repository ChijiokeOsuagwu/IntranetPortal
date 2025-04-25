using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class ReviewResultDetailsViewModel:BaseViewModel
    {
        public long TaskEvaluationHeaderID { get; set; }
        public string md { get; set; }
        public List<TaskEvaluationDetail> EvaluationDetailList { get; set; }
    }
}
