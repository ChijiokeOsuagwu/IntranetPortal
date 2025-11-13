using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AtsModels
{
    public class AssignmentCrewReport
    {
        public long CrewReportId { get; set; }
        public long AssignmentCrewId { get; set; }
        public long AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool IsTeamLead { get; set; }
        public string AttendanceStatus { get; set; }
        public string ArrivalType { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string DepartureType { get; set; }
        public DateTime? DepartureTime { get; set; }
        public bool HasIncidents { get; set; }
        public string IncidenceDetails { get; set; }
        public bool HasFeedback { get; set; }
        public string FeedbackDetails { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }
}
