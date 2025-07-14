using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class ActiveAssignmentsListViewModel:BaseListViewModel
    {
        public string cn { get; set; }
        public DateTime? sd { get; set; }
        public DateTime? ed { get; set; }
        public List<Assignment> AssignmentList { get; set; }
    }
}
