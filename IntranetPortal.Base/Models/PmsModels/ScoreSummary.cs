using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ScoreSummary
    {
        public int ReviewHeaderId { get; set; }
        public string AppraiserId { get; set; }
        public decimal QuantitativeScore { get; set; }
        public decimal QualitativeScore { get; set; }
        public decimal TotalPerformanceScore { get; set; }
    }
}
