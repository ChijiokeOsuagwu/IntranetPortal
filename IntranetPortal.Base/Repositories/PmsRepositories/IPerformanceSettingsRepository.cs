using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IPerformanceSettingsRepository
    {
        IConfiguration _config { get; }

        Task<IList<AppraisalRecommendation>> GetAllRecommendationsAsync();
        Task<IList<ReviewType>> GetAllReviewTypesAsync();
    }
}