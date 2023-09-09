using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class SubmitTaskListViewModel:BaseViewModel
    {
        public int TaskListID { get; set; }
        [Display(Name = "From:")]
        public string FromEmployeeID { get; set; }
        [Display(Name = "From:")]
        public string FromEmployeeName { get; set; }
        [Display(Name = "Submit To:")]
        public string ToEmployeeID { get; set; }

        [Display(Name = "Submit to:")]
        [Required]
        public string ToEmployeeName { get; set; }

        [Display(Name = "Submit for:")]
        [Required]
        public int SubmissionTypeID { get; set; }

        [Display(Name = "Comment:")]
        public string Comment { get; set; }

    }
}
