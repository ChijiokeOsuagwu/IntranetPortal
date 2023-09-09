using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class UpdateProgressViewModel:BaseViewModel
    {
        public long TaskItemID { get; set; }
        public int TaskListID { get; set; }
        public int CurrentProgressStatusID { get; set; }
        public string CurrentProgressStatusDescription { get; set; }

        [Required]
        [Display(Name="New Progress Status:")]
        public int? NewProgressStatusID { get; set; }
        public string Source { get; set; }
    }
}
