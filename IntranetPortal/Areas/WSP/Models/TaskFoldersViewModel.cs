using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System.Collections.Generic;
using System;

namespace IntranetPortal.Areas.WSP.Models
{
    public class TaskFoldersViewModel:BaseViewModel
    {
        public string id { get; set; }
        public int St { get; set; }
        public DateTime? sd { get; set; }
        public DateTime? ed { get; set; }
        public List<WorkItemFolder> TaskFolders { get; set; }
        public long NoOfPendingTasks { get; set; }
    }
}
