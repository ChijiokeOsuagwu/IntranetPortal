using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public class NotificationType
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int GroupID { get; set; }
        public string GroupDescription { get; set; }
    }
}
