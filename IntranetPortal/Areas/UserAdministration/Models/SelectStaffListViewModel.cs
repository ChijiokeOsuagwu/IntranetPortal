using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class SelectStaffListViewModel: BaseListViewModel
    {
        public string ss { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
