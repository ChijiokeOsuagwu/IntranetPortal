using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveRequestViewModel:BaseViewModel
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

        public DateTime? ActualLeaveStartDate { get; set; }
        public DateTime? ActualLeaveEndDate { get; set; }
        public int ActualLeaveDuration { get; set; }
        public int ActualLeaveDurationTypeId { get; set; }
        public string ActualLeaveDurationDescription { get; set; }

        public DateTime? LineManagersResumptionDate { get; set; }
        public DateTime? LineManagerConfirmResumptionTime { get; set; }
        public string LineManagerConfirmResumptionBy { get; set; }

        public DateTime? HrResumptionDate { get; set; }
        public DateTime? HrConfirmResumptionTime { get; set; }
        public string HrConfirmResumptionBy { get; set; }
        public DateTime? LeaveRequestCloseDate { get; set; }

        public bool IsApprovedByLineManager { get; set; }
        public bool IsApprovedByStationManager { get; set; }
        public bool IsApprovedByHeadOfDepartment { get; set; }
        public bool IsApprovedByHR { get; set; }
        public bool IsApprovedByExecutiveManagement { get; set; }


        public LeaveRequestViewModel ExtractFromLeavePlan(LeavePlan plan)
        {
            LeaveRequestViewModel model = new LeaveRequestViewModel();
            model.RequestedDuration = plan.LeavePlanDuration;
            model.RequestedDurationTypeId = plan.LeavePlanDurationTypeId;
            model.RequestedEndDate = plan.LeavePlanEndDate ?? DateTime.Now;
            model.RequestedResumptionDate = plan.LeavePlanResumptionDate;
            model.RequestedStartDate = plan.LeavePlanStartDate ?? DateTime.Now;
            model.LeaveTypeCode = plan.LeaveTypeCode;

            return model;
        }

        public LeaveRequest Convert()
        {
            LeaveRequest request = new LeaveRequest();
            request.ActualLeaveDuration = ActualLeaveDuration;
            request.ActualLeaveDurationDescription = ActualLeaveDurationDescription;
            request.ActualLeaveDurationTypeId = ActualLeaveDurationTypeId;
            request.ActualLeaveEndDate = ActualLeaveEndDate;
            request.ActualLeaveStartDate = ActualLeaveStartDate;

            request.HrConfirmResumptionBy = HrConfirmResumptionBy;
            request.HrConfirmResumptionTime = HrConfirmResumptionTime;
            request.HrResumptionDate = HrResumptionDate;

            request.IsApprovedByExecutiveManagement = IsApprovedByExecutiveManagement;
            request.IsApprovedByHeadOfDepartment = IsApprovedByHeadOfDepartment;
            request.IsApprovedByHR = IsApprovedByHR;
            request.IsApprovedByLineManager = IsApprovedByLineManager;
            request.IsApprovedByStationManager = IsApprovedByStationManager;
            
            request.LeaveEmployeeId = LeaveEmployeeId;
            request.LeaveEmployeeName = LeaveEmployeeName;
            request.LeaveReason = LeaveReason;
            request.LeaveRequestCloseDate = LeaveRequestCloseDate;
            request.LeaveRequestId = LeaveRequestId;
            request.LeaveRequestStatusDescription = LeaveRequestStatusDescription;
            request.LeaveRequestStatusId = LeaveRequestStatusId;
            request.LeaveTypeCode = LeaveTypeCode;
            request.LeaveTypeName = LeaveTypeName;
            request.LeaveYear = LeaveYear;
            request.LineManagerConfirmResumptionBy = LineManagerConfirmResumptionBy;
            request.LineManagerConfirmResumptionTime = LineManagerConfirmResumptionTime;
            request.LineManagersResumptionDate = LineManagersResumptionDate;
            request.RequestedDuration = RequestedDuration;
            request.RequestedDurationDescription = RequestedDurationDescription;
            request.RequestedDurationTypeDescription = RequestedDurationTypeDescription;
            request.RequestedDurationTypeId = RequestedDurationTypeId;
            request.RequestedEndDate = RequestedEndDate;
            request.RequestedResumptionDate = RequestedResumptionDate;
            request.RequestedStartDate = RequestedStartDate;

            return request;
        }
    }
}
