using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LmsModels
{
    public class LeaveNote
    {
        public long Id { get; set; }
        public long LeaveId { get; set; }
        public string FromEmployeeName { get; set; }
        public string NoteContent { get; set; }
        public DateTime? TimeAdded { get; set; }
    }
}
