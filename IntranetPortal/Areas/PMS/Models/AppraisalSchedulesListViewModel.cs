using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class AppraisalSchedulesListViewModel : BaseViewModel
    {
        [Required]
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }

        [Display(Name = "Activity")]
        public int? ActivityTypeId { get; set; }

        [Display(Name = "Schedule For ")]
        public int? ScheduleTypeId { get; set; }

        [Display(Name = "Starting From")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ending On")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Unit")]
        public int? UnitId { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }
        public List<SessionSchedule> SessionScheduleList { get; set; }
    }

}
