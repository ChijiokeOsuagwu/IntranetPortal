using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class AssignmentEventTypesListViewModel:BaseListViewModel
    {
        public List<AssignmentEventType> AssignmentEventTypeList { get; set; }
    }
}
