using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WksModels
{
    public class Workspace
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool IsMain { get; set; }
        public bool IsExecutiveManagement { get; set; }
        public string OwnerID { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }

        public IEnumerable<ProjectFolder> Folders { get; set; }

    }
}
