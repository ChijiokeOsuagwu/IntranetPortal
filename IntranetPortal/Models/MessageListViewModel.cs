using IntranetPortal.Base.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class MessageListViewModel : BaseListViewModel
    {
        public List<Message> UnreadMessages { get; set; }
        public List<Message> ReadMessages { get; set; }
        public int UnreadMessagesCount { get; set; }
        public int ReadMessagesCount { get; set; }
        public int TotalMesssagesCount { get; set; }
    }
}
