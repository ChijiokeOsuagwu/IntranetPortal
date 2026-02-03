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
        public string ei { get; set; }
        public string sn { get; set; }
        public int? yy { get; set; }
        public int? mm { get; set; }
        public List<FolderSubmission> SubmissionList { get; set; }
    }
}
