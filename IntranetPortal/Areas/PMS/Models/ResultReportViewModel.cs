using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ResultReportViewModel : BaseViewModel
    {
        public int id { get; set; }
        public int? ld { get; set; }
        public int? dd { get; set; }
        public int? ud { get; set; }
        public string ReviewSessionDescription { get; set; }
        public List<ResultDetail> ResultDetailList { get; set; }
    }

}
