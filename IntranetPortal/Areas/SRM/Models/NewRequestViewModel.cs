using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Models
{
    public class NewRequestViewModel:BaseViewModel
    {
        public long Id { get; set; }
        public string Number { get; set; }
        [Required]
        [Display(Name ="Problem Description")]
        public string Description { get; set; }

        [Display(Name = "Problem Impact")]
        public string Impact { get; set; }

        [Display(Name = "Severity")]
        [Required]
        public int Severity { get; set; }
        public string SeverityDescription { get; set; }

        [Display(Name ="Date Occurred")]
        public DateTime? IncidentTime { get; set; }

        [Display(Name = "Occurred To")]
        public string IncidentEmployeeId { get; set; }

        [Display(Name = "Occurred To")]
        [Required]
        public string IncidentEmployeeName { get; set; }

        [Display(Name = "Reported By")]
        public string ReportedByEmployeeName { get; set; }

        [Display(Name = "Time Reported")]
        public DateTime? ReportedTime { get; set; }

        [Display(Name = "Status")]
        public string IncidentStatus { get; set; }
        public bool IsFalseNegative { get; set; }

        [Display(Name = "System")]
        public int? ServiceSystemId { get; set; }

        [Display(Name = "System")]
        public string ServiceSystemName { get; set; }

        [Display(Name = "Problem Location")]
        public int? LocationId { get; set; }

        [Display(Name = "Problem Location")]
        public string LocationName { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Display(Name = "Unit")]
        public int? UnitId { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [Display(Name = "Send To: (Service Center)")]
        public string ServiceCenterId { get; set; }

        [Display(Name = "Send To: (Service Center)")]
        public string ServiceCenterName { get; set; }

        public ServiceIncident Convert()
        {
            ServiceIncident s = new ServiceIncident();
            s.DepartmentId = DepartmentId;
            s.DepartmentName = DepartmentName;
            s.Description = Description;
            s.Id = Id;
            s.Impact = Impact;
            s.IncidentEmployeeId = IncidentEmployeeId;
            s.IncidentEmployeeName = IncidentEmployeeName;
            s.IncidentStatus = IncidentStatus;
            s.IncidentTime = IncidentTime;
            s.LocationId = LocationId;
            s.LocationName = LocationName;
            s.Number = Number;
            s.ReportedByEmployeeName = ReportedByEmployeeName;
            s.ReportedTime = ReportedTime;
            s.ServiceCenterId = ServiceCenterId;
            s.ServiceCenterName = ServiceCenterName;
            s.ServiceSystemId = ServiceSystemId;
            s.ServiceSystemName = ServiceSystemName;
            s.Severity = Severity;
            s.SeverityDescription = SeverityDescription;
            s.UnitId = UnitId;
            s.UnitName = UnitName;
            return s;
        }
        public NewRequestViewModel Convert(ServiceIncident s)
        {
            NewRequestViewModel model = new NewRequestViewModel();
            model.DepartmentId = s.DepartmentId;
            model.DepartmentName = s.DepartmentName;
            model.Description = s.Description;
            model.Id = s.Id;
            model.Impact = s.Impact;
            model.IncidentEmployeeId = s.IncidentEmployeeId;
            model.IncidentEmployeeName = s.IncidentEmployeeName;
            model.IncidentStatus = s.IncidentStatus;
            model.IncidentTime = s.IncidentTime;
            model.LocationId = s.LocationId;
            model.LocationName = s.LocationName;
            model.Number = s.Number;
            model.ReportedByEmployeeName = s.ReportedByEmployeeName;
            model.ReportedTime = s.ReportedTime;
            model.ServiceCenterId = s.ServiceCenterId;
            model.ServiceCenterName = s.ServiceCenterName;
            model.ServiceSystemId = s.ServiceSystemId;
            model.ServiceSystemName = s.ServiceSystemName;
            model.Severity = s.Severity;
            model.SeverityDescription = s.SeverityDescription;
            model.UnitId = s.UnitId;
            model.UnitName = s.UnitName;
            return model;
        }

    }
}
