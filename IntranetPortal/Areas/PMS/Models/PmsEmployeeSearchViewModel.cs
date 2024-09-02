using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class PmsEmployeeSearchViewModel:BaseListViewModel
    {
        public int id { get; set; }
        public string sn { get; set; }
        public int? ud { get; set; }
        public List<Employee> EmployeesList { get; set; }
    }
}
