using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class AddEngReportViewModel:BaseViewModel
    {
        public long EngReportId { get; set; }
        public long AssignmentId { get; set; }
        public string AssignmentNo { get; set; }
        public string AssignmentTitle { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Required (ErrorMessage ="Please rate the quality of the audio.")]
        public int AudioQuality { get; set; }
        public string AudioQualityDescription { get; set; }
        [Required(ErrorMessage = "Please rate the quality of the video.")]
        public int VideoQuality { get; set; }
        public string VideoQualityDescription { get; set; }
        public bool ScriptIsAvailable { get; set; }
        public bool MaterialsAreAvailable { get; set; }
        public bool ReporterIsAvailable { get; set; }
        
        public DateTime? ReporterArrivalTime { get; set; }
        public DateTime? ReporterArrivalDate { get; set; }
        public int? ReporterArrivalHour { get; set; }
        public int? ReporterArrivalMinute { get; set; }
        public string ReporterArrivalAmPm { get; set; }


        [Required(ErrorMessage = "Editing Start Time is required.")]
        public DateTime EditingStartTime { get; set; }
        public DateTime EditingStartDate { get; set; }
        public int EditingStartHour { get; set; }
        public int EditingStartMinute { get; set; }
        public string EditingStartAmPm { get; set; }

        [Required(ErrorMessage = "Editing Completion Time is required.")]
        public DateTime EditingEndTime { get; set; }
        public DateTime EditingEndDate { get; set; }
        public int EditingEndHour { get; set; }
        public int EditingEndMinute { get; set; }
        public string EditingEndAmPm { get; set; }

        [Required(ErrorMessage = "Editing Status is required.")]
        public string EditingStatus { get; set; }
        public string Feedback { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }

        public string CustomerName { get; set; }

        public AssignmentEngReport Convert()
        {
            if (ReporterArrivalAmPm == "PM" && ReporterArrivalHour < 12) { ReporterArrivalHour = ReporterArrivalHour + 12; }
            if (EditingStartAmPm == "PM" && EditingStartHour < 12) { EditingStartHour = EditingStartHour + 12; }
            if (EditingEndAmPm == "PM" && EditingEndHour < 12) { EditingEndHour = EditingEndHour + 12; }

            if (ReporterArrivalAmPm == "AM" && ReporterArrivalHour == 12) { ReporterArrivalHour = 0; }
            if (EditingStartAmPm == "AM" && EditingStartHour == 12) { EditingStartHour = 0; }
            if (EditingEndAmPm == "AM" && EditingEndHour == 12) { EditingEndHour = 0; }


            return new AssignmentEngReport
            {
                ReporterArrivalTime = new DateTime(ReporterArrivalDate.Value.Year, ReporterArrivalDate.Value.Month, ReporterArrivalDate.Value.Day, ReporterArrivalHour ?? 0, ReporterArrivalMinute ?? 0, 0),
                EditingStartTime = new DateTime(EditingStartDate.Year, EditingStartDate.Month, EditingStartDate.Day, EditingStartHour, EditingStartMinute, 0),
                EditingEndTime = new DateTime(EditingEndDate.Year, EditingEndDate.Month, EditingEndDate.Day, EditingEndHour, EditingEndMinute, 0),

                AssignmentId = AssignmentId,
                AssignmentTitle = AssignmentTitle,
                AudioQuality = AudioQuality,
                AudioQualityDescription = AudioQualityDescription,
                EditingStatus = EditingStatus,
                EmployeeId = EmployeeId,
                EmployeeName = EmployeeName,
                EngReportId = EngReportId,
                Feedback = Feedback,
                MaterialsAreAvailable = MaterialsAreAvailable,
                ReporterIsAvailable = ReporterIsAvailable,
                ScriptIsAvailable = ScriptIsAvailable,
                VideoQuality = VideoQuality,
                VideoQualityDescription = VideoQualityDescription,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                
            };
        }
        public AddEngReportViewModel Convert(AssignmentEngReport assignmentEngReport)
        {
            int _reporterArrivalHour = 0;
            string _reporterArrivalAmPm = "";
            int _editingStartHour = 0;
            string _editingStartAmPm = "";
            int _editingEndHour = 0;
            string _editingEndAmPm = "";

            if (assignmentEngReport.ReporterArrivalTime.HasValue)
            {
                if (assignmentEngReport.ReporterArrivalTime.Value.Hour == 00)
                {
                    _reporterArrivalHour = 12;
                    _reporterArrivalAmPm = "AM";
                }
                else if (assignmentEngReport.ReporterArrivalTime.Value.Hour > 12)
                {
                    _reporterArrivalHour = assignmentEngReport.ReporterArrivalTime.Value.Hour - 12;
                    _reporterArrivalAmPm = "PM";
                }
                else
                {
                    _reporterArrivalHour = assignmentEngReport.ReporterArrivalTime.Value.Hour;
                    _reporterArrivalAmPm = assignmentEngReport.ReporterArrivalTime.Value.ToString("tt");
                }
            }

            if (assignmentEngReport.EditingStartTime.HasValue)
            {
                if (assignmentEngReport.EditingStartTime.Value.Hour == 00)
                {
                    _editingStartHour = 12;
                    _editingStartAmPm = "AM";
                }
                else if (assignmentEngReport.EditingStartTime.Value.Hour > 12)
                {
                    _editingStartHour = assignmentEngReport.EditingStartTime.Value.Hour - 12;
                    _editingStartAmPm = "PM";
                }
                else
                {
                    _editingStartHour = assignmentEngReport.EditingStartTime.Value.Hour;
                    _editingStartAmPm = assignmentEngReport.EditingStartTime.Value.ToString("tt");
                }
            }

            if (assignmentEngReport.EditingEndTime.HasValue)
            {
                if (assignmentEngReport.EditingEndTime.Value.Hour == 00)
                {
                    _editingEndHour = 12;
                    _editingEndAmPm = "AM";
                }
                else if (assignmentEngReport.EditingEndTime.Value.Hour > 12)
                {
                    _editingEndHour = assignmentEngReport.EditingEndTime.Value.Hour - 12;
                    _editingEndAmPm = "PM";
                }
                else
                {
                    _editingEndHour = assignmentEngReport.EditingEndTime.Value.Hour;
                    _editingEndAmPm = assignmentEngReport.EditingEndTime.Value.ToString("tt");
                }
            }

            return new AddEngReportViewModel
            {
                ReporterArrivalDate = assignmentEngReport.ReporterArrivalTime.Value.Date,
                ReporterArrivalHour = _reporterArrivalHour,
                ReporterArrivalMinute = assignmentEngReport.ReporterArrivalTime.Value.Minute,
                ReporterArrivalAmPm = _reporterArrivalAmPm,

                EditingStartDate = assignmentEngReport.EditingStartTime.Value.Date,
                EditingStartHour = _editingStartHour,
                EditingStartMinute = assignmentEngReport.EditingStartTime.Value.Minute,
                EditingStartAmPm = _editingStartAmPm,

                EditingEndDate = assignmentEngReport.EditingEndTime.Value.Date,
                EditingEndHour = _editingEndHour,
                EditingEndMinute = assignmentEngReport.EditingEndTime.Value.Minute,
                EditingEndAmPm = _editingEndAmPm,

                AssignmentId = assignmentEngReport.AssignmentId,
                AssignmentTitle = assignmentEngReport.AssignmentTitle,
                AudioQuality = assignmentEngReport.AudioQuality,
                AudioQualityDescription = assignmentEngReport.AudioQualityDescription,
                EditingStatus = assignmentEngReport.EditingStatus,
                EmployeeId = assignmentEngReport.EmployeeId,
                EmployeeName = assignmentEngReport.EmployeeName,
                EngReportId = assignmentEngReport.EngReportId,
                Feedback = assignmentEngReport.Feedback,
                MaterialsAreAvailable = assignmentEngReport.MaterialsAreAvailable,
                ReporterIsAvailable = assignmentEngReport.ReporterIsAvailable,
                ScriptIsAvailable = assignmentEngReport.ScriptIsAvailable,
                VideoQuality = assignmentEngReport.VideoQuality,
                VideoQualityDescription = assignmentEngReport.VideoQualityDescription,
                ModifiedBy = assignmentEngReport.ModifiedBy,
                ModifiedTime = assignmentEngReport.ModifiedTime,
            };
        }
    }
}
