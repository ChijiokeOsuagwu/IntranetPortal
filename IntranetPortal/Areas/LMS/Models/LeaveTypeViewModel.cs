using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeaveTypeViewModel:BaseViewModel
    {
        [Required]
        [MaxLength(5, ErrorMessage ="Code must not exceed 5 characters.")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Name")]
        [MaxLength(65, ErrorMessage = "Code must not exceed 65 characters.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
