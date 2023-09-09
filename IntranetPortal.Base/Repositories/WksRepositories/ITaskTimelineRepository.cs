using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface ITaskTimelineRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(TaskTimelineChange taskTimelineChange);
        Task<TaskTimelineChange> GetByIdAsync(long taskTimelineChangeId);
        Task<List<TaskTimelineChange>> GetByTaskItemIdAsync(long taskItemId);
    }
}