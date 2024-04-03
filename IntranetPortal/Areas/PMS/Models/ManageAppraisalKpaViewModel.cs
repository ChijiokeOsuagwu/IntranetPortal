using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ManageAppraisalKpaViewModel : BaseViewModel
    {
        public int? ReviewMetricId { get; set; }
        [Required]
        [Display(Name = "Description*")]
        [MaxLength(500)]
        public string ReviewMetricDescription { get; set; }

        [Required]
        public int ReviewHeaderId { get; set; }
        [Required]
        public string AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }

        [Required]
        public int ReviewSessionId { get; set; }
        public string ReviewSessionDescription { get; set; }

        [Required]
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }

        [Required]
        [Display(Name = "Measurement*")]
        [MaxLength(500)]
        public string ReviewMetricKpi { get; set; }

        [Required]
        [Display(Name = "Target*")]
        [MaxLength(500)]
        public string ReviewMetricTarget { get; set; }

        [Required]
        [Display(Name = "Weightage*")]
        public decimal ReviewMetricWeightage { get; set; }

        public ReviewMetric ConvertToReviewMetric()
        {
            return new ReviewMetric
            {
                AppraiseeId = AppraiseeId,
                AppraiseeName = AppraiseeName,
                ReviewHeaderId = ReviewHeaderId,
                ReviewMetricDescription = ReviewMetricDescription,
                ReviewMetricId = ReviewMetricId ?? 0,
                ReviewMetricKpi = ReviewMetricKpi,
                ReviewMetricTarget = ReviewMetricTarget,
                ReviewMetricTypeDescription = "KPA",
                ReviewMetricTypeId = 0,
                ReviewMetricWeightage = ReviewMetricWeightage,
                ReviewSessionId = ReviewSessionId,
                ReviewSessionDescription = ReviewSessionDescription,
                ReviewYearId = ReviewYearId,
                ReviewYearName = ReviewYearName,
            };
        }
    }

}
