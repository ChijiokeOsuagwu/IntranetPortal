using IntranetPortal.Base.Models.WksModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ProductivityReportViewModel
    {
        public int? FromMonth { get; set; }
        [Required]
        public int FromYear { get; set; }
        public int? ToMonth { get; set; }
        [Required]
        public int ToYear { get; set; }
        public int? UnitID { get; set; }
        public int? DepartmentID { get; set; }
        public int? LocationID { get; set; }
        public string ReportTitle { get; set; }
        public string ReportStartDate { get; set; }
        public string ReportEndDate { get; set; }
        public List<TaskEvaluationHeaderSummary> EvaluationHeaderSummaryList { get; set; }
    }
}
