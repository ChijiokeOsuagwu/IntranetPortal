using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BamsModels
{
    public class DeploymentTeamMember
    {
        public int DeploymentTeamID { get; set; }
        public int AssignmentEventID { get; set; }
        public string AssignmentEventTitle { get; set; }
        public int DeploymentID { get; set; }
        public string DeploymentTitle { get; set; }
        public string TeamMemberID { get; set; }
        public string TeamMemberName { get; set; }
        public string TeamMemberRole { get; set; }
        public string TeamDeploymentStatus { get; set; }
        public string TeamMemberUnit { get; set; }
        public string TeamMemberStation { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
    }
}
