using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetIncident
    {
        public int AssetIncidentID { get; set; }
        public string AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetCategoryID { get; set; }
        public string AssetCategoryName { get; set; }
        public DateTime? IncidentTime { get; set; }
        public string IncidentTitle { get; set; }
        public string IncidentDescription { get; set; }
        public string AssetCondition { get; set; }
        public string ActionTaken { get; set; }
        public string Recommendation { get; set; }
        public string   Comments { get; set; }
        public string LoggedBy { get; set; }
        public DateTime? LoggedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
    }
}
