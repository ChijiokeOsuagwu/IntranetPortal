using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class SelectTaskFolderViewModel:BaseViewModel
    {
        public long TaskItemID { get; set; }
        public string TaskOwnerID { get; set; }
        public string Source { get; set; }
        public List<TaskList> ActiveTaskLists { get; set; }
    }
}
