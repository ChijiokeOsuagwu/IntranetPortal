using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BamsModels
{
    public class DeploymentEquipment
    {
        public int ID { get; set; }
        public int AssignmentEventID { get; set; }
        public string AssignmentEventTitle { get; set; }
        public int DeploymentID { get; set; }
        public string DeploymentTitle { get; set; }
        public string AssetID { get; set; }
        public string AssetName { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public string EquipmentDeploymentStatus { get; set; }
        public int? EquipmentUsageID { get; set; }
        public string PreviousAvailabilityStatus { get; set; }
        public string PreviousLocation { get; set; }
        public string ModifiedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
    }
}
