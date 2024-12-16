using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class EmployeeListViewModel : BaseListViewModel
    {
        public int? LocationID { get; set; }
        public int? DepartmentID { get; set; }
        public int? UnitID { get; set; }
        public DateTime? TerminalDate { get; set; }
        public string CompanyCode { get; set; }
        public string ListLabel { get; set; }
        public List<Employee> EmployeesList { get; set; }
    }
}
