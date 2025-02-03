using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeavePlanListViewModel:BaseListViewModel
    {
        public int yr { get; set; }
        public string nm { get; set; }
        public string ei { get; set; }
        public List<EmployeeLeave> LeavePlanList { get; set; }
        public List<EmployeeLeave> LeaveRequestList { get; set; }
    }
}
