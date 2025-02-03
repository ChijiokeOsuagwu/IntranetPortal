using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class ReturnLeaveViewModel:BaseViewModel
    {
        [Required]
        public long LeaveId { get; set; }
        public string ApproverName { get; set; }
        [Required]
        [Display(Name="Drop a Note Here:")]
        public string ReturnNote { get; set; }
        public string DocumentType { get; set; }
    }
}
