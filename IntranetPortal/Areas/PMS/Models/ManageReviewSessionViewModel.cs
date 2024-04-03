using IntranetPortal.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetPortal.Base.Models.PmsModels;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ManageReviewSessionViewModel:BaseViewModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Session Name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Performance Year")]
        public int ReviewYearId { get; set; }

        [Display(Name = "Performance Year")]
        public string ReviewYearName { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int ReviewTypeId { get; set; }

        [Display(Name = "Type")]
        public string ReviewTypeName { get; set; }

        [Required]
        [Display(Name = "Period Starting From")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Period Starting From")]
        public string StartDateFormatted { get; set; }

        [Required]
        [Display(Name = "Period Ending On")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Period Ending On")]
        public string EndDateFormatted { get; set; }

        [Required]
        [Display(Name = "Minimum No. of Competencies")]
        [Range(0, 50)]
        public int MinNoOfCompetencies { get; set; }

        [Required]
        [Display(Name = "Maximum No. of Competencies")]
        [Range(0, 50)]
        public int MaxNoOfCompetencies { get; set; }

        [Required]
        [Display(Name = "Total Competency Score")]
        [Range(0, 100)]
        public decimal TotalCompetencyScore { get; set; }

        [Required]
        [Display(Name = "Total KPA Score")]
        [Range(0, 100)]
        public decimal TotalKpaScore { get; set; }

        [Required]
        [Display(Name = "Combined Final Score")]
        [Range(100, 200)]
        public decimal TotalCombinedScore { get; set; }

        [Display(Name = "Status")]
        [Required]
        public int IsActive { get; set; }

        public ReviewSession ConvertToReviewSession()
        {
            return new ReviewSession
            {
                EndDate = EndDate,
                Id = Id.Value,
                MaxNoOfCompetencies = MaxNoOfCompetencies,
                MinNoOfCompetencies = MinNoOfCompetencies,
                Name = Name,
                ReviewTypeId = ReviewTypeId,
                ReviewTypeName = ReviewTypeName,
                ReviewYearId = ReviewYearId,
                ReviewYearName = ReviewYearName,
                StartDate = StartDate,
                TotalCombinedScore = TotalCombinedScore,
                TotalCompetencyScore = TotalCompetencyScore,
                TotalKpaScore = TotalKpaScore,
            };
        }

        public ManageReviewSessionViewModel ExtractViewModel(ReviewSession reviewSession)
        {
            return new ManageReviewSessionViewModel
            {
                EndDate = reviewSession.EndDate,
                Id = reviewSession.Id,
                MaxNoOfCompetencies = reviewSession.MaxNoOfCompetencies,
                MinNoOfCompetencies = reviewSession.MinNoOfCompetencies,
                Name = reviewSession.Name,
                ReviewTypeId = reviewSession.ReviewTypeId,
                ReviewTypeName = reviewSession.ReviewTypeName,
                ReviewYearId = reviewSession.ReviewYearId,
                ReviewYearName = reviewSession.ReviewYearName,
                StartDate = reviewSession.StartDate,
                TotalCombinedScore = reviewSession.TotalCombinedScore,
                TotalCompetencyScore = reviewSession.TotalCompetencyScore,
                TotalKpaScore = reviewSession.TotalKpaScore,
            };
        }
    }
}
