using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class EmployeeSeparationViewModel : BaseViewModel
    {
        public DateTime? sd { get; set; }
        public DateTime? ed { get; set; }
        public int RowCount { get; set; }
        public List<EmployeeSeparation> EmployeeSeparationList { get; set; }
    }
}
