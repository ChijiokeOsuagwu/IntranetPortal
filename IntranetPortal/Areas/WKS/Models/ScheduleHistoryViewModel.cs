using IntranetPortal.Base.Models.WksModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WKS.Models
{
    public class ScheduleHistoryViewModel:BaseViewModel
    {
        public long TaskItemID { get; set; }
        public int TaskListID { get; set; }
        public List<TaskTimelineChange> TaskTimelineChanges { get; set; }
    }
}
