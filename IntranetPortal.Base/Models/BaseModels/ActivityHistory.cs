using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.BaseModels
{
    public class ActivityHistory
    {
        public int ActivityHistoryId { get; set; }
        public DateTime? ActivityTime { get; set; }
        public string ActivityUserFullName { get; set; }
        public string ActivityDetails { get; set; }
        public string ActivitySource { get; set; }
        public string ActivitySourceIP { get; set; }
        public string ActivityTimeZone { get; set; }
        public string ActivityMachineName { get; set; }
    }
}
