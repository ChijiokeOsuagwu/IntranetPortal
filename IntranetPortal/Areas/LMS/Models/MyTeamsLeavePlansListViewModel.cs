using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class MyTeamsLeavePlansListViewModel:BaseViewModel
    {
        public int yr { get; set; }
        public int mm { get; set; }
        public string ed { get; set; }
        public string st { get; set; }
        public string td { get; set; }
        public List<EmployeeLeave> LeavePlanList { get; set; }
    }
}
