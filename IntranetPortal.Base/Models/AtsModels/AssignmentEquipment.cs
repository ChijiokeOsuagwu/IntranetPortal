using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AtsModels
{
    public class AssignmentEquipment
    {
        public long AssignmentEquipmentId { get; set; }
        public long AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public int AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetGroupId { get; set; }
        public string AssetGroupName { get; set; }
        public int AssetClassId { get; set; }
        public string AssetClassName { get; set; }
        public int AssetCategoryId { get; set; }
        public string AssetCategoryName { get; set; }
        public string AssignedToEmployeeId { get; set; }
        public string AssignedToEmployeeName { get; set; }
        public string AssignedByEmployeeId { get; set; }
        public string AssignedByEmployeeName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
}
