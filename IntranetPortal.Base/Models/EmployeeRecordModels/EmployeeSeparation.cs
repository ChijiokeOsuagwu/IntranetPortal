using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeSeparation
    {
        public int EmployeeSeparationId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int SeparationTypeId { get; set; }
        public string SeparationTypeDescription { get; set; }
        public int? SeparationReasonId { get; set; }
        public string SeparationReasonDescription { get; set; }
        public string SeparationReasonExplanation { get; set; }
        public DateTime? ExpectedLastWorkedDate { get; set; }
        public DateTime? ActualLastWorkedDate { get; set; }
        public DateTime? NoticeServedDate { get; set; }
        public int NoticePeriodInMonths { get; set; }
        public int OutstandingLeaveDays { get; set; }
        public int OutstandingWorkDays { get; set; }
        public bool EligibleForRehire { get; set; }
        public bool ReturnedAssignedAssets { get; set; }
        public bool IsOwed { get; set; }
        public bool IsIndebted { get; set; }
        public bool HasOutstandings { get; set; }
        public bool HasPayments { get; set; }
        public DateTime? RecordCreatedDate { get; set; }
        public string RecordCreatedBy { get; set; }
        public DateTime? RecordModifiedDate { get; set; }
        public string RecordModifiedBy { get; set; }
    }
}
