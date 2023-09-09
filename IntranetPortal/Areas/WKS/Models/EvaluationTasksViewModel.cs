using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class EvaluationTasksViewModel:BaseViewModel
    {
        public int EvaluationHeaderID { get; set; }
        public string md { get; set; }
        public List<TaskEvaluationDetail> EvaluationDetailList { get; set; }
    }
}
