using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveDuration
    {
        public int Duration { get; set; }
        public int DurationTypeId { get; set; }
        public string DurationTypeDescription { get; set; }
        public string DurationDescription { get; set; }
    }
}
