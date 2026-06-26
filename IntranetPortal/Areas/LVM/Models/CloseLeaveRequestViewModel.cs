using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class CloseLeaveRequestViewModel:BaseViewModel
    {
        public long LeaveRequestId { get; set; }
        public string LeaveEmployeeId { get; set; }
        public string LeaveEmployeeName { get; set; }

        public int LeaveYear { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public string LeaveReason { get; set; }
        public int LeaveRequestStatusId { get; set; }
        public string LeaveRequestStatusDescription { get; set; }
        public DateTime RequestedStartDate { get; set; }
        public DateTime RequestedEndDate { get; set; }
        public int RequestedDuration { get; set; }
        public string RequestedDurationDescription { get; set; }
        public int RequestedDurationTypeId { get; set; }
        public string RequestedDurationTypeDescription { get; set; }
        public DateTime? RequestedResumptionDate { get; set; }

        [Required(ErrorMessage ="Actual Start Date is required.")]
        public DateTime? ActualLeaveStartDate { get; set; }

        [Required(ErrorMessage = "Actual End Date is required.")]
        public DateTime? ActualLeaveEndDate { get; set; }

        [Required(ErrorMessage = "Actual Duration is required.")]
        [Range(1,2000, ErrorMessage ="Invalid Entry. Please enter a valid Duration.")]
        public int ActualLeaveDuration { get; set; }

        [Required(ErrorMessage = "Please select a Duration Type.")]
        public int ActualLeaveDurationTypeId { get; set; }
        public string ActualLeaveDurationDescription { get; set; }

        [Required(ErrorMessage = "Actual Resumption Date is required.")]
        public DateTime? HrResumptionDate { get; set; }
        public DateTime? LeaveRequestCloseDate { get; set; }
        public bool IsLeaveRequestClosed { get; set; }
        public string LeaveRequestClosedBy { get; set; }

        public CloseLeaveRequestViewModel Convert(LeaveRequest leaveRequest )
        {
            return new CloseLeaveRequestViewModel
            {
                ActualLeaveDuration = leaveRequest.ActualLeaveDuration,
                ActualLeaveDurationDescription = leaveRequest.ActualLeaveDurationDescription,
                ActualLeaveDurationTypeId = leaveRequest.ActualLeaveDurationTypeId ?? 0,
                ActualLeaveEndDate = leaveRequest.ActualLeaveEndDate,
                ActualLeaveStartDate = leaveRequest.ActualLeaveStartDate,
                HrResumptionDate = leaveRequest.HrResumptionDate,
                IsLeaveRequestClosed = leaveRequest.IsLeaveRequestClosed,
                LeaveEmployeeId = leaveRequest.LeaveEmployeeId,
                LeaveEmployeeName = leaveRequest.LeaveEmployeeName,
                LeaveReason = leaveRequest.LeaveReason,
                LeaveRequestCloseDate = leaveRequest.LeaveRequestCloseDate,
                LeaveRequestClosedBy = leaveRequest.LeaveRequestClosedBy,
                LeaveRequestId = leaveRequest.LeaveRequestId,
                LeaveRequestStatusDescription = leaveRequest.LeaveRequestStatusDescription,
                LeaveRequestStatusId = leaveRequest.LeaveRequestStatusId,
                LeaveTypeCode = leaveRequest.LeaveTypeCode,
                LeaveTypeName = leaveRequest.LeaveTypeName,
                LeaveYear = leaveRequest.LeaveYear,
                RequestedDuration = leaveRequest.RequestedDuration,
                RequestedDurationDescription = leaveRequest.RequestedDurationDescription,
                RequestedDurationTypeDescription = leaveRequest.RequestedDurationTypeDescription,
                RequestedDurationTypeId = leaveRequest.RequestedDurationTypeId,
                RequestedEndDate = leaveRequest.RequestedEndDate,
                RequestedResumptionDate = leaveRequest.RequestedResumptionDate,
                RequestedStartDate = leaveRequest.RequestedStartDate,
            };
        }
        public LeaveRequest Convert()
        {
            return new LeaveRequest
            {
                ActualLeaveDuration = ActualLeaveDuration,
                ActualLeaveDurationDescription = ActualLeaveDurationDescription,
                ActualLeaveDurationTypeId = ActualLeaveDurationTypeId,
                ActualLeaveEndDate = ActualLeaveEndDate,
                ActualLeaveStartDate = ActualLeaveStartDate,
                HrResumptionDate = HrResumptionDate,
                IsLeaveRequestClosed = IsLeaveRequestClosed,
                LeaveEmployeeId = LeaveEmployeeId,
                LeaveEmployeeName = LeaveEmployeeName,
                LeaveReason = LeaveReason,
                LeaveRequestCloseDate = LeaveRequestCloseDate,
                LeaveRequestClosedBy = LeaveRequestClosedBy,
                LeaveRequestId = LeaveRequestId,
                LeaveRequestStatusDescription = LeaveRequestStatusDescription,
                LeaveRequestStatusId = LeaveRequestStatusId,
                LeaveTypeCode = LeaveTypeCode,
                LeaveTypeName = LeaveTypeName,
                LeaveYear = LeaveYear,
                RequestedDuration = RequestedDuration,
                RequestedDurationDescription = RequestedDurationDescription,
                RequestedDurationTypeDescription = RequestedDurationTypeDescription,
                RequestedDurationTypeId = RequestedDurationTypeId,
                RequestedEndDate = RequestedEndDate,
                RequestedResumptionDate = RequestedResumptionDate,
                RequestedStartDate = RequestedStartDate,
            };
        }
    }
}
