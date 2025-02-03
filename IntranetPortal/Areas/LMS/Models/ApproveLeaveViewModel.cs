using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class ApproveLeaveViewModel:BaseViewModel
    {
        public long ApprovalId { get; set; }
        [Required]
        public long LeaveId { get; set; }
        public string ApproverName { get; set; }
        [Required]
        [Display(Name ="Approve As: ")]
        public string ApproverRole { get; set; }
        public string ApplicantName { get; set; }
        public bool IsApproved { get; set; } = true;
        public DateTime? TimeApproved { get; set; }
        [Display(Name ="Comment")]
        public string ApproverComments { get; set; }
        public string DocumentType { get; set; }

        public LeaveApproval Convert()
        {
            return new LeaveApproval
            {
                ApplicantName = ApplicantName,
                ApprovalId = ApprovalId,
                ApproverComments = ApproverComments,
                ApproverName = ApproverName,
                ApproverRole = ApproverRole,
                IsApproved = IsApproved,
                LeaveId = LeaveId,
                TimeApproved = TimeApproved
            };
        }
    }
}
