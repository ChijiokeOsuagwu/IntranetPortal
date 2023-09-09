using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface ITaskSubmissionRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(TaskSubmission taskSubmission);
        Task<bool> UpdateAsync(long taskSubmissionId);
        Task<bool> DeleteAsync(long taskSubmissionId);
        Task<TaskSubmission> GetByIdAsync(long taskSubmissionId);
        Task<List<TaskSubmission>> GetByToEmployeeIdAndTaskListIdAsync(string toEmployeeId, int taskListId);
        Task<List<TaskSubmission>> GetByToEmployeeIdAsync(string toEmployeeId);
        Task<List<TaskSubmission>> GetByToEmployeeIdAndSubmittedYearAsync(string toEmployeeId, int submittedYear);
        Task<List<TaskSubmission>> GetByToEmployeeIdAndSubmittedYearAndSubmittedMonthAsync(string toEmployeeId, int submittedYear, int submittedMonth);
        Task<List<TaskSubmission>> GetByToEmployeeIdAndFromEmployeeNameAsync(string toEmployeeId, string fromEmployeeName);
        Task<List<TaskSubmission>> GetByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAsync(string toEmployeeId, string fromEmployeeName, int submittedYear);
        Task<List<TaskSubmission>> GetByToEmployeeIdAndFromEmployeeNameAndSubmittedYearAndSubmittedMonthAsync(string toEmployeeId, string fromEmployeeName, int submittedYear, int submittedMonth);
    }
}