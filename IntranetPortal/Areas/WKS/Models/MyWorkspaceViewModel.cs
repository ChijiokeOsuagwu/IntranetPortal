using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class MyWorkspaceViewModel : BaseListViewModel
    {
        public string OwnerID { get; set; }
        public int WorkspaceID { get; set; }
        public Workspace OpenWorkspace { get; set; }
        public bool IncludeArchivedFolders { get; set; }
        public List<Workspace> WorkspaceList { get; set; }
        public List<ProjectFolder> FolderList { get; set; }
    }
}
