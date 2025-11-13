using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IntranetPortal.Areas.ATS.Models
{
    public class AddParticipationReportViewModel:BaseViewModel
    {
        public long? CrewReportId { get; set; }
        public long AssignmentCrewId { get; set; }
        public long AssignmentId { get; set; }
        public string AssignmentNumber { get; set; }
        public string AssignmentTitle { get; set; }
        public string CustomerName { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public bool IsTeamLead { get; set; }
        [Required]
        public string AttendanceStatus { get; set; }
        public string ArrivalType { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public int? ArrivalHour { get; set; }
        public int? ArrivalMinute { get; set; }
        public string ArrivalAmPm { get; set; }
        public string DepartureType { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? DepartureTime { get; set; }
        public int? DepartureHour { get; set; }
        public int? DepartureMinute { get; set; }
        public string DepartureAmPm { get; set; }
        public bool HasIncidents { get; set; }
        public string IncidenceDetails { get; set; }
        public bool HasFeedback { get; set; }
        public string FeedbackDetails { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }

        public AssignmentCrewReport Convert()
        {
            if (ArrivalAmPm == "PM" && ArrivalHour < 12) { ArrivalHour = ArrivalHour + 12; }
            if (DepartureAmPm == "PM" && DepartureHour < 12) { DepartureHour = DepartureHour + 12; }
            if (ArrivalAmPm == "AM" && ArrivalHour == 12) { ArrivalHour = 0; }
            if (DepartureAmPm == "AM" && DepartureHour == 12) { DepartureHour = 0; }

            return new AssignmentCrewReport
            {
                ArrivalTime = new DateTime(ArrivalDate.Value.Year, ArrivalDate.Value.Month, ArrivalDate.Value.Day, ArrivalHour ?? 0, ArrivalMinute ?? 0, 0),
                DepartureTime = new DateTime(DepartureDate.Value.Year, DepartureDate.Value.Month, DepartureDate.Value.Day, DepartureHour ?? 0, DepartureMinute ?? 0, 0),
                ArrivalType = ArrivalType,
                AssignmentCrewId = AssignmentCrewId,
                AssignmentId = AssignmentId,
                AssignmentTitle = AssignmentTitle,
                AttendanceStatus = AttendanceStatus,
                CrewReportId = CrewReportId ?? 0,
                DepartureType = DepartureType,
                EmployeeId = EmployeeId,
                EmployeeName = EmployeeName,
                FeedbackDetails = FeedbackDetails,
                IncidenceDetails = IncidenceDetails,
                HasIncidents = HasIncidents,
                HasFeedback = HasFeedback,
                IsTeamLead = IsTeamLead,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
            };
        }
        public AddParticipationReportViewModel Convert(AssignmentCrewReport assignmentCrewReport)
        {
            int _arrivalHour = 0;
            string _arrivalAmPm = "";
            int _departureHour = 0;
            string _departureAmPm = "";

            if (assignmentCrewReport.ArrivalTime.HasValue)
            {
                if (assignmentCrewReport.ArrivalTime.Value.Hour == 00)
                {
                    _arrivalHour = 12;
                    _arrivalAmPm = "AM";
                }
                else if (assignmentCrewReport.ArrivalTime.Value.Hour > 12)
                {
                    _arrivalHour = assignmentCrewReport.ArrivalTime.Value.Hour - 12;
                    _arrivalAmPm = "PM";
                }
                else
                {
                    _arrivalHour = assignmentCrewReport.ArrivalTime.Value.Hour;
                    _arrivalAmPm = assignmentCrewReport.ArrivalTime.Value.ToString("tt");
                }
            }

            if (assignmentCrewReport.DepartureTime.HasValue)
            {
                if (assignmentCrewReport.DepartureTime.Value.Hour == 00)
                {
                    _departureHour = 12;
                    _departureAmPm = "AM";
                }
                else if (assignmentCrewReport.DepartureTime.Value.Hour > 12)
                {
                    _departureHour = assignmentCrewReport.DepartureTime.Value.Hour - 12;
                    _departureAmPm = "PM";
                }
                else
                {
                    _departureHour = assignmentCrewReport.DepartureTime.Value.Hour;
                    _departureAmPm = assignmentCrewReport.DepartureTime.Value.ToString("tt");
                }
            }

            return new AddParticipationReportViewModel
            {
                DepartureDate = assignmentCrewReport.DepartureTime.Value.Date,
                DepartureHour = _departureHour,
                DepartureMinute = assignmentCrewReport.DepartureTime.Value.Minute,
                DepartureAmPm = _departureAmPm,

                ArrivalDate = assignmentCrewReport.ArrivalTime.Value.Date,
                ArrivalHour = _arrivalHour,
                ArrivalMinute = assignmentCrewReport.ArrivalTime.Value.Minute,
                ArrivalAmPm = _arrivalAmPm,

                ArrivalType = assignmentCrewReport.ArrivalType,
                AssignmentCrewId = assignmentCrewReport.AssignmentCrewId,
                AssignmentId = assignmentCrewReport.AssignmentId,
                AssignmentTitle = assignmentCrewReport.AssignmentTitle,
                AttendanceStatus = assignmentCrewReport.AttendanceStatus,
                CrewReportId = assignmentCrewReport.CrewReportId,
                DepartureType = assignmentCrewReport.DepartureType,
                EmployeeId = assignmentCrewReport.EmployeeId,
                EmployeeName = assignmentCrewReport.EmployeeName,
                FeedbackDetails = assignmentCrewReport.FeedbackDetails,
                IncidenceDetails = assignmentCrewReport.IncidenceDetails,
                HasIncidents = assignmentCrewReport.HasIncidents,
                HasFeedback = assignmentCrewReport.HasFeedback,
                IsTeamLead = assignmentCrewReport.IsTeamLead,
                ModifiedBy = assignmentCrewReport.ModifiedBy,
                ModifiedTime = assignmentCrewReport.ModifiedTime,
            };
        }

    }
}
