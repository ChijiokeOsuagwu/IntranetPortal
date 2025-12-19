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
        Task<List<LeaveProfileDetail>> GetLeaveProfileDetailsByEmployeeIdnLeaveTypeAsync(string employeeId);
        Task<LeaveProfileDetail> GetLeaveProfileDetailByEmployeeIdnLeaveTypeAsync(string employeeId, string leaveTypeCode);

        Task<bool> AddLeaveProfileDetailAsync(LeaveProfileDetail leaveProfileDetail);
        Task<bool> DeleteLeaveProfileDetailAsync(int leaveProfileDetailId);
        Task<bool> EditLeaveProfileDetailAsync(LeaveProfileDetail leaveProfileDetail);
        #endregion
    }
}
