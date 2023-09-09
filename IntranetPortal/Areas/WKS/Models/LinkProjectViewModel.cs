using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class LinkProjectViewModel:BaseViewModel
    {
        public long TaskItemID { get; set; }
        public int TaskListID { get; set; }

        [Required]
        [Display(Name = "Project Code")]
        public string ProjectNumber { get; set; }

        public string Source { get; set; }
    }
}
