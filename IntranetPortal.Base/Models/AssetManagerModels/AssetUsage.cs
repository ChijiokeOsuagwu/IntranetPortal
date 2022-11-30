using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetUsage
    {
        public int UsageID { get; set; }
        public string AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public DateTime? UsageStartTime { get; set; }
        public DateTime? UsageEndTime { get; set; }
        public string Purpose { get; set; }
        public string UsageDescription { get; set; }
        public string UsageLocation { get; set; }
        public string CheckedOutFromLocation { get; set; }
        public string CheckedOutBy { get; set; }
        public DateTime? CheckedOutTime { get; set; }
        public AssetCondition CheckOutCondition { get; set; }
        public string CheckedOutTo { get; set; }
        public string CheckedOutComment { get; set; }
        public string CheckedInBy { get; set; }
        public DateTime? CheckedInTime { get; set; }
        public AssetCondition CheckedInCondition { get; set; }
        public string CheckedInComment { get; set; }
        public string CheckStatus { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
    }
}
