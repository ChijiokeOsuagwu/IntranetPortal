using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class AssignmentCrewListViewModel:BaseViewModel
    {
        public long AssignmentID { get; set; }
        public string AssignmentTitle { get; set; }
        public string AssignmentState { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string NewMemberName { get; set; }
        public string NewMemberRole1 { get; set; }
        public string NewMemberRole2 { get; set; }
        public string NewMemberRole3 { get; set; }
        public List<AssignmentCrewMember> CrewMemberList { get; set; }
    }
}
