using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class DeploymentTeamMemberViewModel : BaseViewModel
    {
        public int DeploymentTeamID { get; set; }
        public int AssignmentEventID { get; set; }
        public string AssignmentEventTitle { get; set; }
        public int DeploymentID { get; set; }
        public string DeploymentTitle { get; set; }
        public string TeamMemberID { get; set; }

        [Required]
        public string TeamMemberName { get; set; }

        [Required]
        public string TeamMemberRole { get; set; }
        public string TeamDeploymentStatus { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }

        public DeploymentTeamMember ConvertToDeploymentTeamMember()
        {
            return new DeploymentTeamMember { 
                AssignmentEventID = AssignmentEventID,
                AssignmentEventTitle = AssignmentEventTitle,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                DeploymentID = DeploymentID,
                DeploymentTeamID = DeploymentTeamID,
                DeploymentTitle = DeploymentTitle,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                TeamDeploymentStatus = TeamDeploymentStatus,
                TeamMemberID = TeamMemberID,
                TeamMemberName = TeamMemberName,
                TeamMemberRole = TeamMemberRole,
            };
        }
    }
}
