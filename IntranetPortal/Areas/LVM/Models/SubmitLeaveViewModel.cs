using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class SubmitLeaveViewModel:BaseViewModel
    {
        public long LeaveSubmissionId { get; set; }

        public long? LeavePlanId { get; set; }

        public long? LeaveRequestId { get; set; }

        [Required]
        [Display(Name = "Submitted By")]
        public string FromEmployeeName { get; set; }

        [Required]
        [Display(Name = "Submit To: ")]
        public string ToEmployeeName { get; set; }

        [Required]
        [Display(Name = "Submit as your*: ")]
        public string ToEmployeeRole { get; set; }

        [Display(Name = "Submit For*: ")]
        public string Purpose { get; set; }

        [Display(Name = "Time Submitted: ")]
        public DateTime? TimeSubmitted { get; set; }

        [Display(Name = "Message: ")]
        public string Message { get; set; }

        public bool IsActioned { get; set; }
        public DateTime? TimeActioned { get; set; }

        public LeaveSubmission Convert()
        {
            return new LeaveSubmission
            {
                FromEmployeeName = FromEmployeeName,
                LeaveSubmissionId = LeaveSubmissionId,
                IsActioned = IsActioned,
                LeavePlanId = LeavePlanId,
                LeaveRequestId = LeaveRequestId,
                Message = Message,
                Purpose = Purpose,
                TimeActioned = TimeActioned,
                TimeSubmitted = TimeSubmitted,
                ToEmployeeName = ToEmployeeName,
                ToEmployeeRole = ToEmployeeRole
            };
        }
        public SubmitLeaveViewModel Convert(LeaveSubmission e)
        {
            return new SubmitLeaveViewModel
            {
                FromEmployeeName = FromEmployeeName,
                LeaveSubmissionId = LeaveSubmissionId,
                IsActioned = IsActioned,
                LeavePlanId = LeavePlanId,
                LeaveRequestId = LeaveRequestId,
                Message = Message,
                Purpose = Purpose,
                TimeActioned = TimeActioned,
                TimeSubmitted = TimeSubmitted,
                ToEmployeeName = ToEmployeeName,
                ToEmployeeRole = ToEmployeeRole
            };
        }


    }
}
