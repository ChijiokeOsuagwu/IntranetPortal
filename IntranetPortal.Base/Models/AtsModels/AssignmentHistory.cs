using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AtsModels
{
    public class AssignmentHistory
    {
        public long Id { get; set; }
        public DateTime? ActivityTime { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityBy { get; set; }
        public long AssignmentId { get; set; }
    }
}
