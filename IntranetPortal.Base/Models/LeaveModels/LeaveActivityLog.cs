using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveActivityLog
    {
            public long LeaveActivityLogId { get; set; }
            public string ActivityDescription { get; set; }
            public DateTime? ActivityTime { get; set; }
            public long? LeaveRequestId { get; set; }
        public long? LeavePlanId { get; set; }
    }
}
