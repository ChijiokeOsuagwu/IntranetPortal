using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class BirthDayListViewModel : BaseListViewModel
    {
        public int? mm { get; set; }
        public int? dd { get; set; }
        public List<Employee> EmployeesList { get; set; }
    }
}
