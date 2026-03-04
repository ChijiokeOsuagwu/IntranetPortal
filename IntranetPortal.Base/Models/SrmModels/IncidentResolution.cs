using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SrmModels
{
    public class IncidentResolution
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long IncidentDescription { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
        public string ResolvedByEmployeeId { get; set; }
        public string ResolvedByEmployeeName { get; set; }
        public DateTime? ResolvedTime { get; set; }
        public string ResolutionDescription { get; set; }
        public bool IsConfirmed { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedTime { get; set; }
    }
}
