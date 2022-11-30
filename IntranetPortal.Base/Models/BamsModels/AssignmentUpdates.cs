using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BamsModels
{
    public class AssignmentUpdates
    {
        public int AssignmentUpdateID { get; set; }
        public string UpdateDescription { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public int AssignmentEventID { get; set; }
        public string AssignmentEventTitle { get; set; }
        public AssignmentUpdateType UpdateType { get; set; }
        public AssignmentUpdateStatus UpdateStatus { get; set; }
        public bool IsCancelled { get; set; }
        public string CancelledBy { get; set; }
        public string CancelledTime { get; set; }
    }

    public enum AssignmentUpdateType
    {
        Information,
        Progress,
        Incidence,
    }

    public enum AssignmentUpdateStatus
    {
        New,
        Saved
    }
}
