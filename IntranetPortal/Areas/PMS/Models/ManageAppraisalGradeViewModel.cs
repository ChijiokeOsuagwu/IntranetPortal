using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ManageAppraisalGradeViewModel : BaseViewModel
    {
        public int AppraisalGradeId { get; set; }

        [Display(Name = "Grade Description")]
        [MaxLength(100)]
        [Required]
        public string AppraisalGradeDescription { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public ReviewGradeType GradeType { get; set; }

        [Display(Name = "Lower Limit Score")]
        [Required]
        public decimal LowerBandScore { get; set; }

        [Display(Name = "Upper Limit Score")]
        [Required]
        public decimal UpperBandScore { get; set; }

        [Display(Name = "Rank")]
        [Required]
        public int GradeRank { get; set; }

        [Display(Name = "Rank")]
        public string GradeRankDescription { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }


        public AppraisalGrade ConvertToPerformanceGrade()
        {
            return new AppraisalGrade
            {
                ReviewSessionId = ReviewSessionId,
                ReviewSessionName = ReviewSessionName,
                GradeRank = GradeRank,
                GradeRankDescription = GradeRankDescription,
                GradeType = ReviewGradeType.Performance,
                LowerBandScore = LowerBandScore,
                UpperBandScore = UpperBandScore,
                AppraisalGradeDescription = AppraisalGradeDescription,
                AppraisalGradeId = AppraisalGradeId,

            };
        }

        public AppraisalGrade ConvertToCompetencyGrade()
        {
            return new AppraisalGrade
            {
                ReviewSessionId = ReviewSessionId,
                ReviewSessionName = ReviewSessionName,
                GradeRank = GradeRank,
                GradeRankDescription = GradeRankDescription,
                GradeType = ReviewGradeType.Competency,
                LowerBandScore = LowerBandScore,
                UpperBandScore = LowerBandScore,
                AppraisalGradeDescription = AppraisalGradeDescription,
                AppraisalGradeId = AppraisalGradeId,
            };
        }
    }
}
