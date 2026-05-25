using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveBalances
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public int LeaveYear { get; set; }
        public long CurrentYearPrifileLeaveDays { get; set; }
        public long CarriedOverLeaveBalance { get; set; }
        public long TotalLeaveDaysDue { get; set; }
        public long TotalLeaveDaysUsed { get; set; }
        public long TotalOutstandingLeaveDays { get; set; }
    }
}
