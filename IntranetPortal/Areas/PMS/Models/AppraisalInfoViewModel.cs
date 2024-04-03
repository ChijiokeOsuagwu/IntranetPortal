using IntranetPortal.Base.Models.PmsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class AppraisalInfoViewModel
    {
        public int ReviewSubmissionID { get; set; }
        public ReviewHeader AppraisalReviewHeader { get; set; }
        public List<ReviewMetric> CompetencyList { get; set; }
        public List<ReviewMetric> KpaList { get; set; }
        public List<ReviewCDG> CdgList { get; set; }
        public string SourcePage { get; set; }
    }
}
