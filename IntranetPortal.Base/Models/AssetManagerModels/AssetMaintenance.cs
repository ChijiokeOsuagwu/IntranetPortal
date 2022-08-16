using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetMaintenance
    {
        public int? AssetMaintenanceID { get; set; }
        public string AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetCategoryID { get; set; }
        public string AssetCategoryName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string MaintenanceTitle { get; set; }
        public string IssueDescription { get; set; }
        public string SolutionDescription { get; set; }
        public string PreviousCondition { get; set; }
        public string FinalCondition { get; set; }
        public string MaintainedBy { get; set; }
        public string SupervisedBy { get; set; }
        public string Comments { get; set; }
        public string LoggedBy { get; set; }
        public DateTime? LoggedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }

    }
}
