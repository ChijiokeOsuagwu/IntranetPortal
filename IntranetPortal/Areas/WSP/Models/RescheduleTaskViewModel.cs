using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class RescheduleTaskViewModel:BaseViewModel
    {
        [Required]
        public long TaskItemID { get; set; }

        [Required]
        public long TaskFolderID { get; set; }

        public DateTime? CurrentStartDate { get; set; }

        public DateTime? CurrentEndDate { get; set; }

        [Display(Name = "Current Start Date")]
        public string CurrentStartDateDescription { get; set; }

        [Display(Name = "Current Start Date")]
        public string CurrentEndDateDescription { get; set; }

        [Display(Name = "New Start Date")]
        public DateTime? NewStartDate { get; set; }

        [Display(Name = "New End Date")]
        public DateTime? NewEndDate { get; set; }

        [Display(Name = "New End Date")]
        public string NewStartDateDescription { get; set; }

        [Display(Name = "New End Date")]
        public string NewEndDateDescription { get; set; }
        public string Source { get; set; }

    }
}
