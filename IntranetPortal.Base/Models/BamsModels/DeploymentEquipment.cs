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
        public string AssetDescription { get; set; }
        public string EquipmentDeploymentStatus { get; set; }
    }
}
