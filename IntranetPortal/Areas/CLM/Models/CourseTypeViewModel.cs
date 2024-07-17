using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.CLM.Models
{
    public class CourseTypeViewModel:BaseViewModel
    {
        public int CourseTypeId { get; set; } = 0;

        [Required]
        [MaxLength(60)]
        [Display(Name = "Description")]
        public string CourseTypeDescription { get; set; }
    }
}
