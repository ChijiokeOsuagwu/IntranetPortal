using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class TaskListViewModel:BaseViewModel
    {
        public long FolderID { get; set; }
        public int? ProgressStatusID { get; set; }
        public string FolderTitle { get; set; }
        public bool FolderIsLocked { get; set; }
        public bool FolderIsArchived { get; set; }
        public string FolderOwnerID { get; set; }
        public string FolderOwnerName { get; set; }
        public bool IsPendingTasks { get; set; }
        public string SourcePage { get; set; }
        public List<TaskItem> TaskItems { get; set; }

    }
}
