using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class AssignmentDeploymentViewModel : BaseViewModel
    {
        public int DeploymentID { get; set; }

        [Required]
        [Display(Name ="Batch:")]
        public string DeploymentTitle { get; set; }

        [Required]
        public int AssignmentEventID { get; set; }
        public string AssignmentEventTitle { get; set; }
        public string AssignmentEventDescription { get; set; }

        [Display(Name ="Departure Time:")]
        [DataType(DataType.DateTime)]
        public DateTime? DepartureTime { get; set; }

        [Display(Name = "Expected Return Time:")]
        [DataType(DataType.DateTime)]
        public DateTime? ArrivalTime { get; set; }

        [Display(Name ="Team Lead:")]
        public string TeamLeadName { get; set; }

        [Display(Name ="Contact Phone(s):")]
        public string TeamLeadPhone { get; set; }

        [Display(Name ="Deployment Status:")]
        [Required]
        public int? StatusID { get; set; }

        [Display(Name = "Deployment Status:")]
        public string StatusDescription { get; set; }

        [Display(Name ="Deployment Station")]
        [Required]
        public int StationID { get; set; }

        [Display(Name = "Deployment Station")]
        public string StationName { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }

        public AssignmentDeployment ConvertToAssignmentDeployment()
        {
            return new AssignmentDeployment { 
                ArrivalTime = ArrivalTime,
                AssignmentEventDescription = AssignmentEventDescription,
                AssignmentEventID = AssignmentEventID,
                AssignmentEventTitle = AssignmentEventTitle,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                DepartureTime = DepartureTime,
                DeploymentID = DeploymentID,
                DeploymentTitle = DeploymentTitle,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                StationID = StationID,
                StationName = StationName,
                StatusDescription = StatusDescription,
                StatusID = StatusID,
                TeamLeadName = TeamLeadName,
                TeamLeadPhone = TeamLeadPhone,
            };
        }
    }
}
