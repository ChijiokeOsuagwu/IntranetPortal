using IntranetPortal.Base.Models.WspModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class WorkItemActivitiesViewModel:BaseViewModel
    {
        public long? FolderID { get; set; }
        public long? TaskID { get; set; }
        public long? ProjectID { get; set; }
        public string SourcePage { get; set; }
        public List<WorkItemActivityLog> ActivityList { get; set; }
    }
}
