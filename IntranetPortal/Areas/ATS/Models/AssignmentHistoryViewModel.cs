using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class AssignmentHistoryViewModel:BaseViewModel
    {
        public long? AssignmentID { get; set; }
        public List<AssignmentHistory> ActivityList { get; set; }

    }
}
