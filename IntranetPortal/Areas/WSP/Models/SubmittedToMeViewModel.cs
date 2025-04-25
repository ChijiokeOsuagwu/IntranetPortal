using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class SubmittedToMeViewModel:BaseViewModel
    {
        public string EmployeeID { get; set; }
        public string FromEmployeeName { get; set; }
        public int SubmittedYear { get; set; }
        public int SubmittedMonth { get; set; }
        public List<FolderSubmission> SubmissionList { get; set; }
    }
}
