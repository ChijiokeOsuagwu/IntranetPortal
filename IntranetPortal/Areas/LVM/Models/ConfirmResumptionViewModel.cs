using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IntranetPortal.Areas.LVM.Models
{
    public class ConfirmResumptionViewModel: BaseViewModel
    {
        public long LeaveResumptionId { get; set; }
        public long LeaveRequestId { get; set; }
        public string LeaveEmployeeName { get; set; }

        public int LeaveYear { get; set; }
        public string LeaveTypeName { get; set; }


        public DateTime ApprovedResumptionDate { get; set; }

        public DateTime ResumptionDateByEmployee { get; set; }

        public int NoOfExtraDaysByEmployee { get; set; }
        public int NoOfUnusedDaysByEmployee { get; set; }
        public string ReasonByEmployee { get; set; }
        public DateTime DateRecordedByEmployee { get; set; }
        public bool EmployeeRequestAdjustment { get; set; }
        public string RequestedAdjustmentType { get; set; }

        public string LineManagerName { get; set; }

        [Required(ErrorMessage = "Resumption Date is required!")]
        public DateTime? ResumptionDateByLineManager { get; set; }
        public int NoOfExtraDaysByLineManager { get; set; }
        public int NoOfUnusedDaysByLineManager { get; set; }

        [Required(ErrorMessage = "Reason is required!")]
        public string ReasonByLineManager { get; set; }
        public bool LineManagerApprovesAdjustment { get; set; }
        public DateTime? DateRecordedByLineManager { get; set; }

        public long LeaveSubmissionId { get; set; }

        public LeaveResumption Convert()
        {
            return new LeaveResumption
            {
                ApprovedResumptionDate = ApprovedResumptionDate,
                DateRecordedByEmployee = DateRecordedByEmployee,
                DateRecordedByLineManager = DateRecordedByLineManager,
                NoOfExtraDaysByEmployee = NoOfExtraDaysByEmployee,
                NoOfExtraDaysByLineManager = NoOfExtraDaysByLineManager,
                NoOfUnusedDaysByEmployee = NoOfUnusedDaysByEmployee,
                NoOfUnusedDaysByLineManager = NoOfUnusedDaysByLineManager,
                LeaveEmployeeName = LeaveEmployeeName,
                LeaveRequestId = LeaveRequestId,
                LeaveResumptionId = LeaveResumptionId,
                LineManagerName = LineManagerName,
                ReasonByLineManager = ReasonByLineManager,
                ResumptionDateByLineManager = ResumptionDateByLineManager,
                ReasonByEmployee = ReasonByEmployee,
                ResumptionDateByEmployee = ResumptionDateByEmployee,
                EmployeeRequestAdjustment = EmployeeRequestAdjustment,
                RequestedAdjustmentType = RequestedAdjustmentType,
                LineManagerApprovesAdjustment = LineManagerApprovesAdjustment,
            };
        }
        public ConfirmResumptionViewModel Convert(LeaveResumption leaveResumption)
        {
            return new ConfirmResumptionViewModel
            {
                ApprovedResumptionDate = leaveResumption.ApprovedResumptionDate,
                DateRecordedByEmployee = leaveResumption.DateRecordedByEmployee,
                DateRecordedByLineManager = leaveResumption.DateRecordedByLineManager,
                NoOfExtraDaysByEmployee = leaveResumption.NoOfExtraDaysByEmployee,
                NoOfExtraDaysByLineManager = leaveResumption.NoOfExtraDaysByLineManager,
                NoOfUnusedDaysByEmployee = leaveResumption.NoOfUnusedDaysByEmployee,
                NoOfUnusedDaysByLineManager = leaveResumption.NoOfUnusedDaysByLineManager,
                LeaveEmployeeName = leaveResumption.LeaveEmployeeName,
                LeaveRequestId = leaveResumption.LeaveRequestId,
                LeaveResumptionId = leaveResumption.LeaveResumptionId,
                LineManagerName = leaveResumption.LineManagerName,
                ReasonByLineManager = leaveResumption.ReasonByLineManager,
                ResumptionDateByLineManager = leaveResumption.ResumptionDateByLineManager,
                ReasonByEmployee = leaveResumption.ReasonByEmployee,
                ResumptionDateByEmployee = leaveResumption.ResumptionDateByEmployee,
                EmployeeRequestAdjustment = leaveResumption.EmployeeRequestAdjustment,
                RequestedAdjustmentType = leaveResumption.RequestedAdjustmentType,
                LineManagerApprovesAdjustment = leaveResumption.LineManagerApprovesAdjustment,

            };
        }
    }
}
