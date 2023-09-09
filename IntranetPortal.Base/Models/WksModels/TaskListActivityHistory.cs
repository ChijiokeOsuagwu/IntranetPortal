using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WksModels
{
    public class TaskListActivityHistory:EntityActivityHistory
    {
        public int? TaskListId { get; set; }
    }
}
