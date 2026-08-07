using IntranetPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace IntranetPortal.Areas.LVM.Models
{
    public class FlagLeaveViewModel:BaseViewModel
    {
        [Required]
        public long LeavePlanId { get; set; }
        [Required]
        public string FlaggedByEmployeeName { get; set; }
        [Required]
        public string FlagReason { get;set; }
    }
}
