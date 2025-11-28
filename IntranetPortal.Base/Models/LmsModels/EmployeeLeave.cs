using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LmsModels
{
    public class EmployeeLeave
    {
        public long Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeFullName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }

        public int LeaveYear { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public string LeaveReason { get; set; }
        public string LeaveStatus { get; set; }
        public bool IsPlan { get; set; }

        public DateTime ProposedLeaveStartDate { get; set; }
        public DateTime ProposedLeaveEndDate { get; set; }
        public int ProposedLeaveDuration { get; set; }
        public int ProposedDurationTypeId { get; set; }
        public string ProposedDurationDescription { get; set; }

        public DateTime? ApprovedLeaveStartDate { get; set; }
        public DateTime? ApprovedLeaveEndDate { get; set; }
        public int ApprovedLeaveDuration { get; set; }
        public int ApprovedDurationTypeId { get; set; }
        public string ApprovedDurationDescription { get; set; }

        public DateTime? ActualLeaveStartDate { get; set; }
        public DateTime? ActualLeaveEndDate { get; set; }
        public int ActualLeaveDuration { get; set; }
        public int ActualDurationTypeId { get; set; }
        public string ActualDurationDescription { get; set; }

        public DateTime? LineManagersResumptionDate { get; set; }
        public DateTime? LineManagerConfirmResumptionDate { get; set; }
        public string LineManagerConfirmResumptionBy { get; set; }

        public DateTime? HrResumptionDate { get; set; }
        public DateTime? HrConfirmResumptionDate { get; set; }
        public string HrConfirmResumptionBy { get; set; }
        public DateTime? RequestCloseDate { get; set; }

        public bool ApprovedByLineManager { get; set; }
        public bool ApprovedByStationManager { get; set; }
        public bool ApprovedByHeadOfDepartment { get; set; }
        public bool ApprovedByHR { get; set; }
        public bool ApprovedByExecutiveManagement { get; set; }
    }
}
