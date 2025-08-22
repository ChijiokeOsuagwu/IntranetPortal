using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PartnerServices.Models
{
    public class ManageSectorViewModel:BaseViewModel
    {
        public int? IndustrySectorID { get; set; }
        [Required]
        [MaxLength(150)]
        [Display(Name="Sector Name")]
        public string IndustrySectorName { get; set; }
    }
}
