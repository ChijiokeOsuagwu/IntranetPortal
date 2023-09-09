using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class TaskListNotesViewModel:BaseViewModel
    {
        public int TaskListID { get; set; }
        public string Source { get; set; }
        public List<TaskListNote> TaskListNotes { get; set; }
    }
}
