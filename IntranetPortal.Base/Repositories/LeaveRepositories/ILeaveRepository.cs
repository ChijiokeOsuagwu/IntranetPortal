using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.LeaveModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.LeaveRepositories
{
    public interface ILeaveRepository
    {
        #region LeaveType Action Methods
        Task<bool> AddLeaveTypeAsync(LeaveType leaveType);
        Task<bool> DeleteLeaveTypeAsync(string code);
        Task<bool> EditLeaveTypeAsync(LeaveType leaveType);
        Task<List<LeaveType>> GetAllLeaveTypesAsync();
        Task<List<LeaveType>> GetAllLeaveTypesExcludingSystemAsync();
        Task<LeaveType> GetLeaveTypeByCodeAsync(string code);
        Task<LeaveType> GetLeaveTypeByNameAsync(string name);
        #endregion

        #region LeaveProfile Action Methods
        Task<bool> AddLeaveProfileAsync(LeaveProfile leaveProfile);
        Task<bool> DeleteLeaveProfileAsync(string profileCode);
        Task<bool> EditLeaveProfileAsync(LeaveProfile leaveProfile);
        Task<LeaveProfile> GetLeaveProfileByCodeAsync(string profileCode);
        Task<LeaveProfile> GetLeaveProfileByNameAsync(string profileName);
        Task<List<LeaveProfile>> GetAllLeaveProfilesAsync();
        #endregion

        #region LeaveProfileDetails Action Interfaces
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByProfileCodeAsync(string profileCode);
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetailByIdAsync(int leaveProfileDetailId);
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByProfileCodenLeaveTypeAsync(string profileCode, string leaveTypeCode);
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByEmployeeIdAsync(string employeeId);
        Task<LeaveProfileDetail> GetLeaveProfileDetailByEmployeeIdnLeaveTypeAsync(string employeeId, string leaveTypeCode);
        Task<LeaveProfileDetail> GetLeaveProfileDetailByEmployeeNamenLeaveTypeAsync(string employeeName, string leaveTypeCode);

        Task<bool> AddLeaveProfileDetailAsync(LeaveProfileDetail leaveProfileDetail);
        Task<bool> DeleteLeaveProfileDetailAsync(int leaveProfileDetailId);
        Task<bool> EditLeaveProfileDetailAsync(LeaveProfileDetail leaveProfileDetail);
        #endregion

        #region Leave Plans Action Methods

        #region Leave Plan Write Action Methods
        Task<long> AddLeavePlanAsync(LeavePlan e);
        Task<bool> DeleteLeavePlanAsync(long leavePlanId);
        Task<bool> EditLeavePlanAsync(LeavePlan e);
        Task<bool> EditLeavePlanReturnStatusAsync(long leavePlanId, bool isReturned);
        #endregion

        #region Leave Plans By Id & Employee ID & Name
        Task<LeavePlan> GetLeavePlanByIdAsync(long leavePlanId);

        //By EmployeeId
        Task<List<LeavePlan>> GetLeavePlansByEmployeeIdAsync(string employeeId);
        Task<List<LeavePlan>> GetLeavePlansByEmployeeIdAsync(string employeeId, int leaveYear);
        Task<List<LeavePlan>> GetLeavePlansByEmployeeIdAsync(string employeeId, int leaveYear, int leaveMonth);

        // By Employee Name
        Task<List<LeavePlan>> GetLeavePlansByEmployeeNameAsync(string employeeName);
        Task<List<LeavePlan>> GetLeavePlansByEmployeeNameAsync(string employeeName, int leaveYear);
        Task<List<LeavePlan>> GetLeavePlansByEmployeeNameAsync(string employeeName, int leaveYear, int leaveMonth);

        #endregion

        #region Leave Plans By LocationId & UnitId

        // By LocationId
        Task<List<LeavePlan>> GetLeavePlansByLocationIdAsync(int locationId, int leaveYear);
        Task<List<LeavePlan>> GetLeavePlansByLocationIdAsync(int locationId, int leaveYear, int leaveMonth);


        // By UnitId
        Task<List<LeavePlan>> GetLeavePlansByUnitIdAsync(int unitId, int leaveYear);
        Task<List<LeavePlan>> GetLeavePlansByUnitIdAsync(int unitId, int leaveYear, int leaveMonth);

        // By LocationId & UnitId
        Task<List<LeavePlan>> GetLeavePlansByLocationIdnUnitIdAsync(int locationId, int unitId, int leaveYear);
        Task<List<LeavePlan>> GetLeavePlansByLocationIdnUnitIdAsync(int locationId, int unitId, int leaveYear, int leaveMonth);
        #endregion

        #region Leave Plans By Leave Year & Leave Month
        // By Leave Year and Leave Months
        Task<List<LeavePlan>> GetLeavePlansByLeaveYearAsync(int leaveYear);
        Task<List<LeavePlan>> GetLeavePlansByLeaveYearnLeaveMonthAsync(int leaveYear, int leaveMonth);
        #endregion

        #region Leave Plans By ReportingLine Id
        Task<List<LeavePlan>> GetLeavePlansByReportingLineIdAsync(string teamLeadId);
        Task<List<LeavePlan>> GetLeavePlansByReportingLineIdAsync(string teamLeadId, int leaveYear);
        Task<List<LeavePlan>> GetLeavePlansByReportingLineIdAsync(string teamLeadId, int leaveYear, int startMonth);
        #endregion

        #endregion

        #region Leave Requests Action Methods

        #region Leave Requests Write Action Methods
        Task<long> AddLeaveRequestAsync(LeaveRequest r);
        Task<bool> DeleteLeaveRequestAsync(long leaveRequestId);
        Task<bool> EditLeaveRequestAsync(LeaveRequest r);
        Task<bool> UpdateLeaveRequestStatusAsync(long leaveRequestId, int newStatus);
        Task<bool> UpdateLeaveRequestApprovalStatusAsync(long leaveRequestId, ApprovalType approvalType);
        Task<bool> UpdateLeaveRequestHrConfirmedAsync(long leaveRequestId, string confirmedBy, DateTime confirmedTime);
        Task<bool> UpdateLeaveRequestToClosedAsync(LeaveRequest leaveRequest, string leaveRequestClosedBy);
        Task<bool> UpdateLeaveRequestAdjustmentRequestAsync(long leaveRequestId, bool requestedAdjustment);

        #endregion

        #region Leave Requests Read Action Methods

        #region Leave Requests By LeaveRequestId & Employee Id & Name
        Task<LeaveRequest> GetLeaveRequestByIdAsync(long leaveRequestId);

        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(string employeeId);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(string employeeId, int leaveYear);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(string employeeId, int leaveYear, int leaveMonth);

        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdnStatusAsync(string employeeId, int leaveStatus);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdnStatusAsync(string employeeId, int leaveYear, int leaveStatus);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdnStatusAsync(string employeeId, int leaveYear, int leaveMonth, int leaveStatus);


        // By Employee Name
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName, int leaveYear);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName, int leaveYear, int leaveMonth);
        #endregion

        #region Leave Requests By Reporting Line
        Task<List<LeaveRequest>> GetLeaveRequestsByReportingLineIdnStatusAsync(string teamLeadId, int leaveYear, int leaveMonth, int leaveStatus);
        Task<List<LeaveRequest>> GetLeaveRequestsByReportingLineIdnStatusAsync(string teamLeadId, int leaveYear, int leaveStatus);

        Task<List<LeaveRequest>> GetLeaveRequestsByReportingLineIdAsync(string teamLeadId, int leaveYear, int leaveMonth);
        Task<List<LeaveRequest>> GetLeaveRequestsByReportingLineIdAsync(string teamLeadId, int leaveYear);

        #endregion

        #region Leave Requests By LocationId & UnitId

        // By LocationId
        Task<List<LeaveRequest>> GetLeaveRequestsByLocationIdAsync(int locationId, int leaveYear);
        Task<List<LeaveRequest>> GetLeaveRequestsByLocationIdAsync(int locationId, int leaveYear, int leaveMonth);


        // By UnitId
        Task<List<LeaveRequest>> GetLeaveRequestsByUnitIdAsync(int unitId, int leaveYear);
        Task<List<LeaveRequest>> GetLeaveRequestsByUnitIdAsync(int unitId, int leaveYear, int leaveMonth);

        // By LocationId & UnitId
        Task<List<LeaveRequest>> GetLeaveRequestsByLocationIdnUnitIdAsync(int locationId, int unitId, int leaveYear);
        Task<List<LeaveRequest>> GetLeaveRequestsByLocationIdnUnitIdAsync(int locationId, int unitId, int leaveYear, int leaveMonth);
        #endregion

        #region Leave Requests By Leave Year & Leave Month
        // By Leave Year and Leave Months
        Task<List<LeaveRequest>> GetLeaveRequestsByLeaveYearAsync(int leaveYear);
        Task<List<LeaveRequest>> GetLeaveRequestsByLeaveYearnLeaveMonthAsync(int leaveYear, int leaveMonth);
        #endregion

        #region LeaveRequests By Resumption Dates
        Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearAsync(int leaveResumptionYear);
        Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthAsync(int leaveResumptionYear, int leaveResumptionMonth);
        Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnUnitIdAsync(int leaveResumptionYear, int leaveResumptionMonth, int unitId);
        Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnLocationIdAsync(int leaveResumptionYear, int leaveResumptionMonth, int locationId);
        Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionByResumptionYearnResumptionMonthnLocationIdnUnitIdAsync(int leaveResumptionYear, int leaveResumptionMonth, int locationId, int unitId);
        #endregion

        #region Approved Leave Leave Requests
        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLeaveMonthAsync(int leaveYear, int leaveMonth);
        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnLocationIdAsync(int leaveYear, int leaveMonth, int locationId);
        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnUnitIdAsync(int leaveYear, int leaveMonth, int unitId);
        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLeaveMonthnLocationIdnUnitIdAsync(int leaveYear, int leaveMonth, int locationId, int unitId);


        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnUnitIdAsync(int leaveYear, int unitId);
        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLocationIdAsync(int leaveYear, int locationId);
        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearnLocationIdnUnitIdAsync(int leaveYear, int locationId, int unitId);
        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByLeaveYearAsync(int leaveYear);
        #endregion
        #endregion

        #endregion

        #region Leave Submission Action Methods
        Task<bool> AddLeaveSubmissionAsync(LeaveSubmission e);
        Task<bool> DeleteSubmissionAsync(long id);
        Task<bool> UpdateSubmissionActionStatusAsync(long leaveSubmissionId, DateTime? timeActioned);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByLeaveSubmissionIdAsync(long leaveSubmissionId);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByToEmployeeNameAsync(string toEmployeeName);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByYearSubmittedAsync(string toEmployeeName, int yearSubmitted);


        Task<List<LeaveSubmission>> GetLeaveSubmissionsByRolenYearSubmittedAsync(string toEmployeeRole, int yearSubmitted);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByRequestIdnRolenPurposeAsync(long leaveRequestId, string toEmployeeRole, string purpose);
        #endregion

        #region Leave Approval Action Methods
        Task<long> AddLeaveApprovalAsync(LeaveApproval e);
        Task<bool> DeleteApprovalAsync(long leaveApprovalId);
        Task<List<LeaveApproval>> GetLeaveApprovalsByLeavePlanIdAsync(long leavePlanId);
        Task<List<LeaveApproval>> GetLeaveApprovalsByLeaveRequestIdAsync(long leaveRequestId);
        Task<LeaveApproval> GetApprovalByIdAsync(long leaveApprovalId);
        #endregion

        #region Leave Resumptions Action Methods

        Task<long> AddLeaveResumptionAsync(LeaveResumption e);
        Task<bool> DeleteLeaveResumptionAsync(long leaveResumptionId);
        Task<bool> UpdateLeaveResumptionByLineManagerAsync(long leaveResumptionId, string lineManagerName, DateTime resumptionDateByLineManager, int noOfExtraDaysByLineManager, int noOfUnusedLeaveDaysByLineManager, string commentsByLineManager, bool approvesAdjustment);

        Task<LeaveResumption> GetLeaveResumptionByLeaveResumptionIdAsync(long leaveResumptionId);
        Task<LeaveResumption> GetLeaveResumptionByLeaveRequestIdAsync(long leaveRequestId);

        #endregion

        #region Leave Document Action Interfaces
        Task<long> AddLeaveDocumentAsync(LeaveDocument e);
        Task<bool> DeleteLeaveDocumentAsync(long leaveDocumentId);
        Task<LeaveDocument> GetLeaveDocumentByIdAsync(long leaveDocumentId);
        Task<List<LeaveDocument>> GetLeaveDocumentsByLeaveRequestIdAsync(long leaveRequestId);
        #endregion

        #region Leave Adjustments Action Interfaces
        Task<long> AddLeaveAdjustmentAsync(LeaveAdjustment e);
        Task<bool> DeleteLeaveAdjustmentAsync(long leaveAdjustmentId);
        Task<LeaveAdjustment> GetLeaveAdjustmentByIdAsync(long leaveAdjustmentId);
        Task<List<LeaveAdjustment>> GetLeaveAdjustmentsByLeaveRequestIdAsync(long leaveRequestId);
        #endregion

        #region Leave Allowances Action Interfaces
        Task<long> AddLeaveAllowanceAsync(LeaveAllowance e);
        Task<bool> DeleteLeaveAllowanceAsync(long leaveAllowanceId);
        Task<LeaveAllowance> GetLeaveAllowanceByIdAsync(long leaveAllowanceId);
        Task<List<LeaveAllowance>> GetLeaveAllowanceByLeaveRequestIdAsync(long leaveRequestId);
        Task<List<LeaveAllowance>> GetLeaveAllowanceByEmployeeIdnLeaveYearAsync(string employeeId, int leaveYear);
        #endregion

        #region Leave Transactions Action Interfaces
        Task<long> AddLeaveTransactionAsync(LeaveTransaction t);
        Task<bool> DeleteLeaveTransactionAsync(long leaveTransactionId);
        Task<bool> DeleteLeaveTransactionByLeaveAdjustmentIdAsync(long leaveAdjustmentId);
        Task<LeaveTransaction> GetLeaveTransactionByIdAsync(long leaveTransactionId);
        Task<LeaveTransaction> GetLeaveTransactionByAdjustmentIdAsync(long leaveAdjustmentId);
        #endregion

        #region Leave Rolling Balances Action Interfaces
        Task<long> AddLeaveRollingBalanceAsync(LeaveRollingBalance t);
        Task<bool> UpdateLeaveRollingBalanceAsync(LeaveRollingBalance t);
        Task<bool> DeleteLeaveRollingBalanceAsync(long leaveRollingBalanceId);
        Task<LeaveRollingBalance> GetLeaveRollingBalanceByTransactionIdAsync(long leaveTransactionId);
        Task<LeaveRollingBalance> GetLeaveRollingBalanceByEmployeeIdAsync(string leaveEmployeeId, int leaveYear, string leaveTypeCode);
        #endregion

        #region Leave Activity Log Action Methods
        Task<List<LeaveActivityLog>> GetLeaveActivityLogByLeavePlanIdAsync(long leavePlanId);
        Task<List<LeaveActivityLog>> GetLeaveActivityLogByLeaveRequestIdAsync(long leaveRequestId);
        Task<bool> AddLeaveActivityLogAsync(LeaveActivityLog log);
        Task<bool> DeleteLeaveActivityLogAsync(long leaveActivityLogId);
        #endregion

        #region Leave Notes Log Action Interfaces
        Task<bool> AddNoteAsync(LeaveNote e);
        Task<List<LeaveNote>> GetNotesByLeavePlanIdAsync(long leavePlanId);
        Task<List<LeaveNote>> GetNotesByLeaveRequestIdAsync(long leaveRequestId);
        #endregion

        #region Leave Reports Action Interfaces
        
        //Leave Plan Compliance
        Task<List<LeavePlanCompliance>> GetLeavePlanComplianceByUnitsAsync(int leaveYear);
        Task<List<LeavePlanCompliance>> GetLeavePlanComplianceByDepartmentsAsync(int leaveYear);
        Task<List<LeavePlanCompliance>> GetLeavePlanComplianceByLocationsAsync(int leaveYear);

        //Leave Request Compliance
        Task<List<LeaveRequestCompliance>> GetLeaveRequestComplianceByLocationsAsync(int leaveYear);
        Task<List<LeaveRequestCompliance>> GetLeaveRequestComplianceByDepartmentsAsync(int leaveYear);
        Task<List<LeaveRequestCompliance>> GetLeaveRequestComplianceByUnitsAsync(int leaveYear);

        //Annual Leave Summary
        Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByEmployeeNameAsync(int leaveYear, string employeeName);
        Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByUnitIdAsync(int leaveYear, int unitId);
        Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByDepartmentIdAsync(int leaveYear, int unitId);
        Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByLocationIdAsync(int leaveYear, int unitId);
        Task<List<AnnualLeaveSummary>> GetAnnualLeaveSummaryByLocationIdnUnitIdAsync(int leaveYear, int locationId, int unitId);

        #endregion
    }
}
