using IntranetPortal.Base.Models.WksModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.WksRepositories
{
    public interface ITaskItemEvaluationRepository
    {
        IConfiguration _config { get; }

        Task<List<TaskItemEvaluation>> GetByTaskListIdAndEvaluatorIdAsync(int taskListId, string evaluatorId);
    }
}
