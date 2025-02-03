using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeavePlanViewModel:BaseViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Display(Name ="Full Name")]
        public string EmployeeFullName { get; set; }

        [Display(Name ="Year")]
        public int LeaveYear { get; set; }

        [Required]
        [Display(Name ="Type")]
        public string LeaveTypeCode { get; set; }

        [Display(Name = "Type")]
        public string LeaveTypeName { get; set; }

        [Display(Name = "Reason for Leave")]
        public string LeaveReason { get; set; }

        [Display(Name = "Status")]
        public string LeaveStatus { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime LeaveStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? LeaveEndDate { get; set; }

        [Required]
        [Display(Name = "Duration")]
        public int Duration { get; set; }

        [Required]
        [Display(Name = "Duration Type")]
        public int DurationTypeId { get; set; }

        [Display(Name = "Duration Type")]
        public string DurationTypeDescription { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsPlan { get; set; }

        public bool ApprovedByLineManager { get; set; }
        public bool ApprovedByStationManager { get; set; }
        public bool ApprovedByHeadOfDepartment { get; set; }
        public bool ApprovedByHR { get; set; }
        public bool ApprovedByExecutiveManagement { get; set; }

        public DateTime? ResumptionDate { get; set; }
        public DateTime? CloseRequestDate { get; set; }
        public DateTime? LineManagersResumptionDate { get; set; }
        public DateTime? LineManagerConfirmResumptionDate { get; set; }
        public string LineManagerConfirmResumptionBy { get; set; }
        public DateTime? HrConfirmResumptionDate { get; set; }
        public string HrConfirmResumptionBy { get; set; }

        public LeavePlanViewModel Extract(EmployeeLeave leave)
        {
            return new LeavePlanViewModel
            {
                DepartmentId = leave.DepartmentId,
                DepartmentName = leave.DepartmentName,
                Duration = leave.Duration,
                DurationTypeDescription = leave.DurationTypeDescription,
                DurationTypeId = leave.DurationTypeId,
                EmployeeFullName = leave.EmployeeFullName,
                EmployeeId = leave.EmployeeId,
                Id = leave.Id,
                IsPlan = leave.IsPlan,
                LeaveEndDate = leave.LeaveEndDate,
                LeaveReason = leave.LeaveReason,
                LeaveStartDate = leave.LeaveStartDate,
                LeaveStatus = leave.LeaveStatus,
                LeaveTypeCode = leave.LeaveTypeCode,
                LeaveTypeName = leave.LeaveTypeName,
                LeaveYear = leave.LeaveYear,
                LocationId = leave.LocationId,
                LocationName = leave.LocationName,
                UnitId = leave.UnitId,
                UnitName = leave.UnitName,

                ApprovedByExecutiveManagement = leave.ApprovedByExecutiveManagement,
                ApprovedByHeadOfDepartment = leave.ApprovedByHeadOfDepartment,
                ApprovedByHR = leave.ApprovedByHR,
                ApprovedByLineManager = leave.ApprovedByLineManager,
                ApprovedByStationManager = leave.ApprovedByStationManager,

                ResumptionDate = leave.ResumptionDate,
                CloseRequestDate = leave.CloseRequestDate,

                LineManagersResumptionDate = leave.LineManagerConfirmResumptionDate,
                LineManagerConfirmResumptionBy = leave.LineManagerConfirmResumptionBy,
                LineManagerConfirmResumptionDate = leave.LineManagerConfirmResumptionDate,

                HrConfirmResumptionDate = leave.HrConfirmResumptionDate,
                HrConfirmResumptionBy = leave.HrConfirmResumptionBy,
            };
        }
        public EmployeeLeave Convert()
        {
            return new EmployeeLeave
            {
                DepartmentId = DepartmentId,
                DepartmentName = DepartmentName,
                Duration = Duration,
                DurationTypeDescription = DurationTypeDescription,
                DurationTypeId = DurationTypeId,
                EmployeeFullName = EmployeeFullName,
                EmployeeId = EmployeeId,
                Id = Id,
                IsPlan = IsPlan,
                LeaveEndDate = LeaveEndDate ?? DateTime.Today,
                LeaveReason = LeaveReason,
                LeaveStartDate = LeaveStartDate,
                LeaveStatus = LeaveStatus,
                LeaveTypeCode = LeaveTypeCode,
                LeaveTypeName = LeaveTypeName,
                LeaveYear = LeaveYear,
                LocationId = LocationId,
                LocationName = LocationName,
                UnitId = UnitId,
                UnitName = UnitName,

                ApprovedByExecutiveManagement = ApprovedByExecutiveManagement,
                ApprovedByHeadOfDepartment = ApprovedByHeadOfDepartment,
                ApprovedByHR = ApprovedByHR,
                ApprovedByLineManager = ApprovedByLineManager,
                ApprovedByStationManager = ApprovedByStationManager,

                ResumptionDate = ResumptionDate,
                CloseRequestDate = CloseRequestDate,

                LineManagersResumptionDate = LineManagerConfirmResumptionDate,
                LineManagerConfirmResumptionBy = LineManagerConfirmResumptionBy,
                LineManagerConfirmResumptionDate = LineManagerConfirmResumptionDate,

                HrConfirmResumptionDate = HrConfirmResumptionDate,
                HrConfirmResumptionBy = HrConfirmResumptionBy,
            };
        }
    }
}
