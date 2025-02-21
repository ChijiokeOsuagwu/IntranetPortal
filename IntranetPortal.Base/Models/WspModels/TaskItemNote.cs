using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskItemNote : EntityNote
    {
        public long TaskItemId { get; set; }
    }
}
