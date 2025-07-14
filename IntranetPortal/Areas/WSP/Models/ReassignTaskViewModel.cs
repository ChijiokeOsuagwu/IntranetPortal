using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class ReassignTaskViewModel:BaseViewModel
    {
        [Required]
        public long OldTaskDelegationId { get; set; }
        public long NewTaskDelegationId { get; set; }

        [Required]
        public long TaskItemId { get; set; }

        [Display(Name = "Task No.: ")]
        public string TaskNumber { get; set; }

        [Display(Name="Task Resolution: ")]
        public string MoreInformation { get; set; }

        [Display(Name = "Task Description: ")]
        public string TaskItemDescription { get; set; }

        [Display(Name = "Progress Status: ")]
        public int ProgressStatusId { get; set; }

        [Display(Name = "Progress Status: ")]
        public string ProgressStatusDescription { get; set; }

        public string TaskOwnerId { get; set; }

        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ExpectedDueTime { get; set; }


        [Required]
        public string DelegatedByEmployeeId { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Delegated By: ")]
        public string DelegatedByEmployeeName { get; set; }
        public string DelegatedToEmployeeId { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Delegated To: ")]
        public string DelegatedToEmployeeName { get; set; }

        [Display(Name="Time Delegated: ")]
        public DateTime? DelegatedTime { get; set; }
        public bool IsReAssigned { get; set; }
        public DateTime? ReassignedTime { get; set; }

    }
}
