using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class SeparationOutstandingListViewModel:BaseListViewModel
    {
        public int id { get; set; }
        public string EmployeeName { get; set; }
        public List<EmployeeSeparationOutstanding> SeparationOutstandingList { get; set; }
    }
}
