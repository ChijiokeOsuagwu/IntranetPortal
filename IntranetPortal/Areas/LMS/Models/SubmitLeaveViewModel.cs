using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class SubmitLeaveViewModel:BaseViewModel
    {
        public long Id { get; set; }

        [Required]
        public long LeaveId { get; set; }

        [Required]
        public string FromEmployeeId { get; set; }

        [Display(Name ="Submitted By")]
        public string FromEmployeeName { get; set; }

        [Required]
        [Display(Name = "Submit To*: ")]
        public string ToEmployeeId { get; set; }

        [Display(Name ="Submit To: ")]
        public string ToEmployeeName { get; set; }

        [Required]
        [Display(Name = "Submit as your*: ")]
        public string ToEmployeeRole { get; set; }

        [Display(Name ="Submit For*: ")]
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
                FromEmployeeId = FromEmployeeId,
                FromEmployeeName = FromEmployeeName,
                Id = Id,
                IsActioned = IsActioned,
                LeaveId = LeaveId,
                Message = Message,
                Purpose = Purpose,
                TimeActioned = TimeActioned,
                TimeSubmitted = TimeSubmitted,
                ToEmployeeId = ToEmployeeId,
                ToEmployeeName = ToEmployeeName,
                ToEmployeeRole = ToEmployeeRole
            };
        }
        public SubmitLeaveViewModel Convert(LeaveSubmission e)
        {
            return new SubmitLeaveViewModel
            {
                FromEmployeeId = e.FromEmployeeId,
                FromEmployeeName = FromEmployeeName,
                Id = Id,
                IsActioned = IsActioned,
                LeaveId = LeaveId,
                Message = Message,
                Purpose = Purpose,
                TimeActioned = TimeActioned,
                TimeSubmitted = TimeSubmitted,
                ToEmployeeId = ToEmployeeId,
                ToEmployeeName = ToEmployeeName,
                ToEmployeeRole = ToEmployeeRole
            };
        }

    }
}
