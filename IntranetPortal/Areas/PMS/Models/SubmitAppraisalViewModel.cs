using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class SubmitAppraisalViewModel : BaseViewModel
    {
        public int ReviewSubmissionID { get; set; }
        [Required]
        public int ReviewHeaderID { get; set; }
        public string AppraiseeID { get; set; }
        public int ReviewSessionID { get; set; }

        [Required]
        public string FromEmployeeID { get; set; }

        [Display(Name = "Submit By:")]
        public string FromEmployeeName { get; set; }

        [Required]
        [Display(Name = "Submit To*:")]
        public string ToEmployeeID { get; set; }

        [Display(Name = "Submit To:")]
        public string ToEmployeeName { get; set; }

        [Required]
        [Display(Name = "Role*")]
        public int ToEmployeeRoleID { get; set; }

        [Display(Name = "Role")]
        public string ToEmployeeRoleName { get; set; }

        [Required]
        [Display(Name = "Purpose*")]
        public int SubmissionPurposeID { get; set; }

        [Display(Name = "Purpose")]
        public string SubmissionPurposeDescription { get; set; }

        public DateTime? TimeSubmitted { get; set; }

        [Display(Name = "Comment")]
        public string SubmissionMessage { get; set; }

        public ReviewSubmission ConvertToReviewSubmission()
        {
            return new ReviewSubmission
            {
                FromEmployeeId = FromEmployeeID,
                FromEmployeeName = FromEmployeeName,
                ReviewHeaderId = ReviewHeaderID,
                ReviewSessionId = ReviewSessionID,
                ReviewSubmissionId = ReviewSubmissionID,
                SubmissionMessage = SubmissionMessage,
                SubmissionPurposeDescription = SubmissionPurposeDescription,
                SubmissionPurposeId = SubmissionPurposeID,
                ToEmployeeId = ToEmployeeID,
                ToEmployeeName = ToEmployeeName,
                ToEmployeeRoleId = ToEmployeeRoleID,
                ToEmployeeRoleName = ToEmployeeRoleName,
            };
        }
    }
}
