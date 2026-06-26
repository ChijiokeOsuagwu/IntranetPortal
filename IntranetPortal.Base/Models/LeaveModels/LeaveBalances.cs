using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveBalances
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public int LeaveYear { get; set; }
        public bool PreviouBalanceCanBeCarriedOver { get; set; }
        public int PreviousBalanceXpiryMonth { get; set; }
        public long CurrentYearProfileLeaveDays { get; set; }
        public long PreviousYearLeaveBalance { get; set; }
        public long TotalLeaveDaysGiven { get; set; }
        public long TotalLeaveDaysUsed { get; set; }
        public long TotalOutstandingLeaveDays { get; set; }
        public long AnnualLeaveDaysPlusPreviousYearBalance { get; set; }
        public long AnnualLeaveDaysPlusLeaveDaysGiven { get; set; }
        public long AnnualLeaveDaysPlusLeaveDaysGivenMinusLeaveDaysUsed { get; set; }
        public long AnnualLeaveDaysPlusPreviousYearBalancePlusLeaveDaysGivenMinusLeaveDaysUsed { get; set; }
    }
}
