using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ManageAppraisalScheduleViewModel : BaseViewModel
    {
        public int SessionScheduleId { get; set; }

        [Required]
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }

        [Required]
        public int ScheduleTypeId { get; set; }

        [Display(Name = "Schedule Type")]
        public string ScheduleTypeDescription { get; set; }

        [Required]
        [Display(Name = "Activity Type")]
        public int ActivityTypeId { get; set; }

        [Display(Name = "Acivity Type Description")]
        public string ActivityTypeDescription { get; set; }

        [Display(Name = "Location")]
        public int? ScheduleLocationId { get; set; }

        [Display(Name = "Location")]
        public string ScheduleLocationName { get; set; }

        [Display(Name = "Department")]
        public int? ScheduleDepartmentId { get; set; }

        [Display(Name = "Department")]
        public string ScheduleDepartmentName { get; set; }

        [Display(Name = "Unit")]
        public int? ScheduleUnitId { get; set; }

        [Display(Name = "Unit")]
        public string ScheduleUnitName { get; set; }

        [Display(Name = "Employee")]
        public string ScheduleEmployeeId { get; set; }

        [Display(Name = "Employee")]
        public string ScheduleEmployeeName { get; set; }

        [Required]
        [Display(Name = "Start From")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduleStartTime { get; set; }

        [Required]
        [Display(Name = "End On")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduleEndTime { get; set; }

        public SessionSchedule ConvertToSessionSchedule()
        {
            return new SessionSchedule
            {
                ActivityType = (SessionActivityType)ActivityTypeId,
                SessionActivityTypeDescription = ActivityTypeDescription,
                ReviewSessionId = ReviewSessionId,
                ReviewSessionName = ReviewSessionName,
                ReviewYearId = ReviewYearId,
                ReviewYearName = ReviewYearName,
                ScheduleDepartmentId = ScheduleDepartmentId,
                ScheduleDepartmentName = ScheduleDepartmentName,
                ScheduleEmployeeId = ScheduleEmployeeId,
                ScheduleEmployeeName = ScheduleEmployeeName,
                ScheduleEndTime = ScheduleEndTime,
                ScheduleLocationId = ScheduleLocationId,
                ScheduleLocationName = ScheduleLocationName,
                ScheduleStartTime = ScheduleStartTime,
                ScheduleType = (SessionScheduleType)ScheduleTypeId,
                ScheduleTypeDescription = ScheduleTypeDescription,
                ScheduleUnitId = ScheduleUnitId,
                ScheduleUnitName = ScheduleUnitName,
                SessionScheduleId = SessionScheduleId,
            };
        }
    }
}
