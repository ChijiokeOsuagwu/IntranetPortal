using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class AppraisalNonParticipantsViewModel:BaseListViewModel
    {
        public int id { get; set; } = 0;
        public int? ld { get; set; } = 0;
        public int? ud { get; set; } = 0;
        public int RecordCount { get; set; } = 0;
        public string ReviewSessionDescription { get; set; }
        public List<Employee> EmployeesList { get; set; }
    }
}
