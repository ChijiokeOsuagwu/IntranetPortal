using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class WorkspaceListViewModel:BaseListViewModel
    {
        public string OwnerID { get; set; }
        public List<Workspace> WorkspaceList { get; set; }
    }
}
