using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class EmployeeReportLineListViewModel : BaseListViewModel
    {
        public string EmployeeID { get; set; }
        public string StaffName { get; set; }
        public List<EmployeeReportLine> EmployeeReportLineList { get; set; }
    }
}
