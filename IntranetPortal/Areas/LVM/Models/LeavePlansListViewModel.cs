using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeavePlansListViewModel:BaseListViewModel
    {
        public int? l { get; set; }
        public int? u { get; set; }
        public int? y { get; set; }
        public int? m { get; set; }
        public string n { get; set; }
        public List<LeavePlan> LeavePlanList { get; set; }
    }
}
