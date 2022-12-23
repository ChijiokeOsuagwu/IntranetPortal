using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class MessagesListViewModel : BaseListViewModel
    {
        public string MessageRecipientID { get; set; }
        public List<Message> MessageList { get; set; }
        public int UnreadMessagesCount { get; set; }
        public int ReadMessagesCount { get; set; }
        public int TotalMesssagesCount { get; set; }
        public int RecordCount { get; set; }
    }
}
