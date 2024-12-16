using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeaveProfileDetailViewModel:BaseViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ProfileId { get; set; }

        [Display(Name ="Profile Name")]
        public string ProfileName { get; set; }

        [Required]
        [Display(Name ="Leave Type")]
        public string LeaveTypeCode { get; set; }

        [Display(Name ="Leave Type")]
        public string LeaveTypeName { get; set; }

        [Required]
        [Display(Name ="Duration")]
        public int Duration { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int DurationTypeId { get; set; }

        [Display(Name = "Type")]
        public string DurationTypeDescription { get; set; }

        [Required]
        public bool IsYearly { get; set; }

        [Required]
        public bool CanBeCarriedOver { get; set; }

        [Display(Name = "Carry Forward Up To: ")]
        public int? CarryOverEndMonth { get; set; }

        [Display(Name = "Carry Forward Up To: ")]
        public string CarryOverEndMonthName { get; set; }

        [Required]
        public bool CanBeMonetized { get; set; }

    }
}
