using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class WorkItemNote
    {
        public long NoteId { get; set; }
        public DateTime? NoteTime { get; set; }
        public string NoteContent { get; set; }
        public string NoteWrittenBy { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string CancelledBy { get; set; }
        public long? TaskItemId { get; set; }
        public long? WorkItemFolderId { get; set; }
        public long? ProjectId { get; set; }
    }
}
