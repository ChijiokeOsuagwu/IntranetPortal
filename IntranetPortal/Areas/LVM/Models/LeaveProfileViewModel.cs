using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveProfileViewModel:BaseViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Code")]
        [MaxLength(4, ErrorMessage = "Code must not exceed 4 characters.")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Name")]
        [MaxLength(65, ErrorMessage = "Name must not exceed 65 characters.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [MaxLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Description { get; set; }

    }
}
