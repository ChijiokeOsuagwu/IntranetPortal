using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class WorkItemFolder
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int FolderTypeId { get; set; }
        public WorkItemFolderType FolderType { get; set; }
        public string FolderTypeDescription { get; set; }
        public long? WorkspaceId { get; set; }
        public string WorkspaceTitle { get; set; }
        public bool InMainWorkspace { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedTime { get; set; }
        public bool IsReuseable { get; set; }
        public bool IsLocked { get; set; }
        public Workspace FolderWorkSpace { get; set; }
    }
}
