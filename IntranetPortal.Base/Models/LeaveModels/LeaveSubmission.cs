using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveSubmission
    {
        public long LeaveSubmissionId { get; set; }
        public long? LeavePlanId { get; set; }
        public long? LeaveRequestId { get; set; }
        public string FromEmployeeName { get; set; }
        public string ToEmployeeName { get; set; }
        public string ToEmployeeRole { get; set; }
        public string Purpose { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        public string Message { get; set; }
        public bool IsActioned { get; set; }
        public DateTime? TimeActioned { get; set; }
    }
}
