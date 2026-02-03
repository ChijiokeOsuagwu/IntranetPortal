using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.XMD.Models
{
    public class ProjectNotesViewModel:BaseViewModel
    {
        public int NoteType { get; set; }
        public long? ProjectID { get; set; }
        public long? TaskItemID { get; set; }
        public string LoggedInEmployeeID { get; set; }
        public string LoggedInEmployeeName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string NewLeaveNote { get; set; }
        public List<WorkItemNote> NoteList { get; set; }
    }
}
