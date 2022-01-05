using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.GlobalSettings.Models
{
    public class DepartmentListViewModel : BaseListViewModel
    {
        public List<Department> DepartmentList { get; set; }
    }
}
