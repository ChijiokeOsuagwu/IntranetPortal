using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IntranetPortal.Areas.LVM.Models
{
    public class SendResumptionNoticeViewModel:BaseViewModel
    {
        public long LeaveResumptionId { get; set; }
        public long LeaveRequestId { get; set; }
        public string LeaveEmployeeName { get; set; }

        [Required(ErrorMessage ="Select the staff to Send this notice to.")]
        public string SendToEmployeeName { get; set; }

        [Required(ErrorMessage ="Role is required!")]
        public string SendToEmployeeRole { get; set; }
        public DateTime ApprovedResumptionDate { get; set; }

        [Required(ErrorMessage ="Actual Resumption Date is required!")]
        public DateTime ResumptionDateByEmployee { get; set; }
        public int NoOfExtraDaysByEmployee { get; set; }
        public int NoOfUnusedDaysByEmployee { get; set; }
        public bool EmployeeRequestAdjustment { get; set; }
        public string RequestedAdjustmentType { get; set; }
        public string ReasonByEmployee { get; set; }
        public DateTime DateRecordedByEmployee { get; set; }

        public LeaveRequest LeaveRequest { get; set; }
    }
}
