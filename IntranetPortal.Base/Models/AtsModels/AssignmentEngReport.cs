using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AtsModels
{
    public class AssignmentEngReport
    {
        public long EngReportId { get; set; }
        public long AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int AudioQuality { get; set; }
        public string AudioQualityDescription { get; set; }
        public int VideoQuality { get; set; }
        public string VideoQualityDescription { get; set; }
        public bool ScriptIsAvailable { get; set; }
        public bool MaterialsAreAvailable { get; set; }
        public bool ReporterIsAvailable { get; set; }
        public DateTime? ReporterArrivalTime { get; set; }
        public DateTime? EditingStartTime { get; set; }
        public DateTime? EditingEndTime { get; set; }
        public string EditingStatus { get; set; }
        public string Feedback { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
}
