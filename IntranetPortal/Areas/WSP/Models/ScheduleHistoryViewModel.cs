using IntranetPortal.Base.Models.WspModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.WSP.Models
{
    public class ScheduleHistoryViewModel
    {
        public long TaskItemID { get; set; }
        public long TaskFolderID { get; set; }
        public List<TaskTimelineChange> TaskTimelineChanges { get; set; }
    }
}
