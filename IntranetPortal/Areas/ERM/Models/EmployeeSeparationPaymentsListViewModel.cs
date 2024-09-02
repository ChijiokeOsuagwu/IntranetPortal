using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class EmployeeSeparationPaymentsListViewModel:BaseListViewModel
    {
        public string EmployeeId { get; set; }
        public string  EmployeeName { get; set; }
        public int EmployeeSeparationId { get; set; }
        public List<EmployeeSeparationPayments> EmployeeSeparationPayments { get; set; }
    }
}
