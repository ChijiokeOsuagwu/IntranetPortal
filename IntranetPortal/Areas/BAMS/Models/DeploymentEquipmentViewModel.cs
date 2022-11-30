using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class DeploymentEquipmentViewModel : BaseViewModel
    {
        public int ID { get; set; }
        public int AssignmentEventID { get; set; }
        public string AssignmentEventTitle { get; set; }
        public int DeploymentID { get; set; }
        public string DeploymentTitle { get; set; }
        public string AssetID { get; set; }

        [Required]
        public string AssetName { get; set; }
        public int AssetTypeID { get; set; }
        public string AssetTypeName { get; set; }
        public int? EquipmentUsageID { get; set; }
        public string EquipmentDeploymentStatus { get; set; }
        public string PreviousAvailabilityStatus { get; set; }
        public string PreviousLocation { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }

        public DeploymentEquipment ConvertToDeploymentEquipment()
        {
            return new DeploymentEquipment
            {
                AssetID = AssetID,
                AssetName = AssetName,
                AssetTypeID = AssetTypeID,
                AssetTypeName = AssetTypeName,
                AssignmentEventID = AssignmentEventID,
                AssignmentEventTitle = AssignmentEventTitle,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                DeploymentID = DeploymentID,
                DeploymentTitle = DeploymentTitle,
                EquipmentDeploymentStatus = EquipmentDeploymentStatus,
                EquipmentUsageID = EquipmentUsageID,
                PreviousAvailabilityStatus = PreviousAvailabilityStatus,
                PreviousLocation = PreviousLocation,
                ID = ID,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
            };
        }
    }
}
