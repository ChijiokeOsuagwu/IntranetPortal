using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BamsModels
{
    public class AssetEquipmentGroup
    {
        public int? AssetEquipmentGroupID { get; set; }
        public string AssetID { get; set; }
        public string AssetNo { get; set; }
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int? AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public AssetCondition ConditionStatus { get; set; }
        public string CurrentLocation { get; set; }
        public int? EquipmentGroupID { get; set; }
        public string EquipmentGroupName { get; set; }
        public string EquipmentGroupDescription { get; set; }
        public string AddedBy { get; set; }
        public string AddedTime { get; set; }
    }
}
