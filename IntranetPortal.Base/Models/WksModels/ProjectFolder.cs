using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WksModels
{
    public class ProjectFolder
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int WorkspaceID { get; set; }
        public string WorkspaceTitle { get; set; }
        public bool InMainWorkspace { get; set; }
        public string OwnerID { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public bool IsArchived { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string DeletedBy { get; set; }

        public Workspace FolderWorkspace { get; set; }
    }
}
