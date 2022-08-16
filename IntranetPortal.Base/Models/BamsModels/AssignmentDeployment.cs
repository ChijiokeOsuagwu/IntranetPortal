using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BamsModels
{
    public class AssignmentDeployment
    {
        public int DeploymentID { get; set; }
        public string DeploymentTitle { get; set; }
        public int AssignmentEventID { get; set; }
        public string AssignmentEventTitle { get; set; }
        public string AssignmentEventDescription { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string TeamLeadName { get; set; }
        public string TeamLeadPhone { get; set; }
        public string ProgressDescription { get; set; }
        public int? StatusID { get; set; }
        public string StatusDescription { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
    }
}
