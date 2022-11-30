using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BamsModels
{
    public class AssignmentExtension
    {
        public int? AssignmentExtensionID { get; set; }
        public string ExtensionType { get; set; }
        public int? AssignmentEventID { get; set; }
        public string AssignmentEventTitle { get; set; }
        public string ExtensionReason { get; set; }
        public DateTime? FromTime { get; set; }
        public string FromTimeFormatted { get; set; }
        public DateTime? ToTime { get; set; }
        public string ToTimeFormatted { get; set; }
        public int StatusID { get; set; }
        public string StatusDescription { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
    }
}
