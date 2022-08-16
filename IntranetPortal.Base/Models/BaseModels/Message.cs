using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public class Message
    {
        public string MessageID { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public DateTime? SentTime { get; set; }
        public string SentBy { get; set; } 
        public string ActionUrl { get; set; }

        public long MessageDetailID { get; set; }
        public string RecipientID { get; set; }
        public string RecipientName { get; set; }
        public bool IsRead { get; set; }
        public string ReadTime { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedTime { get; set; }
    }
}
