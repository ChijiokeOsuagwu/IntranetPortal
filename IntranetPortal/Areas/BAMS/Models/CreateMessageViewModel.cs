using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class CreateMessageViewModel : BaseViewModel
    {
        public string MessageID { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public DateTime? SentTime { get; set; }
        public string SentBy { get; set; }
        public string ActionUrl { get; set; }
        public int? MessageType { get; set; }

        public Message ConvertToMessage()
        {
            return new Message
            {
                ActionUrl = ActionUrl,
                IsRead = false,
                DeletedTime = string.Empty,
                IsDeleted = false,
                MessageBody = MessageBody,
                MessageID = MessageID,
                ReadTime = string.Empty,
                RecipientID = string.Empty,
                MessageDetailID = 0,
                RecipientName = string.Empty,
                SentBy = SentBy,
                SentTime = SentTime,
                Subject = Subject,
            };
        }
    }
}
