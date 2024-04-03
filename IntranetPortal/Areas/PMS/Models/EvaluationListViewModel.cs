using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class EvaluationListViewModel : BaseViewModel
    {
        public int ReviewHeaderID { get; set; }
        public int ReviewSessionID { get; set; }
        public string AppraiseeID { get; set; }
        public int SubmissionID { get; set; }
        public string AppraiserID { get; set; }
        public string PrimaryAppraiserID { get; set; }
        public List<ReviewResult> ReviewResultList { get; set; }
    }
}
