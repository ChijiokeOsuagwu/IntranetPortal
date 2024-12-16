using IntranetPortal.Base.Models.LmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.LmsRepositories
{
    public interface IEmployeeLeaveRepository
    {
        Task<bool> AddAsync(EmployeeLeave e);
        Task<bool> DeleteAsync(long id);
        Task<bool> EditAsync(EmployeeLeave e);
        Task<EmployeeLeave> GetByIdAsync(long id);
        Task<List<EmployeeLeave>> GetByEmployeeIdnYearAsync(string employeeId, int year, bool getPlan);
    }
}