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
        Task<List<LeaveType>> GetLeaveTypes();
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

        #region Employee Leave Plan Service Interfaces
        Task<List<EmployeeLeave>> GetLeavePlans(string EmployeeId, int LeaveYear);
        Task<EmployeeLeave> GetLeavePlan(long Id);
        Task<bool> CreateLeavePlan(EmployeeLeave e);
        #endregion

        #region LMS Helper Service Interfaces
        DateTime GenerateLeaveEndDate(DateTime StartDate, int DurationTypeId, int Duration);
        #endregion
    }
}