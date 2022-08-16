using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class EmailModel
    {
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string PlainContent { get; set; }
        public string HtmlContent { get; set; }
    }
}
