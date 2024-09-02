using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class ReviewMessage
    {
        public int ReviewMessageId { get; set; }
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string FromEmployeeId { get; set; }
        public string FromEmployeeName { get; set; }
        public string FromEmployeeSex { get; set; }
        public string MessageBody { get; set; }
        public DateTime? MessageTime { get; set; }
        public bool MessageIsCancelled { get; set; }
        public DateTime? TimeCancelled { get; set; }
    }
}
