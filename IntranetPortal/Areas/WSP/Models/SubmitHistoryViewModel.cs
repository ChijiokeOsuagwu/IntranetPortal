using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class SubmitHistoryViewModel:BaseViewModel
    {
        public long FolderID { get; set; }
        public string FolderName { get; set; }
        public List<FolderSubmission> FolderSumissions { get; set; }
    }
}
