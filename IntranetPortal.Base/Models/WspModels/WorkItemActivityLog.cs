using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class WorkItemActivityLog
    {
        public long Id { get; set; }
        public DateTime? Time { get; set; }
        public string Description { get; set; }
        public string ActivityBy { get; set; }
        public long? TaskItemId { get; set; }
        public long? WorkItemFolderId { get; set; }
        public long? ProjectId { get; set; }
    }
}
