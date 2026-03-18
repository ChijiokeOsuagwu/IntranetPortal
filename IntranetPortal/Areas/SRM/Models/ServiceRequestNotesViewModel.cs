using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Models
{
    public class ServiceRequestNotesViewModel:BaseViewModel
    {
        public long? ServiceIncidentId { get; set; }
        public string LoggedInEmployeeID { get; set; }
        public string LoggedInEmployeeName { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string NewLeaveNote { get; set; }
        public List<ServiceRequestNote> NoteList { get; set; }
    }
}
