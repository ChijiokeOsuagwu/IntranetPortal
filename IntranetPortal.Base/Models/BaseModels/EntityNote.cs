using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public abstract class EntityNote
    {
        public long NoteId { get; set; }
        public DateTime? NoteTime { get; set; }
        public string NoteDescription { get; set; }
        public string NoteWrittenBy { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledOn { get; set; }
    }
}
