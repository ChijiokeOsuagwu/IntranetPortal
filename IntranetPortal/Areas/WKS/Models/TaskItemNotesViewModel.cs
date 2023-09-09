using IntranetPortal.Base.Models.WksModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class TaskItemNotesViewModel
    {
        public long TaskItemID { get; set; }
        public int TaskListID { get; set; }
        public string Source { get; set; }
        public List<TaskNote> TaskItemNotes { get; set; }
    }
}
