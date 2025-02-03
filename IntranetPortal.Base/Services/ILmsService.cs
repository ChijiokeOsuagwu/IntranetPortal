using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.LmsModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface ILmsService
    {
        #region Leave Types Service Interfaces
        Task<bool> CreateLeaveType(LeaveType leaveType);
        Task<bool> DeleteLeaveType(string code);
        Task<bool> UpdateLeaveType(LeaveType leaveType);
        Task<LeaveType> GetLeaveType(string LeaveTypeCode);
        Task<LeaveType> GetLeaveTypeByName(string Name);
        Task<List<LeaveType>> GetLeaveTypes(bool ExcludeSystem = true);
        #endregion

        #region Leave Profile Service Interfaces
        Task<bool> CreateLeaveProfile(LeaveProfile leaveProfile);
        Task<bool> DeleteLeaveProfile(int Id);
        Task<bool> UpdateLeaveProfile(LeaveProfile leaveProfile);

        Task<LeaveProfile> GetLeaveProfile(int Id);
        Task<LeaveProfile> GetLeaveProfile(string Name);
        Task<List<LeaveProfile>> GetLeaveProfiles();
        #endregion

        #region Public Holiday Service Interfaces
        Task<bool> CreatePublicHoliday(PublicHoliday holiday);
        Task<bool> DeletePublicHoliday(int Id);
        Task<bool> UpdatePublicHoliday(PublicHoliday holiday);

        Task<PublicHoliday> GetPublicHoliday(int Id);
        Task<List<PublicHoliday>> GetPublicHolidays(int year);

        #endregion

        #region Leave Profile Details Service Interfaces
        //========= Read Method Interfaces ==============//
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(int LeaveProfileId);
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetails(int LeaveProfileId, string LeaveTypeCode);
        Task<LeaveProfileDetail> GetLeaveProfileDetail(int Id);

        //=========== Write Method Interfaces ===========//
        Task<bool> CreateLeaveProfileDetail(LeaveProfileDetail leaveProfileDetail);
        Task<bool> UpdateLeaveProfileDetail(LeaveProfileDetail d);
        Task<bool> DeleteLeaveProfileDetail(int Id);
        #endregion

        #region Employee Leave Write Service Interfaces
        Task<long> CreateLeaveAsync(EmployeeLeave e);
        Task<bool> UpdateLeaveAsync(EmployeeLeave e);
        Task<bool> DeleteLeaveAsync(long id);
        Task<bool> UpdateLeaveStatusAsync(long LeaveId, string NewStatus);
        #endregion

        #region Employee Leaves Read Service Interfaces
        Task<List<EmployeeLeave>> GetEmployeeLeavesAsync(string EmployeeId, int LeaveYear, bool IsPlan);
        Task<EmployeeLeave> GetEmployeeLeaveAsync(long Id);
        Task<List<EmployeeLeave>> SearchMyTeamsEmployeeLeavesAsync(string TeamLeadId, int LeaveYear, int LeaveMonth, string EmployeeId = null, string LeaveStatus = null, bool IsPlan = true);
        Task<List<EmployeeLeave>> SearchAllEmployeeLeavesAsync(int LeaveYear, int LeaveMonth, string EmployeeName = null, string LeaveStatus = null, bool IsPlan = true);
        #endregion

        #region Leave Submission Service Interfaces
        Task<bool> SubmitLeaveAsync(LeaveSubmission e, string DocumentType);
        #endregion

        #region Leave Approval Service Interfaces
        Task<bool> ApproveLeaveAsync(LeaveApproval e, string DocumentType);
        Task<List<LeaveApproval>> GetLeaveApprovalsAsync(long LeaveId);
        #endregion

        #region Leave Notes Service Interfaces
        Task<List<LeaveNote>> GetLeaveNotesAsync(long LeaveId);
        Task<bool> AddLeaveNoteAsync(LeaveNote e);
        #endregion

        #region Leave Activities Service Interfaces
        Task<List<LeaveActivityLog>> GetLeaveActivitiesAsync(long LeaveId);
        Task<bool> AddActivityLogAsync(LeaveActivityLog e);
        #endregion

        #region Leave Documents Service Interfaces
        Task<List<LeaveDocument>> GetLeaveDocumentsAsync(long LeaveId);
        Task<LeaveDocument> GetLeaveDocumentAsync(long DocumentId);
        Task<bool> AddLeaveDocumentAsync(LeaveDocument e);
        Task<bool> DeleteLeaveDocumentAsync(long LeaveDocumentId);
        #endregion

        #region LMS Helper Service Interfaces
        DateTime GenerateLeaveEndDate(DateTime StartDate, int DurationTypeId, int Duration);
        #endregion
    }
}