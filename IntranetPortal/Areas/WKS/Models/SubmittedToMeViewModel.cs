using IntranetPortal.Base.Models.WksModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class SubmittedToMeViewModel
    {
        public string EmployeeID { get; set; }
        public string FromEmployeeName { get; set; }
        public int SubmittedYear { get; set; }
        public int SubmittedMonth { get; set; }
        public List<TaskSubmission> TaskSubmissionList { get; set; }

    }
}
