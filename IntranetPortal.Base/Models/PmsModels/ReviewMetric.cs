using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ReviewMetric
    {
        public int ReviewMetricId { get; set; }
        public string ReviewMetricDescription { get; set; }
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionDescription { get; set; }
        public string AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }
        public int ReviewMetricTypeId { get; set; }
        public string ReviewMetricTypeDescription { get; set; }
        public string ReviewMetricKpi { get; set; }
        public string ReviewMetricTarget { get; set; }
        public decimal ReviewMetricWeightage { get; set; }
        public string PrimaryAppraiserId { get; set; }
        public string PrimaryAppraiserName { get; set; }

    }
}
