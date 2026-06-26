using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveTransaction
    {
        public long LeaveTransactionId { get; set; }
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }
        public int LeaveYear { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public long? LeaveRequestId { get; set; }
        public long? LeaveAdjustmentId { get; set; }
        public int NumberOfDaysUsed { get; set; }
        public int NumberOfDaysGiven { get; set; }
        public int OpeningBalance { get; set; }
        public int PreviousBalance { get; set; }
        public string TransactionDescription { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionRecordedBy { get; set; }
        public int LeaveUnitId { get; set; }
        public string LeaveUnitName { get; set; }
        public int LeaveDepartmentId { get; set; }
        public string LeaveDepartmentName { get; set; }
        public int LeaveLocationId { get; set; }
        public string LeaveLocationName { get; set; }
    }
}
