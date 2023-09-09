using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface ITaskEvaluationDetailRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(TaskEvaluationDetail taskEvaluationDetail);
        Task<List<TaskEvaluationDetail>> GetByTaskEvaluationHeaderIdAsync(int taskEvaluationHeaderId);
        Task<List<TaskEvaluationDetail>> GetByTaskEvaluationHeaderIdAndTaskItemIdAsync(int taskEvaluationHeaderId, long taskItemId);
        Task<bool> UpdateAsync(TaskEvaluationDetail taskEvaluationDetail);
    }
}