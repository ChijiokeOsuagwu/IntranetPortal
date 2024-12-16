using IntranetPortal.Base.Models.LmsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.LmsRepositories
{
    public interface ILeaveProfileRepository
    {
        Task<bool> AddAsync(LeaveProfile leaveProfile);
        Task<bool> DeleteAsync(int id);
        Task<bool> EditAsync(LeaveProfile leaveProfile);
        Task<List<LeaveProfile>> GetAllAsync();
        Task<LeaveProfile> GetByIdAsync(int id);
        Task<LeaveProfile> GetByNameAsync(string profileName);
    }
}