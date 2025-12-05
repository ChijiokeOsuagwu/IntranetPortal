using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeavePlanViewModel : BaseViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Display(Name = "Full Name")]
        public string EmployeeFullName { get; set; }

        [Display(Name = "Year")]
        public int LeaveYear { get; set; }

        [Required]
        [Display(Name = "Type")]
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
                Duration = leave.ProposedLeaveDuration,
                DurationTypeDescription = leave.ProposedDurationDescription,
                DurationTypeId = leave.ProposedDurationTypeId,
                EmployeeFullName = leave.EmployeeFullName,
                EmployeeId = leave.EmployeeId,
                Id = leave.Id,
                IsPlan = leave.IsPlan,
                LeaveEndDate = leave.ProposedLeaveEndDate,
                LeaveReason = leave.LeaveReason,
                LeaveStartDate = leave.ProposedLeaveStartDate,
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

                //ResumptionDate = leave.ResumptionDate,
                CloseRequestDate = leave.RequestCloseDate,

                LineManagersResumptionDate = leave.LineManagerConfirmResumptionDate,
                LineManagerConfirmResumptionBy = leave.LineManagerConfirmResumptionBy,
                LineManagerConfirmResumptionDate = leave.LineManagerConfirmResumptionDate,

                HrConfirmResumptionDate = leave.HrConfirmResumptionDate,
                HrConfirmResumptionBy = leave.HrConfirmResumptionBy,
            };
        }
        //public EmployeeLeave Convert()
        public EmployeeLeave Convert()
        {
            EmployeeLeave e = new EmployeeLeave();

            e.DepartmentId = DepartmentId;
            e.DepartmentName = DepartmentName;
            e.ProposedLeaveDuration = Duration;
            e.ProposedDurationDescription = DurationTypeDescription;
            e.ProposedDurationTypeId = DurationTypeId;
            e.EmployeeFullName = EmployeeFullName;
            e.EmployeeId = EmployeeId;
            e.Id = Id;
            e.IsPlan = IsPlan;
            e.ProposedLeaveEndDate = LeaveEndDate ?? DateTime.Today;
            e.LeaveReason = LeaveReason;
            e.ProposedLeaveStartDate = LeaveStartDate;
            e.LeaveStatus = LeaveStatus;
            e.LeaveTypeCode = LeaveTypeCode;
            e.LeaveTypeName = LeaveTypeName;
            e.LeaveYear = LeaveYear;
            e.LocationId = LocationId;
            e.LocationName = LocationName;
            e.UnitId = UnitId;
            e.UnitName = UnitName;

            e.ApprovedByExecutiveManagement = ApprovedByExecutiveManagement;
            e.ApprovedByHeadOfDepartment = ApprovedByHeadOfDepartment;
            e.ApprovedByHR = ApprovedByHR;
            e.ApprovedByLineManager = ApprovedByLineManager;
            e.ApprovedByStationManager = ApprovedByStationManager;

            //ResumptionDate = ResumptionDate,
            e.RequestCloseDate = CloseRequestDate;

            e.LineManagersResumptionDate = LineManagerConfirmResumptionDate;
            e.LineManagerConfirmResumptionBy = LineManagerConfirmResumptionBy;
            e.LineManagerConfirmResumptionDate = LineManagerConfirmResumptionDate;

            e.HrConfirmResumptionDate = HrConfirmResumptionDate;
            e.HrConfirmResumptionBy = HrConfirmResumptionBy;
            return e;
    }
    }
}
