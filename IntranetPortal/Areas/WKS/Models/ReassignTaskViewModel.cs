using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ReassignTaskViewModel:BaseViewModel
    {
        public long TaskItemID { get; set; }

        public int OldTaskListID { get; set; }
        public int NewTaskListID { get; set; }
        public string Source { get; set; }
        public string OldTaskOwnerID { get; set; }

        [Display(Name="From:")]
        public string OldTaskOwnerName { get; set; }

        [Required]
        [Display(Name = "To:")]
        public string NewTaskOwnerID { get; set; }
        public string NewTaskOwnerName { get; set; }
    }
}
