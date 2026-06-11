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
        Task<bool> DeleteLeaveProfileAsync(int id);
        Task<bool> EditLeaveProfileAsync(LeaveProfile leaveProfile);
        Task<LeaveProfile> GetLeaveProfileByIdAsync(int id);
        Task<LeaveProfile> GetLeaveProfileByNameAsync(string profileName);
        Task<List<LeaveProfile>> GetAllLeaveProfilesAsync();
        #endregion

        #region LeaveProfileDetails Action Interfaces
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByProfileIdAsync(int profileId);
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetailByIdAsync(int leaveProfileDetailId);
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByProfileIdnLeaveTypeAsync(int profileId, string leaveTypeCode);
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
        Task<bool> UpdateLeavePlanStatusAsync(long leavePlanId,  int newLeaveStatus);
        #endregion

        #region Leave Plans By Id & Employee ID & Name
        Task<LeavePlan> GetLeavePlanByIdAsync(long leavePlanId);
        Task<List<LeavePlan>> GetLeavePlansByEmployeeIdAsync(string employeeId, int leaveYear);

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

        #endregion

        #region Leave Requests Action Methods
        #region Leave Requests Write Action Methods
        Task<long> AddLeaveRequestAsync(LeaveRequest r);
        Task<bool> DeleteLeaveRequestAsync(long leaveRequestId);
        Task<bool> EditLeaveRequestAsync(LeaveRequest r);
        Task<bool> UpdateLeaveRequestStatusAsync(long leaveRequestId, int newStatus);
        Task<bool> UpdateLeaveRequestApprovalStatusAsync(long leaveRequestId, ApprovalType approvalType);
        #endregion

        #region Leave Requests Read Action Methods
        // By LeaveRequestId & Employee Id & Name
        Task<LeaveRequest> GetLeaveRequestByIdAsync(long leaveRequestId);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(string employeeId, int leaveYear);

        // By Employee Name
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName, int leaveYear);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeNameAsync(string employeeName, int leaveYear, int leaveMonth);


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

        #endregion

        #endregion

        #region Leave Balances Action Interfaces
        Task<long> GetLeaveDaysUsedByEmployeeIdnLeaveTypeCodenLeaveYearAsync(string employeeId, string leaveTypeCode, int leaveYear);
        Task<long> GetLeaveDaysUsedByEmployeeNamenLeaveTypeCodenLeaveYearAsync(string employeeName, string leaveTypeCode, int leaveYear);
        #endregion

        #region Leave Submission Action Methods
        Task<bool> AddLeaveSubmissionAsync(LeaveSubmission e);
        Task<bool> DeleteSubmissionAsync(long id);
        Task<bool> UpdateSubmissionActionStatusAsync(long leaveSubmissionId, DateTime? timeActioned);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByLeaveSubmissionIdAsync(long leaveSubmissionId);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByToEmployeeNameAsync(string toEmployeeName);
        Task<List<LeaveSubmission>> GetLeaveSubmissionsByYearSubmittedAsync(string toEmployeeName, int yearSubmitted);


        Task<List<LeaveSubmission>> GetLeaveSubmissionsByRolenYearSubmittedAsync(string toEmployeeRole, int yearSubmitted);
        #endregion

        #region Leave Approval Action Methods
        Task<long> AddLeaveApprovalAsync(LeaveApproval e);
        Task<bool> DeleteApprovalAsync(long leaveApprovalId);
        Task<List<LeaveApproval>> GetLeaveApprovalsByLeavePlanIdAsync(long leavePlanId);
        Task<List<LeaveApproval>> GetLeaveApprovalsByLeaveRequestIdAsync(long leaveRequestId);
        Task<LeaveApproval> GetApprovalByIdAsync(long leaveApprovalId);
        #endregion

        #region Leave Document Action Interfaces
        Task<long> AddLeaveDocumentAsync(LeaveDocument e);
        Task<bool> DeleteLeaveDocumentAsync(long leaveDocumentId);
        Task<LeaveDocument> GetLeaveDocumentByIdAsync(long leaveDocumentId);
        Task<List<LeaveDocument>> GetLeaveDocumentsByLeaveRequestIdAsync(long leaveRequestId);
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
    }
}
