using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeaveNoteListViewModel:BaseListViewModel
    {
        public long LeaveID { get; set; }
        public int LeaveYear { get; set; }
        public string LoggedInEmployeeID { get; set; }
        public string LoggedInEmployeeName { get; set; }
        public string ApplicantID { get; set; }
        public string ApplicantName { get; set; }
        public string NewLeaveNote { get; set; }
        public string SourcePage { get; set; }
        public List<LeaveNote> LeaveNoteList { get; set; }
    }
}
