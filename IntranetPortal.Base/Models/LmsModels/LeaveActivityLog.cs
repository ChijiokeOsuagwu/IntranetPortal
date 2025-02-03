using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LmsModels
{
    public class LeaveActivityLog
    {
        public long Id { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime? ActivityTime { get; set; }
        public long LeaveId { get; set; }
    }
}
