using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetMovement
    {
        public int? AssetMovementID { get; set; }
        public string AssetID { get; set; }
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetCategoryID { get; set; }
        public string AssetCategoryName { get; set; }
        public DateTime? MovedOn { get; set; }
        public int? MovedFromLocationID { get; set; }
        public string MovedFromLocationName { get; set; }
        public int? MovedToLocationID { get; set; }
        public string MovedToLocationName { get; set; }
        public int? MovedToBinLocationID { get; set; }
        public string MovedToBinLocationName { get; set; }
        public string MovementPurpose { get; set; }
        public AssetCondition AssetConditionStatus { get; set; }
        public string AssetConditionDescription { get; set; }
        public string SupervisedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Comments { get; set; }
        public string LoggedBy { get; set; }
        public DateTime? LoggedTime { get; set; }
        public string MovementStatus { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
    }
}
