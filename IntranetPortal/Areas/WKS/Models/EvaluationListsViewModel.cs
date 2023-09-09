using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class EvaluationListsViewModel:BaseViewModel
    {
        public string TaskOwnerID { get; set; }
        public int? yy { get; set; }
        public int? mm { get; set; }
        public string md { get; set; }
        public List<TaskEvaluationHeader> EvaluationHeaderList { get; set; }
    }
}
