using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class LocationGroupViewModel:BaseViewModel
    {
        public int LocationGroupId { get; set; }
        [Display(Name="Name")]
        [MaxLength(100)]
        [Required]
        public string LocationGroupName { get; set; }
    }
}
