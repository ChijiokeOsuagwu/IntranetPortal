using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveDocument
    {
        public long LeaveDocumentId { get; set; }
        public long LeaveRequestId { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentDescription { get; set; }
        public string DocumentReferencePath { get; set; }
        public string DocumentFullPath { get; set; }
        public DateTime? TimeUploaded { get; set; }
    }
}
