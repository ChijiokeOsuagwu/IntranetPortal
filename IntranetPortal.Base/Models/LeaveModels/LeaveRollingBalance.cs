using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveRollingBalance
    {
        public long RollingBalanceId { get; set; }
        public long? LeaveTransactionId { get; set; }
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }
        public int LeaveYear { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public bool PreviousBalanceCanBeCarriedOver { get; set; }
        public int PreviousBalanceExpiryMonth { get; set; }
        public string PreviousBalanceExpiryMonthName { get; set; }
        public long AnnualProfileLeaveDays { get; set; }
        public long PreviousYearsLeaveBalance { get; set; }
        public long LeaveDaysUsed { get; set; }
        public long LeaveDaysDeducted { get; set; }
        public long LeaveDaysAdded { get; set; }
        public long TotalOutstandingLeaveDaysBeforeExpiry { get; set; }
        public long TotalOutstandingLeaveDaysAfterExpiry { get; set; }
        public DateTime LeaveBalanceDate { get; set; }
        public int LeaveUnitId { get; set; }
        public string LeaveUnitName { get; set; }
        public int LeaveDepartmentId { get; set; }
        public string LeaveDepartmentName { get; set; }
        public int LeaveLocationId { get; set; }
        public string LeaveLocationName { get; set; }
    }
}
