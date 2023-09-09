using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WksModels
{
    public class TaskTimelineChange
    {
        public long? TimelineChangeId { get; set; }
        public long TaskItemId { get; set; }
        public int TaskListId { get; set; }
        public DateTime? PreviousStartDate { get; set; }
        public DateTime? PreviousEndDate { get; set; }
        public DateTime? NewStartDate { get; set; }
        public DateTime? NewEndDate { get; set; }
        public decimal DifferentInDays { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
    }
}
