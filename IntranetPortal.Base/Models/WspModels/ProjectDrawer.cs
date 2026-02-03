using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class ProjectDrawer
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long? WorkspaceId { get; set; }
        public string WorkspaceTitle { get; set; }
        public bool InMainWorkspace { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedTime { get; set; }
        public bool IsReuseable { get; set; }
        public bool IsLocked { get; set; }
    }
}
