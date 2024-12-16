using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeaveTypesListViewModel:BaseListViewModel
    {
        public List<LeaveType> LeaveTypesList { get; set; }
    }
}
