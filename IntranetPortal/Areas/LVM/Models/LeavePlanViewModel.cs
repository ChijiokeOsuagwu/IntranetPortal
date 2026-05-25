using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeavePlanViewModel:BaseViewModel
    {
        public long LeavePlanId { get; set; }

        [Required]
        [Display(Name ="Name")]
        public string LeaveEmployeeId { get; set; }

        [Display(Name = "Name")]
        public string LeaveEmployeeName { get; set; }
        public int LeaveUnitId { get; set; }
        public string LeaveUnitName { get; set; }
        public int LeaveDepartmentId { get; set; }
        public string LeaveDepartmentName { get; set; }
        public int LeaveLocationId { get; set; }
        public string LeaveLocationName { get; set; }

        [Display(Name = "Year")]
        [Required]
        public int LeaveYear { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string LeaveTypeCode { get; set; }

        [Display(Name = "Type")]
        public string LeaveTypeName { get; set; }

        [MaxLength(5000)]
        [Display(Name = "Reason")]
        public string LeaveReason { get; set; }

        [Display(Name = "Leave Start On")]
        public DateTime? LeavePlanStartDate { get; set; }

        [Display(Name = "Leave Start On")]
        public string LeavePlanStartDateFormatted { get; set; }

        [Display(Name = "Leave Ends On")]
        public DateTime? LeavePlanEndDate { get; set; }

        [Display(Name = "Leave Ends On")]
        public string LeavePlanEndDateFormatted { get; set; }

        [Display(Name = "Resumes Work On")]
        public DateTime? LeavePlanResumptionDate { get; set; }

        [Display(Name = "Resumes Work On")]
        public DateTime? LeavePlanResumptionDateFormatted { get; set; }

        public int LeavePlanDurationInDays { get; set; }

        [Required]
        public int LeavePlanDuration { get; set; }

        [Required]
        public int LeavePlanDurationTypeId { get; set; }
        public string LeavePlanDurationTypeDescription { get; set; }
        public string LeavePlanDurationDescription { get; set; }

        public int LeavePlanStatusId { get; set; }
        public string LeavePlanStatusDescription { get; set; }

        public LeavePlanViewModel Extract(LeavePlan plan)
        {
            return new LeavePlanViewModel
            {
                LeaveDepartmentId = plan.LeaveDepartmentId,
                LeaveDepartmentName = plan.LeaveDepartmentName,
                LeaveEmployeeId = plan.LeaveEmployeeId,
                LeaveEmployeeName = plan.LeaveEmployeeName,
                LeaveLocationId = plan.LeaveLocationId,
                LeaveLocationName = plan.LeaveLocationName,
                LeavePlanId = plan.LeavePlanId,
                LeaveReason = plan.LeaveReason,
                LeaveTypeCode = plan.LeaveTypeCode,
                LeaveTypeName = plan.LeaveTypeName,
                LeaveUnitId = plan.LeaveUnitId,
                LeaveUnitName = plan.LeaveUnitName,
                LeaveYear = plan.LeaveYear,
                LeavePlanDuration = plan.LeavePlanDuration,
                LeavePlanDurationDescription = plan.LeavePlanDurationDescription,
                LeavePlanDurationInDays = plan.LeavePlanDurationInDays,
                LeavePlanDurationTypeDescription = plan.LeavePlanDurationTypeDescription,
                LeavePlanDurationTypeId = plan.LeavePlanDurationTypeId,
                LeavePlanEndDate = plan.LeavePlanEndDate,
                LeavePlanResumptionDate = plan.LeavePlanResumptionDate,
                LeavePlanStartDate = plan.LeavePlanStartDate,
                LeavePlanStatusId = plan.LeavePlanStatusId,
                LeavePlanStatusDescription = plan.LeavePlanStatusDescription,
            };
        }
        
        public LeavePlan Convert()
        {
            LeavePlan p = new LeavePlan();

            p.LeaveDepartmentId = LeaveDepartmentId;
            p.LeaveDepartmentName = LeaveDepartmentName;
            p.LeaveEmployeeId = LeaveEmployeeId;
            p.LeaveEmployeeName = LeaveEmployeeName;
            p.LeaveLocationId = LeaveLocationId;
            p.LeaveLocationName = LeaveLocationName;
            p.LeavePlanId = LeavePlanId;
            p.LeaveReason = LeaveReason;
            p.LeaveTypeCode = LeaveTypeCode;
            p.LeaveTypeName = LeaveTypeName;
            p.LeaveUnitId = LeaveUnitId;
            p.LeaveUnitName = LeaveUnitName;
            p.LeaveYear = LeaveYear;
            p.LeavePlanDuration = LeavePlanDuration;
            p.LeavePlanDurationDescription = LeavePlanDurationDescription;
            p.LeavePlanDurationInDays = LeavePlanDurationInDays;
            p.LeavePlanDurationTypeDescription = LeavePlanDurationTypeDescription;
            p.LeavePlanDurationTypeId = LeavePlanDurationTypeId;
            p.LeavePlanEndDate = LeavePlanEndDate;
            p.LeavePlanResumptionDate = LeavePlanResumptionDate;
            p.LeavePlanStartDate = LeavePlanStartDate;
            p.LeavePlanStatusId = LeavePlanStatusId;
            p.LeavePlanStatusDescription = LeavePlanStatusDescription;

            switch (LeavePlanDurationTypeId) 
            {
                case (int)DurationTypeEnum.WorkingDays:
                    p.LeavePlanDurationTypeDescription = "Working Day(s)";
                    p.LeavePlanDurationDescription = $"{LeavePlanDuration} Working Day(s)";
                    break;
                case (int)DurationTypeEnum.Days:
                    p.LeavePlanDurationTypeDescription = "Day(s)";
                    p.LeavePlanDurationDescription = $"{LeavePlanDuration} Day(s)";
                    break;
                case (int)DurationTypeEnum.Weeks:
                    p.LeavePlanDurationTypeDescription = "Week(s)";
                    p.LeavePlanDurationDescription = $"{LeavePlanDuration} Week(s)";
                    break;
                case (int)DurationTypeEnum.Months:
                    p.LeavePlanDurationTypeDescription = "Month(s)";
                    p.LeavePlanDurationDescription = $"{LeavePlanDuration} Month(s)";
                    break;
                case (int)DurationTypeEnum.Years:
                    p.LeavePlanDurationTypeDescription = "Year(s)";
                    p.LeavePlanDurationDescription = $"{LeavePlanDuration} Year(s)";
                    break;
                default:
                    break;
            }

            return p;
        }

    }
}
