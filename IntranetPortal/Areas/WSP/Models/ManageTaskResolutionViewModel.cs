using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class ManageTaskResolutionViewModel:BaseViewModel
    {
        [Required]
        public long TaskItemID { get; set; }
        public string TaskItemDescription { get; set; }
        [Required]
        public long TaskFolderID { get; set; }
        public string TaskFolderName { get; set; }
        public string TaskOwnerID { get; set; }
        public string TaskOwnerName { get; set; }

        public string TaskResolution { get; set; }
    }
}
