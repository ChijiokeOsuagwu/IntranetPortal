using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntranetPortal.Base.Models.AtsModels
{
    public class Assignment
    {
        public long? Id { get; set; }
        public string No { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? EventTypeId { get; set; }
        public string EventTypeTitle { get; set; }
        public DateTime? EventStartTime { get; set; }
        public DateTime? EventEndTime { get; set; }
        public DateTime? ReportDueDate { get; set; }
        public int? StationId { get; set; }
        public string StationName { get; set; }
        public string AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToRole { get; set; }
        public string AssignedById { get; set; }
        public string AssignedByName { get; set; }
        public string EventVenue { get; set; }
        public string EventState { get; set; }
        public string EventCountry { get; set; }
        public string ClientId { get; set; }
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
    }
}
