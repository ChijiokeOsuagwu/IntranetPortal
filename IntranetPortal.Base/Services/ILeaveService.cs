using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.LeaveModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface ILeaveService
    {
        #region Leave Types Service Interfaces
        Task<bool> CreateLeaveType(LeaveType leaveType);
        Task<bool> DeleteLeaveType(string code);
        Task<bool> UpdateLeaveType(LeaveType leaveType);
        Task<LeaveType> GetLeaveType(string LeaveTypeCode);
        Task<LeaveType> GetLeaveTypeByName(string Name);
        Task<List<LeaveType>> GetLeaveTypes(bool ExcludeSystem = true);
        #endregion

        #region Public Holiday Service Interfaces
        Task<bool> CreatePublicHoliday(PublicHoliday holiday);
        Task<bool> DeletePublicHoliday(int Id);
        Task<bool> UpdatePublicHoliday(PublicHoliday holiday);

        Task<PublicHoliday> GetPublicHoliday(int Id);
        Task<List<PublicHoliday>> GetPublicHolidays(int year);

        #endregion

        #region Leave Profiles Service Interfaces
        Task<List<LeaveProfile>> GetLeaveProfiles();
        Task<LeaveProfile> GetLeaveProfileByCode(string ProfileCode);
        Task<LeaveProfile> GetLeaveProfileByName(string ProfileName);
        Task<bool> CreateLeaveProfile(LeaveProfile leaveProfile);
        Task<bool> UpdateLeaveProfile(LeaveProfile leaveProfile);
        Task<bool> DeleteLeaveProfile(string ProfileCode);

        #endregion

        #region Leave Profile Details Service Interfaces
        //========= Read Method Interfaces ==============//
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(string LeaveProfileCode);
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(string LeaveProfileCode, string LeaveTypeCode);
        Task<LeaveProfileDetail> GetLeaveProfileDetail(int Id);

        //=========== Write Method Interfaces ===========//
        Task<bool> CreateLeaveProfileDetail(LeaveProfileDetail leaveProfileDetail);
        Task<bool> UpdateLeaveProfileDetail(LeaveProfileDetail d);
        Task<bool> DeleteLeaveProfileDetail(int Id);
        #endregion

        #region Leave Plans Service Interfaces
        Task<List<LeavePlan>> GetLeavePlansAsync(string EmployeeId, int LeaveYear);
        Task<LeavePlan> GetLeavePlanAsync(long LeavePlanId);
        Task<List<LeavePlan>> SearchLeavePlansAsync(int LeaveYear, int LeaveMonth, string EmployeeName = null, int? LocationId = null, int? UnitId = null, string EmployeeId = null);
        Task<List<LeavePlan>> SearchMyTeamsLeavePlansAsync(string TeamLeadId, int LeaveYear, int LeaveMonth, string EmployeeId = null);


        Task<long> CreateLeavePlanAsync(LeavePlan p);
        Task<bool> UpdateLeavePlanAsync(LeavePlan p);
        Task<bool> DeleteLeavePlanAsync(long id);
        Task<bool> UpdateLeavePlanFlagAsync(long LeavePlanId, bool IsFlagged, string FlagReason = null, string FlaggedBy = null);
        #endregion

        #region Leave Request Service Interfaces

        #region Leave Request Write Interfaces
        Task<long> CreateLeaveRequestAsync(LeaveRequest r);
        Task<bool> UpdateLeaveRequestAsync(LeaveRequest r);
        Task<bool> DeleteLeaveRequestAsync(long id);
        Task<bool> HrConfirmLeaveRequestAsync(long LeaveRequestId, string ConfirmedBy, DateTime ConfirmedTime);
        Task<bool> CloseLeaveRequestAsync(LeaveRequest r, string LeaveRequestClosedBy);
        #endregion

        #region Leave Request Read Interfaces
        Task<List<LeaveRequest>> GetLeaveRequestsAsync(string EmployeeId, int LeaveYear);
        Task<LeaveRequest> GetLeaveRequestAsync(long LeaveRequestId);
        Task<List<LeaveRequest>> SearchLeaveRequestsAsync(int LeaveYear, int LeaveMonth, string EmployeeName = null, int? LocationId = null, int? UnitId = null);
        Task<List<LeaveRequest>> SearchMyTeamsLeaveRequestsAsync(string TeamLeadId, int LeaveYear, int LeaveMonth, string EmployeeId = null, int? LeaveStatus = null);

        Task<List<LeaveRequest>> SearchApprovedLeaveRequestsAsync(int LeaveYear, int LeaveMonth = 0, int? LocationId = null, int? UnitId = null);

        Task<List<LeaveRequest>> GetLeaveRequestsDueResumptionAsync(int ResumptionYear, int ResumptionMonth, int? LocationId = null, int? UnitId = null);
        #endregion

        #endregion

        #region Leave Balances Service Interfaces
        Task<LeaveRollingBalance> GetRefreshedLeaveBalancesAsync(string LeaveTypeCode, int LeaveYear, string EmployeeId = null, string EmployeeName = null);
        Task<bool> RefreshAllEmployeesAnnualLeaveBalances(int LeaveYear);
        #endregion

        #region Leave Submission Service Interfaces
        Task<bool> SubmitLeaveAsync(LeaveSubmission e);
        Task<bool> DeleteLeaveSubmissionAsync(long LeaveSubmissionId);
        Task<LeaveSubmission> GetLeaveSubmissionByIdAsync(long LeaveSubmissionId);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByApproverIdAsync(string approverName, int? submittedYear = null);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByApproverRoleAsync(string approverRole, int? submittedYear = null);

        #endregion

        #region Leave Approval Service Methods
        Task<bool> ApproveLeaveAsync(LeaveApproval e, LeaveSubmission s, DocumentType t);
        Task<List<LeaveApproval>> GetLeaveApprovalsAsync(long? LeavePlanId = null, long? LeaveRequestId = null);

        Task<bool> DeclineLeaveAsync(LeaveApproval a, LeaveSubmission s, DocumentType t);
        #endregion

        #region Leave Documents Service Interfaces
        Task<bool> AddLeaveDocumentAsync(LeaveDocument document);
        Task<bool> DeleteLeaveDocumentAsync(long LeaveDocumentId);

        Task<LeaveDocument> GetLeaveDocumentAsync(long LeaveDocumentId);
        Task<List<LeaveDocument>> GetLeaveDocumentsAsync(long LeaveRequestId);
        #endregion

        #region Leave Resumption Service Interfaces
        Task<bool> SubmitLeaveResumptionNoticeAsync(LeaveResumption u, string SendToEmployeeName, string SendToEmployeeRole);
        Task<bool> ConfirmLeaveResumptionAsync(LeaveResumption u, long LeaveSubmissionId);
        Task<LeaveResumption> GetLeaveResumptionAsync(long LeaveRequestId, long LeaveResumptionId);
        #endregion

        #region Leave Adjustments Service Interfaces
        Task<List<LeaveAdjustment>> GetLeaveAdjustmentsAsync(long LeaveRequestId);
        Task<LeaveAdjustment> GetLeaveAdjustmentAsync(long LeaveAdjustmentId);

        Task<bool> AddLeaveAdjustmentAsync(LeaveAdjustment adjustment);
        Task<bool> DeleteLeaveAdjustmentAsync(long LeaveAdjustmentId, string DeletedBy);
        #endregion

        #region Leave Service Helper Interfaces
        DateTime GenerateLeaveEndDate(DateTime StartDate, int DurationTypeId, int Duration);
        int GetLeaveBalance(string EmployeeId, string LeaveTypeCode, int LeaveYear);
        int GetLeaveDuration(DateTime StartDate, DateTime ResumptionDate);

        #endregion

        #region Leave Notes & Activities Service Methods
        Task<List<LeaveNote>> GetLeavePlanNotesAsync(long LeavePlanId);
        Task<List<LeaveNote>> GetLeaveRequestNotesAsync(long LeaveRequestId);
        Task<bool> AddLeaveNoteAsync(LeaveNote e);

        Task<List<LeaveActivityLog>> GetLeaveActivitiesAsync(long? LeavePlanId = null, long? LeaveRequestId = null);
        #endregion

        #region Leave Reports Service Interfaces
        Task<List<LeavePlanCompliance>> GetLeavePlanComplianceAsync(int LeaveYear, ReportParameter parameter);
        Task<List<LeaveRequestCompliance>> GetLeaveRequestComplianceAsync(int LeaveYear, ReportParameter parameter);

        Task<List<AnnualLeaveSummary>> SearchAnnualLeaveSummaryAsync(int LeaveYear, int UnitId = 0, int DepartmentId = 0, int LocationId = 0, string EmployeeName = null);
        #endregion
    }
}
