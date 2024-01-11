using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ProjectMoreInfoViewModel:BaseViewModel
    {
        public int ProjectID { get; set; }
        public int FolderID { get; set; }
        public string Instructions { get; set; }
        public string Deliverables { get; set; }
    }
}
