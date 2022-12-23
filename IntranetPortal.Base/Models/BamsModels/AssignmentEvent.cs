using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BamsModels
{
    public class AssignmentEvent
    {
        public int? ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int EventTypeID { get; set; }
        public string EventTypeName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public string Venue { get; set; }
        public string State { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string LiaisonName { get; set; }
        public string LiaisonPhone { get; set; }
        public int StatusID { get; set; }
        public string StatusDescription { get; set; }
        public bool IsPaid { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTime { get; set; }
    }

    public enum AssignmentNotificationType
    {
        NewAssignment,
        EventExtension,
        EventCancelled,
        AssignmentUpdate,
    }
}
