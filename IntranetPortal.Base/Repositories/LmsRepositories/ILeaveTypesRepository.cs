using IntranetPortal.Base.Models.LmsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.LmsRepositories
{
    public interface ILeaveTypesRepository
    {
        Task<bool> AddAsync(LeaveType leaveType);
        Task<bool> DeleteAsync(string code);
        Task<bool> EditAsync(LeaveType leaveType);
        Task<List<LeaveType>> GetAllAsync();
        Task<List<LeaveType>> GetAllExcludingSystemAsync();
        Task<LeaveType> GetByCodeAsync(string code);
        Task<LeaveType> GetByNameAsync(string name);
    }
}