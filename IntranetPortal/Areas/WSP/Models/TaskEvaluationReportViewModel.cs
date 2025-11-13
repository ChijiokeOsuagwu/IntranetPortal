using IntranetPortal.Base.Models.WspModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class TaskEvaluationReportViewModel
    {
        public long id { get; set; }
        public string TaskFolderTitle { get; set; }
        public string EmployeeName { get; set; }
        public List<TaskEvaluationDetail> EvaluationDetailsList { get; set; }
    }
}
