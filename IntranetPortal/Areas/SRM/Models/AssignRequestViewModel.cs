using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Models
{
    public class AssignRequestViewModel:BaseViewModel
    {
        public long IncidentId { get; set; }
        public string ServiceTeamId { get; set; }
        [Required]
        [Display(Name ="Assign To: ")]
        public string AssignedToEmployeeName { get; set; }
        public string AssignedByEmployeeName { get; set; }
        public DateTime? AssignedTime { get; set; }
    }
}
