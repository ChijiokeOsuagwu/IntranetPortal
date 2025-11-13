using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System.Collections.Generic;

namespace IntranetPortal.Areas.ATS.Models
{
    public class CrewReportsListViewModel:BaseListViewModel
    {
        public long AssignmentID { get; set; }
        public string AssignmentTitle { get; set; }
        public List<AssignmentCrewReport> CrewReportList { get; set; }
    }
}
