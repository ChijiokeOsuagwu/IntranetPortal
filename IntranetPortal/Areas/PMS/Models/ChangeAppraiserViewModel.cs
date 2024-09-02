using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ChangeAppraiserViewModel:BaseViewModel
    {
        [Required]
        [Display(Name ="Appraisal Session")]
        public int ReviewSessionID { get; set; }
        public AppraiserChangeType ChangeType { get; set; }

        public string AppraiseeID { get; set; }

        [Display(Name ="Appraisee")]
        public string AppraiseeName { get; set; }

        [Display(Name = "Unit")]
        public int? UnitID { get; set; }

        [Required]
        [Display(Name = "New Principal Appraiser")]
        public string sn { get; set; }
    }
}
