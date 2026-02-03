using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class Project
    {
        public long ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectDetails { get; set; }
        public string ProjectOwnerId { get; set; }
        public string ProjectOwnerName { get; set; }
        public int ProgressStatusId { get; set; }
        public string ProgressStatusDescription { get; set; }
        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ExpectedEndTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public long? ProjectDrawerId { get; set; }
        public string ProjectDrawerTitle { get; set; }
        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public int? ProjectTypeId { get; set; }
        public string ProjectTypeName { get; set; }
        public long? MasterProjectId { get; set; }
        public string MasterProjectTitle { get; set; }
        public bool IsExecutiveManagementProject { get; set; }
        public bool IsClosed { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedTime { get; set; }

        public string ModifiedBy { get; set; }
    }
}
