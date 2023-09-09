using IntranetPortal.Base.Models.WksModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public abstract class EntityActivityHistory
    {
        public long ActivityHistoryId { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime? ActivityTime { get; set; }
        public string ActivityBy { get; set; }
        public object EntityId { get; set; }

        public TaskItemActivityHistory ConvertToTaskItemActivityHistory()
        {
            return new TaskItemActivityHistory
            {
                ActivityBy = ActivityBy,
                ActivityDescription = ActivityDescription,
                ActivityHistoryId = ActivityHistoryId,
                ActivityTime = ActivityTime,
                EntityId = EntityId,
                TaskItemId = (long?)EntityId,
            };
        }
        public TaskListActivityHistory ConvertToTaskListActivityHistory()
        {
            return new TaskListActivityHistory
            {
                ActivityBy = ActivityBy,
                ActivityDescription = ActivityDescription,
                ActivityHistoryId = ActivityHistoryId,
                ActivityTime = ActivityTime,
                EntityId = EntityId,
                TaskListId = (int?)EntityId,
            };
        }
    }
}
