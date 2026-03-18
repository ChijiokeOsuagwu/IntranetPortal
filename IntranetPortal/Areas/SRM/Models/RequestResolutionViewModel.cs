using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Models
{
    public class RequestResolutionViewModel:BaseViewModel
    {
        public long Id { get; set; }
        [Required]
        public long IncidentId { get; set; }
        public string IncidentNumber { get; set; }

        [Display(Name ="Problem Description")]
        public string IncidentDescription { get; set; }

        [Display(Name = "Service Type")]
        public int? ServiceTypeId { get; set; }

        [Display(Name = "Service Type")]
        public string ServiceTypeName { get; set; }

        [Display(Name = "Resolved By")]
        public string ResolvedByEmployeeId { get; set; }

        [Required]
        [Display(Name = "Resolved By")]
        public string ResolvedByEmployeeName { get; set; }

        [Required]
        [Display(Name = "Date Resolved")]
        public DateTime? ResolvedTime { get; set; }

        [Required]
        [Display(Name ="Solution Description")]
        public string ResolutionDescription { get; set; }

        [Display(Name = "Recorded By")]
        public string RecordedByEmployeeName { get; set; }

        [Display(Name = "Recorded On")]
        public DateTime RecordedTime { get; set; }

        public bool IsConfirmed { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedTime { get; set; }

        public IncidentResolution Convert()
        {
            return new IncidentResolution
            {
               ConfirmedBy = ConfirmedBy,
               ConfirmedTime = ConfirmedTime,
               Id = Id,
               IncidentDescription = IncidentDescription,
               IncidentId = IncidentId,
               IncidentNumber = IncidentNumber,
               IsConfirmed = IsConfirmed,
               RecordedByEmployeeName = RecordedByEmployeeName,
               RecordedTime = RecordedTime,
               ResolutionDescription = ResolutionDescription,
               ResolvedByEmployeeId = ResolvedByEmployeeId,
               ResolvedByEmployeeName = ResolvedByEmployeeName,
               ResolvedTime = ResolvedTime,
               ServiceTypeId = ServiceTypeId,
               ServiceTypeName = ServiceTypeName,
            };
        }
        public RequestResolutionViewModel Convert(IncidentResolution e)
        {
            return new RequestResolutionViewModel
            {
                ConfirmedBy = e.ConfirmedBy,
                ConfirmedTime = e.ConfirmedTime,
                Id = e.Id,
                IncidentDescription = e.IncidentDescription,
                IncidentId = e.IncidentId,
                IncidentNumber = e.IncidentNumber,
                IsConfirmed = e.IsConfirmed,
                RecordedByEmployeeName = e.RecordedByEmployeeName,
                RecordedTime = e.RecordedTime,
                ResolutionDescription = e.ResolutionDescription,
                ResolvedByEmployeeId = e.ResolvedByEmployeeId,
                ResolvedByEmployeeName = e.ResolvedByEmployeeName,
                ResolvedTime = e.ResolvedTime,
                ServiceTypeId = e.ServiceTypeId,
                ServiceTypeName = e.ServiceTypeName,
            };
        }
    }
}
