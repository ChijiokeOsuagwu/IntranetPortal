using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.PmsModels
{
    public class PmsActivityHistory
    {
        public int ActivityId { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime? ActivityTime { get; set; }
        public int ReviewHeaderId { get; set; }
    }
}
