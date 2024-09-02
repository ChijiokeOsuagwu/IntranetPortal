using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ApproveContractViewModel : BaseViewModel
    {
        public int ReviewApprovalID { get; set; }
        public int ApprovalTypeID { get; set; }
        public string ApprovalTypeDescription { get; set; }

        [Required]
        public int ReviewHeaderID { get; set; }

        [Required]
        [Display(Name = "Appover*")]
        public string ApproverID { get; set; }

        [Display(Name = "Appover*")]
        public string ApproverName { get; set; }

        [Required]
        [Display(Name = "Approving As*")]
        public int ApproverRoleID { get; set; }

        [Display(Name = "Approved As*")]
        public string ApproverRoleDescription { get; set; }

        [Required]
        public string AppraiseeID { get; set; }

        [Display(Name = "Appraisee")]
        public string AppraiseeName { get; set; }

        [Required]
        public bool IsApproved { get; set; }
        public DateTime? ApprovedTime { get; set; }

        [Display(Name = "Comments")]
        public string ApprovedComments { get; set; }

        public int? ReviewMetricID { get; set; }
        public string ReviewMetricDescription { get; set; }

        public int SubmissionID { get; set; }

        public int ReviewSessionID { get; set; }

        public ReviewApproval ConvertToReviewApproval()
        {
            return new ReviewApproval
            {
                AppraiseeId = AppraiseeID,
                AppraiseeName = AppraiseeName,
                ApprovalTypeDescription = ApprovalTypeDescription,
                ApprovalTypeId = ApprovalTypeID,
                ApprovedComments = ApprovedComments,
                ApprovedTime = ApprovedTime,
                ApproverId = ApproverID,
                ApproverName = ApproverName,
                ApproverRoleDescription = ApproverRoleDescription,
                ApproverRoleId = ApproverRoleID,
                IsApproved = IsApproved,
                ReviewApprovalId = ReviewApprovalID,
                ReviewHeaderId = ReviewHeaderID,
                ReviewSessionId = ReviewSessionID,
                ReviewMetricDescription = ReviewMetricDescription,
                ReviewMetricId = ReviewMetricID,
            };
        }
    }
}
