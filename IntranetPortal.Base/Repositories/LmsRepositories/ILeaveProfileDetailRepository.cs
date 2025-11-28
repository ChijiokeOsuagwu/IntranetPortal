using IntranetPortal.Base.Models.LmsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.LmsRepositories
{
    public interface ILeaveProfileDetailRepository
    {
        #region Write Action Methods
        Task<bool> AddAsync(LeaveProfileDetail leaveProfileDetail);
        Task<bool> DeleteAsync(int id);
        Task<bool> EditAsync(LeaveProfileDetail leaveProfileDetail);
        #endregion

        #region Read Action Methods
        Task<List<LeaveProfileDetail>> GetByIdAsync(int id);
        Task<List<LeaveProfileDetail>> GetByProfileIdAsync(int profileId);
        Task<List<LeaveProfileDetail>> GetByProfileIdnLeaveTypeAsync(int profileId, string leaveTypeCode);

        Task<List<LeaveProfileDetail>> GetByEmployeeIdnLeaveTypeAsync(string employeeId);
        Task<LeaveProfileDetail> GetByEmployeeIdnLeaveTypeAsync(string employeeId, string leaveTypeCode);
        #endregion
    }
}