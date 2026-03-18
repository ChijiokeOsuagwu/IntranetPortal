using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SrmModels
{
    public class ServiceIncident
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Impact { get; set; }
        public int Severity { get; set; }
        public string SeverityDescription { get; set; }
        public DateTime? IncidentTime { get; set; }
        public string IncidentEmployeeId { get; set; }
        public string IncidentEmployeeName { get; set; }
        public string ReportedByEmployeeName { get; set; }
        public DateTime? ReportedTime { get; set; }
        public string IncidentStatus { get; set; }
        public bool IsFalseNegative { get; set; }
        public int? ServiceSystemId { get; set; }
        public string ServiceSystemName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int? UnitId { get; set; }
        public string UnitName { get; set; }
        public bool IsAssigned { get; set; }
        public string ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; }
        public bool ConfirmedResolved { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedTime { get; set; }
    }
}
