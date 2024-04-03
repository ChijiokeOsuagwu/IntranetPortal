using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class CreatePerformanceGoalsViewModel : BaseViewModel
    {
        public int? ReviewHeaderID { get; set; }

        [Required]
        public int ReviewSessionID { get; set; }
        public string ReviewSessionName { get; set; }

        [Required]
        public string AppraiseeID { get; set; }
        public string AppraiseeName { get; set; }

        [Required]
        [Display(Name = "Performance Goal(s)")]
        public string PerformanceGoals { get; set; }

        [Required]
        [Display(Name = "Principal Appraiser (Usually your Line Manager)")]
        public string AppraiserID { get; set; }

        [Display(Name = "Principal Appraiser (Usually your Line Manager)")]
        public string AppraiserName { get; set; }
    }

}
