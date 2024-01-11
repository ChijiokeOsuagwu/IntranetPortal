using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ReviewScore
    {
        public int ReviewHeaderId { get; set; }
        public string AppraiserId { get; set; }
        public int ReviewMetricTypeId { get; set; }
        public decimal TotalScore { get; set; }
    }
}
