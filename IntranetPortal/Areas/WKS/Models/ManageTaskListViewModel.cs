using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ManageTaskListViewModel:BaseViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name="Folder Name")]
        [MaxLength(150)]
        public string Name { get; set; }

        [Display(Name = "Folder Description")]
        [MaxLength(250)]
        public string Description { get; set; }

        public bool IsArchived { get; set; }

        [Required]
        [Display(Name="Owner")]
        public string OwnerId { get; set; }

        [Display(Name = "Owner")]
        [MaxLength(150)]
        public string OwnerName { get; set; }

        [Display(Name="Created On")]
        public DateTime? CreatedTime { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string DeletedBy { get; set; }
        public bool IsNew { get; set; }
        public string Source { get; set; }
    }
}
