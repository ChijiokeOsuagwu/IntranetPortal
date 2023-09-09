using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class LinkProgramViewModel:BaseViewModel
    {
        public long TaskItemID { get; set; }
        public int TaskListID { get; set; }

        [Required]
        [Display(Name="Program Time")]
        public DateTime? ProgramTime { get; set; }

        [Display(Name = "Programme")]
        public string ProgramName { get; set; }

        [Required]
        [Display(Name = "Programme")]
        public string ProgramCode { get; set; }

        public string Source { get; set; }
    }
}
