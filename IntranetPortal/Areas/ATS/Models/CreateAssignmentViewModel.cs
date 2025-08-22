using IntranetPortal.Base.Models.AtsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class CreateAssignmentViewModel:BaseViewModel
    {
        public long? Id { get; set; }

        [Required]
        public string No { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name="Event Title")]
        public string Title { get; set; }

        [MaxLength(500)]
        [Display(Name = "Event Details")]
        public string Description { get; set; }

        [Display(Name = "Event Type")]
        public int? EventTypeId { get; set; }

        [Display(Name = "Event Type")]
        public string EventTypeTitle { get; set; }

        [Required(ErrorMessage = "Please select the Event Start Date.")]
        public DateTime? EventStartDate { get; set; }

        [Required(ErrorMessage = "Please select the Event Start Hour.")]
        [Display(Name="Start Hour")]
        public int EventStartHour { get; set; }

        [Required(ErrorMessage = "Please select the Event Start Minute.")]
        [Display(Name="Start Minute")]
        public int EventStartMinute { get; set; }

        [Required(ErrorMessage = "Please select AM or PM for the Event Start Time.")]
        public string EventStartAmPm { get; set; }

        [Required(ErrorMessage = "Please select the Event End Date.")]
        public DateTime? EventEndDate { get; set; }
        [Required(ErrorMessage = "Please select the Event End Hour.")]
        public int EventEndHour { get; set; }
        [Required(ErrorMessage = "Please select the Event End Minute.")]
        public int EventEndMinute { get; set; }
        [Required (ErrorMessage="Please select AM or PM for the Event End Time.")]
        public string EventEndAmPm { get; set; }

        public DateTime? ReportDueDate { get; set; }
        public int? StationId { get; set; }
        public string StationName { get; set; }
        public string AssignedToId { get; set; }
        [Required]
        public string AssignedToName { get; set; }
        [Required]
        public string AssignedToRole { get; set; }
        [Required]
        public string AssignedById { get; set; }
        public string AssignedByName { get; set; }
        public string EventVenue { get; set; }
        [Required]
        public string EventState { get; set; }
        public string EventCountry { get; set; }
        public string ClientId { get; set; }
        [Required]
        public string ClientName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string ApprovalStatus { get; set; }
        public string ProgressStatus { get; set; }
        public bool IsPaid { get; set; }
        public bool IsLive { get; set; }
        public bool IsUsed { get; set; }
        public bool IsPriority { get; set; }
        public bool IsConfirmed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }

        public Assignment Convert()
        {
            if(EventStartAmPm == "PM" && EventStartHour < 12) { EventStartHour = EventStartHour + 12; }
            if(EventEndAmPm == "PM" && EventEndHour < 12) { EventEndHour = EventEndHour + 12; }
            if(EventStartAmPm == "AM" && EventStartHour == 12) { EventStartHour = 00; }
            if(EventStartAmPm == "AM" && EventEndHour == 12) { EventEndHour = 00; }

            return new Assignment
            {
                ApprovalStatus = ApprovalStatus,
                AssignedById = AssignedById,
                AssignedByName = AssignedByName,
                AssignedToId = AssignedToId,
                AssignedToName = AssignedToName,
                AssignedToRole = AssignedToRole,
                ClientId = ClientId,
                ClientName = ClientName,
                ContactPerson = ContactPerson,
                ContactPhone = ContactPhone,
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                Description = Description,
                EventCountry = EventCountry,
                EventEndTime = new DateTime(EventEndDate.Value.Year, EventEndDate.Value.Month, EventEndDate.Value.Day, EventEndHour, EventEndMinute, 0),
                EventStartTime = new DateTime(EventStartDate.Value.Year, EventStartDate.Value.Month, EventStartDate.Value.Day, EventStartHour, EventStartMinute, 0),
                EventState = EventState,
                EventTypeId = EventTypeId,
                EventTypeTitle = EventTypeTitle,
                EventVenue = EventVenue,
                Id = Id,
                IsConfirmed = IsConfirmed,
                IsLive = IsLive,
                IsPaid = IsPaid,
                IsPriority = IsPriority,
                IsUsed = IsUsed,
                No = No,
                ProgressStatus = ProgressStatus,
                ReportDueDate = ReportDueDate,
                StationId = StationId,
                StationName = StationName,
                Title = Title,
            };
        }
        public CreateAssignmentViewModel Convert(Assignment assignment)
        {
            int _startHour = 0;
            string _startAmPm = "AM";
            int _endHour = 0;
            string _endAmPm = "AM";
            if(assignment.EventStartTime.HasValue && assignment.EventStartTime.Value.Hour > 12)
            {
                _startHour = assignment.EventStartTime.Value.Hour - 12;
                _startAmPm = "PM";
            }

            if (assignment.EventEndTime.HasValue && assignment.EventEndTime.Value.Hour > 12)
            {
                _endHour = assignment.EventEndTime.Value.Hour - 12;
                _endAmPm = "PM";
            }

            if(assignment.EventStartTime.HasValue && assignment.EventStartTime.Value.Hour == 00)
            {
                _startHour = 12;
                _startAmPm = "AM";
            }

            if (assignment.EventEndTime.HasValue && assignment.EventEndTime.Value.Hour == 00)
            {
                _endHour = 12;
                _endAmPm = "AM";
            }

            return new CreateAssignmentViewModel
            {
                ApprovalStatus = assignment.ApprovalStatus,
                AssignedById = assignment.AssignedById,
                AssignedByName = assignment.AssignedByName,
                AssignedToId = assignment.AssignedToId,
                AssignedToName = assignment.AssignedToName,
                AssignedToRole = assignment.AssignedToRole,
                ClientId = assignment.ClientId,
                ClientName = assignment.ClientName,
                ContactPerson = assignment.ContactPerson,
                ContactPhone = assignment.ContactPhone,
                CreatedBy = assignment.CreatedBy,
                CreatedTime = assignment.CreatedTime,
                Description = assignment.Description,
                EventCountry = assignment.EventCountry,
                EventEndDate = assignment.EventEndTime.Value.Date,
                EventEndHour = _endHour,
                EventEndMinute = assignment.EventEndTime.Value.Minute,
                EventEndAmPm = _endAmPm,
                EventStartDate = assignment.EventStartTime.Value.Date,
                EventStartHour = _startHour,
                EventStartMinute = assignment.EventStartTime.Value.Minute,
                EventStartAmPm = _startAmPm,
                EventState = assignment.EventState,
                EventTypeId = assignment.EventTypeId,
                EventTypeTitle = assignment.EventTypeTitle,
                EventVenue = assignment.EventVenue,
                Id = assignment.Id,
                IsConfirmed = assignment.IsConfirmed,
                IsLive = assignment.IsLive,
                IsPaid = assignment.IsPaid,
                IsPriority = assignment.IsPriority,
                IsUsed = assignment.IsUsed,
                No = assignment.No,
                ProgressStatus = assignment.ProgressStatus,
                ReportDueDate = assignment.ReportDueDate,
                StationId = assignment.StationId,
                StationName = assignment.StationName,
                Title = assignment.Title,
            };
        }
    }
}
