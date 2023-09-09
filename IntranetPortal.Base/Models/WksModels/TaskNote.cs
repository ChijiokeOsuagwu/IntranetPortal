using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WksModels
{
    public class TaskNote:EntityNote
    {
        public long TaskItemId { get; set; }
    }
}
