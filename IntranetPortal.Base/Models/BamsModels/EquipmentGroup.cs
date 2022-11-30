using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BamsModels
{
    public class EquipmentGroup
    {
        public int? EquipmentGroupID { get; set; }
        public string EquipmentGroupName { get; set; }
        public string EquipmentGroupDescription { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
    }
}
