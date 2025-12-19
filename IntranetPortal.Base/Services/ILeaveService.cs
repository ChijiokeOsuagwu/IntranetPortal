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
        Task<LeaveProfile> GetLeaveProfile(int Id);
        Task<LeaveProfile> GetLeaveProfile(string Name);
        Task<bool> CreateLeaveProfile(LeaveProfile leaveProfile);
        Task<bool> UpdateLeaveProfile(LeaveProfile leaveProfile);
        Task<bool> DeleteLeaveProfile(int Id);

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

    }
}
