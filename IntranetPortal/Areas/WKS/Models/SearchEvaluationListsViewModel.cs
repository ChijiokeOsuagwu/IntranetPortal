using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class SearchEvaluationListsViewModel:BaseViewModel
    {
        public int? yy { get; set; }
        public int? mm { get; set; }
        public string sn { get; set; }
        public string tp { get; set; }
        public string TaskOwnerID { get; set; }
        public List<TaskList> TaskLists { get; set; }
        public List<TaskEvaluationHeader> EvaluationHeaderList { get; set; }
    }
}
