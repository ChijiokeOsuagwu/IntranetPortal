using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ManageAppraisalCdgViewModel : BaseViewModel
    {
        public int? ReviewCdgId { get; set; }
        [Required]
        [Display(Name = "CDG Description*")]
        [MaxLength(500)]
        public string ReviewCdgDescription { get; set; }

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
        [Display(Name = "Objective*")]
        [MaxLength(500)]
        public string ReviewCdgObjective { get; set; }

        [Required]
        [Display(Name = "Action Plan*")]
        [MaxLength(500)]
        public string ReviewCdgActionPlan { get; set; }

        public ReviewCDG ConvertToReviewCdg()
        {
            return new ReviewCDG
            {
                AppraiseeId = AppraiseeId,
                AppraiseeName = AppraiseeName,
                ReviewHeaderId = ReviewHeaderId,
                ReviewCdgDescription = ReviewCdgDescription,
                ReviewCdgId = ReviewCdgId ?? 0,
                ReviewCdgObjective = ReviewCdgObjective,
                ReviewCdgActionPlan = ReviewCdgActionPlan,
                ReviewSessionId = ReviewSessionId,
                ReviewSessionName = ReviewSessionDescription,
                ReviewYearId = ReviewYearId,
                ReviewYearName = ReviewYearName,
            };
        }

    }
}
