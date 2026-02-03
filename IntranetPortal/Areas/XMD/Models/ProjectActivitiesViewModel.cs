using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.XMD.Models
{
    public class ProjectActivitiesViewModel:BaseViewModel
    {
        public long? DrawerID { get; set; }
        public long ProjectID { get; set; }
        public long TaskItemID { get; set; }
        public List<WorkItemActivityLog> ActivityList { get; set; }
    }
}
