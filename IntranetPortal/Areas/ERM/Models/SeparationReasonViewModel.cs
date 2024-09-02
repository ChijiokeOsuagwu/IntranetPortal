using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class SeparationReasonViewModel:BaseViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(100, ErrorMessage = "Must not exceed 100 charaters.")]
        public string Description { get; set; }
    }
}
