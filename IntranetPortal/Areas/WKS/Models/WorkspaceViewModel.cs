using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class WorkspaceViewModel:BaseViewModel
    {
        public int? ID { get; set; }

        [Required(ErrorMessage="Title is required")]
        [MaxLength(60)]
        [Display(Name="Title")]
        public string Title { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
