using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveProfileEmployeesListViewModel:BaseViewModel
    {
        public int LeaveProfileId { get; set; }
        public List<EmployeeRoll> EmployeeRollsList { get; set; }
    }
}
