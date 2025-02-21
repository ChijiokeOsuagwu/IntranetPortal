using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class Workspace
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsMain { get; set; }
        public bool IsExecutiveManagement { get; set; }
        public string OwnerID { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedBy { get; set; }
   
        public List<WorkItemFolder> Folders { get; set; }

    }
}
