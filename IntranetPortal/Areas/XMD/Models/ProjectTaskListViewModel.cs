using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.XMD.Models
{
    public class ProjectTaskListViewModel:BaseViewModel
    {
        public long ProjectID { get; set; }
        public string ProjectNo { get; set; }
        public int? ProgressStatusID { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectOwnerID { get; set; }
        public string ProjectOwnerName { get; set; }
        public List<DelegatedTaskItem> TaskItems { get; set; }
        public List<WorkItemFolder> TaskFolderList { get; set; }

    }
}
