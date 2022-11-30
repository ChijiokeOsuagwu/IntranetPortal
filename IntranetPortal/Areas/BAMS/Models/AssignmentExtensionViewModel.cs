using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class AssignmentExtensionViewModel : BaseViewModel
    {
        public int? AssignmentExtensionID { get; set; }

        [Required]
        public int AssignmentEventID { get; set; }

        [Display(Name="Event Title")]
        public string AssignmentEventTitle { get; set; }

        [Display(Name = "Extend From:")]
        [DataType(DataType.DateTime)]
        public DateTime? FromTime { get; set; }

        [Display(Name = "Extend To:")]
        [DataType(DataType.DateTime)]
        public DateTime? ToTime { get; set; }

        [Display(Name = "Type:")]
        public string ExtensionType { get; set; }

        [Display(Name = "Reason:")]
        public string ExtensionReason { get; set; }

        [Display(Name = "Extend From:")]
        public string FromTimeFormatted { get; set; }

        [Display(Name = "Extend To:")]
        public string ToTimeFormatted { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }

        public AssignmentExtension ConvertToAssignmentExtension()
        {
            return new AssignmentExtension
            {
                AssignmentExtensionID = AssignmentExtensionID,
                FromTime = FromTime,
                ToTime = ToTime,
                AssignmentEventID = AssignmentEventID,
                AssignmentEventTitle = AssignmentEventTitle,
                ExtensionType = ExtensionType,
                ExtensionReason = ExtensionReason,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
            };
        }

    }
}
