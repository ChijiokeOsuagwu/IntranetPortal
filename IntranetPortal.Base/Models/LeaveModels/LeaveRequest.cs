using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveRequest
    {
        public long LeaveRequestId { get; set; }
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }
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
        public int LeaveRequestStatusId { get; set; }
        public string LeaveRequestStatusDescription { get; set; }
        public DateTime RequestedStartDate { get; set; }
        public DateTime RequestedEndDate { get; set; }
        public int RequestedDuration { get; set; }
        public string RequestedDurationDescription { get; set; }
        public int RequestedDurationTypeId { get; set; }
        public string RequestedDurationTypeDescription { get; set; }
        public DateTime? RequestedResumptionDate { get; set; }

        public DateTime? ActualLeaveStartDate { get; set; }
        public DateTime? ActualLeaveEndDate { get; set; }
        public int ActualLeaveDuration { get; set; }
        public int ActualLeaveDurationTypeId { get; set; }
        public string ActualLeaveDurationDescription { get; set; }

        public DateTime? LineManagersResumptionDate { get; set; }
        public DateTime? LineManagerConfirmResumptionTime { get; set; }
        public string LineManagerConfirmResumptionBy { get; set; }

        public DateTime? HrResumptionDate { get; set; }
        public DateTime? HrConfirmResumptionTime { get; set; }
        public string HrConfirmResumptionBy { get; set; }
        public DateTime? LeaveRequestCloseDate { get; set; }

        public bool IsApprovedByLineManager { get; set; }
        public bool IsApprovedByStationManager { get; set; }
        public bool IsApprovedByHeadOfDepartment { get; set; }
        public bool IsApprovedByHR { get; set; }
        public bool IsApprovedByExecutiveManagement { get; set; }
    }
}
