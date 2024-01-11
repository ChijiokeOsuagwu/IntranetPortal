using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ManageGradeProfileViewModel : BaseViewModel
    {
        public int? GradeHeaderId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Profile Name")]
        public string GradeHeaderName { get; set; }

        [MaxLength(250)]
        [Display(Name = "Profile Description")]
        public string GradeHeaderDescription { get; set; }
    }
}
