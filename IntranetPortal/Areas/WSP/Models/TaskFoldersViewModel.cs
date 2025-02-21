using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System.Collections.Generic;

namespace IntranetPortal.Areas.WSP.Models
{
    public class TaskFoldersViewModel:BaseViewModel
    {
        public string Id { get; set; }
        public int St { get; set; }
        public int Yr { get; set; }
        public int? Mn { get; set; }
        public List<WorkItemFolder> TaskFolders { get; set; }
        public int NoOfPendingTasks { get; set; }
    }
}
