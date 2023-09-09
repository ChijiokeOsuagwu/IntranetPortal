using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class TaskBoardViewModel:BaseViewModel
    {
        public int id { get; set; }
        public int? ps { get; set; }
        public int? dy { get; set; }
        public int? dm { get; set; }
        public bool TaskListIsLocked { get; set; }
        public bool TaskListIsArchived { get; set; }
        public string TaskListOwnerID { get; set; }
        public List<TaskItem> TaskItems { get; set; }
        public string Source { get; set; }
    }
}
