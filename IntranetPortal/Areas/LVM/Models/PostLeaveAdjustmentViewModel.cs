using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class PostLeaveAdjustmentViewModel
    {
        public long LeaveRequestId { get; set; }
        public int LeaveYearId { get; set; }
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }
        public string AdjustmentType { get; set; }
        public int NumberOfDays { get; set; }
        public string DurationDescription { get; set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public DateTime AdjustmentMadeOn { get; set; }
        public string AdjustmentMadeBy { get; set; }
    }
}
