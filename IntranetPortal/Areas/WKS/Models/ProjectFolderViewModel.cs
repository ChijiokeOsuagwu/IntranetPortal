using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace IntranetPortal.Areas.WKS.Models
{
    public class ProjectFolderViewModel : BaseViewModel
    {
        public int? FolderID { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Title")]
        public string FolderTitle { get; set; }

        [MaxLength(100)]
        [Display(Name = "Description")]
        public string FolderDescription { get; set; }
        public string OwnerID { get; set; }
        public int? WorkspaceID { get; set; }

        [Display(Name = "Workspace")]
        public string WorkspaceTitle { get; set; }

        [Display(Name = "Status")]
        public bool IsArchived { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
