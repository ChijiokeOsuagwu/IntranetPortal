using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LmsModels
{
    public class LeaveDocument
    {
        public long DocumentId { get; set; }
        public long LeaveId { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentDescription { get; set; }
        public string DocumentFormat { get; set; }
        public string DocumentLink { get; set; }
        public DateTime? UploadedTime { get; set; }
        public string UploadedBy { get; set; }
    }
}
