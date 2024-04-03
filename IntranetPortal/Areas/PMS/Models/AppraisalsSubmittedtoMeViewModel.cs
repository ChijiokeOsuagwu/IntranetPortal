using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class AppraisalsSubmittedtoMeViewModel : BaseViewModel
    {
        public string AppraiserID { get; set; }
        public string AppraiserName { get; set; }
        public int? id { get; set; }
        public List<ReviewSubmission> ReviewSubmissionList { get; set; }
    }
}
