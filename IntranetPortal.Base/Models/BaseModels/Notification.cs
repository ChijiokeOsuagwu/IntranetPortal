using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public int NotificationGroupID { get; set; }
        public string NotificationGroupDescription { get; set; }
        public int NotificationType { get; set; }
        public string NotificationTypeDescription { get; set; }
        public string MessageTitle { get; set; }
        public string MessageBody { get; set; }
    }
}
