using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ERM.Models
{
    public class ManageSeparationViewModel:BaseViewModel
    {
        public int EmployeeSeparationId { get; set; }
        public string EmployeeId { get; set; }

        [Required]
        [Display(Name="Staff Name*")]
        public string EmployeeName { get; set; }
        
        [Required]
        public int UnitId { get; set; }

        
        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        [Required]
        [Display(Name ="Type*")]
        public int SeparationTypeId { get; set; }

        [Display(Name ="Type")]
        public string SeparationTypeDescription { get; set; }

        [Display(Name = "Reason")]
        public int SeparationReasonId { get; set; }

        [Display(Name = "Reason")]
        public string SeparationReasonDescription { get; set; }

        [Required]
        [Display(Name = "Reason Explanation*")]
        public string SeparationReasonExplanation { get; set; }

        [Display(Name = "Notice Submitted Date")]
        public DateTime? NoticeServedDate { get; set; }

        [Display(Name = "Notice Submitted Date")]
        public string NoticeServedDateFormatted { get; set; }

        [Required]
        [Display(Name ="Notice Period*")]
        public int NoticePeriodInMonths { get; set; }

        [Display(Name = "Expected Last Work Date" )]
        public DateTime? ExpectedLastWorkedDate { get; set; }

        [Display(Name = "Expected Last Work Date")]
        public string ExpectedLastWorkedDateFormatted { get; set; }

        [Display(Name = "Actual Last Work Date")]
        public DateTime? ActualLastWorkedDate { get; set; }

        [Display(Name = "Actual Last Work Date")]
        public string ActualLastWorkedDateFormatted { get; set; }

        [Display(Name = "Outstanding Leave Days")]
        public int OutstandingLeaveDays { get; set; }

        [Display(Name ="Outstanding Work Days")]
        public int OutstandingWorkDays { get; set; }

        [Required]
        public bool EligibleForRehire { get; set; }

        [Required]
        public bool ReturnedAssignedAssets { get; set; }
        public bool IsOwed { get; set; }
        public bool IsIndebted { get; set; }
        public DateTime? RecordCreatedDate { get; set; }
        public string RecordCreatedBy { get; set; }
        public DateTime? RecordModifiedDate { get; set; }
        public string RecordModifiedBy { get; set; }

        public EmployeeSeparation Convert()
        {
            return new EmployeeSeparation
            {
                EligibleForRehire = EligibleForRehire,
                EmployeeId = EmployeeId,
                EmployeeName = EmployeeName,
                EmployeeSeparationId = EmployeeSeparationId,
                ExpectedLastWorkedDate = ExpectedLastWorkedDate,
                NoticeServedDate = NoticeServedDate,
                RecordCreatedBy = RecordCreatedBy,
                RecordCreatedDate = RecordCreatedDate,
                RecordModifiedBy = RecordModifiedBy,
                RecordModifiedDate = RecordModifiedDate,
                ReturnedAssignedAssets = ReturnedAssignedAssets,
                SeparationReasonDescription = SeparationReasonDescription,
                SeparationReasonExplanation = SeparationReasonExplanation,
                SeparationReasonId = SeparationReasonId,
                SeparationTypeDescription = SeparationTypeDescription,
                SeparationTypeId = SeparationTypeId,
                ActualLastWorkedDate = ActualLastWorkedDate,
                IsIndebted = IsIndebted,
                IsOwed = IsOwed,
                NoticePeriodInMonths = NoticePeriodInMonths,
                OutstandingLeaveDays = OutstandingLeaveDays,
                OutstandingWorkDays = OutstandingWorkDays,
                UnitId = UnitId,
                UnitName = UnitName,
                DepartmentId = DepartmentId,
                DepartmentName = DepartmentName,
                LocationId = LocationId,
                LocationName = LocationName
            };
        }

        public ManageSeparationViewModel Extract(EmployeeSeparation e)
        {
            return new ManageSeparationViewModel
            {
                EligibleForRehire = e.EligibleForRehire,
                EmployeeId = e.EmployeeId,
                EmployeeName = e.EmployeeName,
                EmployeeSeparationId = e.EmployeeSeparationId,
                ExpectedLastWorkedDate = e.ExpectedLastWorkedDate,
                ExpectedLastWorkedDateFormatted = e.ExpectedLastWorkedDate == null ? "[Not Specified]" : e.ExpectedLastWorkedDate.Value.ToLongDateString(),
                NoticeServedDate = e.NoticeServedDate,
                NoticeServedDateFormatted = e.NoticeServedDate == null ? "[Not Specified]" : e.NoticeServedDate.Value.ToLongDateString(),
                RecordCreatedBy = e.RecordCreatedBy,
                RecordCreatedDate = e.RecordCreatedDate,
                RecordModifiedBy = e.RecordModifiedBy,
                RecordModifiedDate = e.RecordModifiedDate,
                ReturnedAssignedAssets = e.ReturnedAssignedAssets,
                SeparationReasonDescription = e.SeparationReasonDescription,
                SeparationReasonExplanation = e.SeparationReasonExplanation,
                SeparationReasonId = e.SeparationReasonId??0,
                SeparationTypeDescription = e.SeparationTypeDescription,
                SeparationTypeId = e.SeparationTypeId,
                ActualLastWorkedDate = e.ActualLastWorkedDate,
                ActualLastWorkedDateFormatted = e.ActualLastWorkedDate == null ? "[Not Specified]" : e.ActualLastWorkedDate.Value.ToLongDateString(),
                IsIndebted = e.IsIndebted,
                IsOwed = e.IsOwed,
                NoticePeriodInMonths = e.NoticePeriodInMonths,
                OutstandingLeaveDays = e.OutstandingLeaveDays,
                OutstandingWorkDays = e.OutstandingWorkDays,
                UnitId = e.UnitId,
                UnitName = e.UnitName,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.DepartmentName,
                LocationId = e.LocationId,
                LocationName = e.LocationName
            };
        }
    }
}
