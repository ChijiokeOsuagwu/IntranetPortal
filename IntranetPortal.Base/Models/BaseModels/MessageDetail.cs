using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public class MessageDetail
    {
        public long MessageDetailID { get; set; }
        public string RecipientID { get; set; }
        public string RecipientName { get; set; }
        public string MessageID { get; set; }
        public bool IsRead { get; set; }
        public string ReadTime { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedTime { get; set; }
    }
}
