using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class AssetEquipmentGroupViewModel : BaseViewModel
    {
        public int? AssetEquipmentGroupID { get; set; }
        public string AssetID { get; set; }
        public string AssetNo { get; set; }
        [Required]
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public int? AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public AssetCondition ConditionStatus { get; set; }
        public string CurrentLocation { get; set; }
        [Required]
        public int EquipmentGroupID { get; set; }
        public string EquipmentGroupName { get; set; }
        public string EquipmentGroupDescription { get; set; }
        public string AddedBy { get; set; }
        public string AddedTime { get; set; }

        public AssetEquipmentGroup ConvertToAssetEquipmentGroup()
        {
            return new AssetEquipmentGroup
            {
                AddedBy = AddedBy,
                AddedTime = AddedTime,
                AssetDescription = AssetDescription,
                AssetEquipmentGroupID = AssetEquipmentGroupID,
                AssetID = AssetID,
                AssetName = AssetName,
                AssetNo = AssetNo,
                AssetTypeID = AssetTypeID,
                AssetTypeName = AssetTypeName,
                ConditionStatus = ConditionStatus,
                CurrentLocation = CurrentLocation,
                EquipmentGroupDescription = EquipmentGroupDescription,
                EquipmentGroupID = EquipmentGroupID,
                EquipmentGroupName = EquipmentGroupName,
            };
        }
    }
}
