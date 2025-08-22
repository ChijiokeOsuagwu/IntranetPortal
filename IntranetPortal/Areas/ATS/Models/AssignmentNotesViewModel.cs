using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class AssignmentNotesViewModel:BaseViewModel
    {
        public long AssignmentID { get; set; }
        public string LoggedInEmployeeID { get; set; }
        public string LoggedInEmployeeName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string NewLeaveNote { get; set; }
        public List<AssignmentNote> NoteList { get; set; }

    }
}
