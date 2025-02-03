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
        public int LeaveYear { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public string LeaveReason { get; set; }
        public string LeaveStatus { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }
        public int Duration { get; set; }
        public int DurationTypeId { get; set; }
        public string DurationTypeDescription { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsPlan { get; set; }

        public DateTime? ResumptionDate { get; set; }
        public DateTime? CloseRequestDate { get; set; }
        public DateTime? LineManagersResumptionDate { get; set; }
        public DateTime? LineManagerConfirmResumptionDate { get; set; }
        public string LineManagerConfirmResumptionBy { get; set; }
        public DateTime? HrConfirmResumptionDate { get; set; }
        public string HrConfirmResumptionBy { get; set; }

        public bool ApprovedByLineManager { get; set; }
        public bool ApprovedByStationManager { get; set; }
        public bool ApprovedByHeadOfDepartment { get; set; }
        public bool ApprovedByHR { get; set; }
        public bool ApprovedByExecutiveManagement { get; set; }
    }
}
