using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class PmsEnquiryViewModel : BaseViewModel
    {
        public string nm { get; set; }
        public int id { get; set; }
        public int? dd { get; set; }
        public int? ud { get; set; }
        public List<ResultSummary> ResultSummaryList { get; set; }
    }

}
