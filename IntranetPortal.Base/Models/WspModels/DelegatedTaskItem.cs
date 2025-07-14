using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.WspModels
{
    public class DelegatedTaskItem : TaskItem
    {
        public long TaskDelegationId { get; set; }
        public string DelegatedByEmployeeId { get; set; }
        public string DelegatedByEmployeeName { get; set; }
        public string DelegatedToEmployeeId { get; set; }
        public string DelegatedToEmployeeName { get; set; }
        public DateTime? DelegatedTime { get; set; }
        public bool IsReAssigned { get; set; }
        public DateTime? ReassignedTime { get; set; }
    }
}
