using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class SeparationReasonListViewModel:BaseListViewModel
    {
        public List<EmployeeSeparationReason> SeparationReasonList { get; set; }
    }
}
