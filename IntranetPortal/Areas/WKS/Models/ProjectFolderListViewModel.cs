using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ProjectFolderListViewModel : BaseListViewModel
    {
        public string OwnerID { get; set; }
        public int? id { get; set; }
        public List<ProjectFolder> ProjectFolderList { get; set; }
    }
}
