using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveApproval
    {
        public long LeaveApprovalId { get; set; }
        public long? LeavePlanId { get; set; }
        public long? LeaveRequestId { get; set; }
        public string ApproverName { get; set; }
        public string ApproverRole { get; set; }
        public string ApplicantName { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? TimeApproved { get; set; }
        public string ApproverComments { get; set; }
    }
}
