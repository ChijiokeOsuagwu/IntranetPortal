using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.XMD.Models
{
    public class EmployeesFoldersReportViewModel:BaseViewModel
    {
        public string id { get; set; }
        public string sn { get; set; }
        public int? gd { get; set; }
        public string LocationGroupName { get; set; }
        public int? ld { get; set; }
        public string LocationName { get; set; }
        public int? dd { get; set; }
        public string DepartmentName { get; set; }
        public int? ud { get; set; }
        public string UnitName { get; set; }
        public DateTime? sd { get; set; }
        public DateTime? ed { get; set; }
        public int vs { get; set; }
        public List<WorkItemFolder> FoldersList { get; set; }
        public List<EmployeeRoll> EmployeesList { get; set; }

    }
}
