using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class EmployeeOptionsViewModel:BaseViewModel
    {
        [Required]
        public string EmployeeId { get; set; }
        [Display(Name ="Full Name")]
        public string EmployeeFullName { get; set; }

        [Display(Name="Leave Profile")]
        public int LeaveProfileId { get; set; }
        public string LeaveProfileName { get; set; }
    }
}
