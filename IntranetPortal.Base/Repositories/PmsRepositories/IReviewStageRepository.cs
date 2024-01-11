using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewStageRepository
    {
        IConfiguration _config { get; }

        Task<IList<ReviewStage>> GetAllAsync();
        Task<IList<ReviewStage>> GetAllPreviousAsync(int currentStageId);
    }
}