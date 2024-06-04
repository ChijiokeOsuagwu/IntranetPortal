using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewMetricRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewMetric reviewMetric);
        Task<bool> DeleteAsync(int reviewMetricId);
        Task<List<ReviewMetric>> GetByIdAsync(int reviewMetricId);
        Task<List<ReviewMetric>> GetByMetricDescriptionAsync(int reviewHeaderId, string metricDescription);
        Task<List<ReviewMetric>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<int> GetCmpCountByReviewHeaderIdAsync(int reviewHeaderId);
        Task<int> GetKpaCountByReviewHeaderIdAsync(int reviewHeaderId);
        Task<List<ReviewMetric>> GetCmpsByReviewHeaderIdAsync(int reviewHeaderId);
        Task<List<ReviewMetric>> GetKpasByReviewHeaderIdAsync(int reviewHeaderId);
        Task<decimal> GetTotalCmpWeightageByReviewHeaderIdAsync(int reviewHeaderId);
        Task<decimal> GetTotalKpaWeightageByReviewHeaderIdAsync(int reviewHeaderId);
        Task<decimal> GetTotalWeightageByReviewHeaderIdAsync(int reviewHeaderId);
        Task<IList<ReviewMetric>> GetUnevaluatedByMetricTypeIdAsync(int reviewHeaderId, string appraiserId, int metricTypeId);
        Task<bool> UpdateAsync(ReviewMetric reviewMetric);
    }
}