using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class TaskFolderNote : EntityNote
    {
        public long TaskFolderId { get; set; }
    }
}
