using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewResultRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewResult reviewResult);
        Task<bool> AddSummaryAsync(ResultSummary resultSummary);
        Task<bool> DeleteSummaryAsync(int reviewHeaderId);
        Task<List<string>> GetAppraisersByReviewHeaderId(int reviewHeaderId);
        Task<List<AppraiserDetail>> GetAppraisersDetailsByReviewHeaderId(int reviewHeaderId);
        Task<IList<ReviewResult>> GetByAppraiserIdAndMetricId(int reviewHeaderId, string appraiserId, int reviewMetricId);
        Task<IList<ReviewResult>> GetByAppraiserIdAndMetricTypeId(int reviewHeaderId, string appraiserId, int reviewMetricTypeId);
        Task<IList<ReviewResult>> GetByAppraiserIdAndReviewHeaderId(int reviewHeaderId, string appraiserId);
        Task<IList<ReviewResult>> GetById(int reviewResultId);
        Task<IList<ReviewResult>> GetIntitalByMetricIdAsync(int reviewHeaderId, string appraiserId, int metricId);
        Task<IList<ReviewResult>> GetIntitalByMetricTypeIdAsync(int reviewHeaderId, string appraiserId, int metricTypeId);
        Task<IList<ReviewResult>> GetIntitalByThirdPartyAsync(int reviewHeaderId, string appraiserId, int metricTypeId);
        Task<IList<ResultDetail>> GetPrincipalResultDetailByDepartmentCodeAndReviewSessionIdAsync(int reviewSessionId, int departmentId);
        Task<IList<ResultDetail>> GetPrincipalResultDetailByLocationIdAndReviewSessionIdAsync(int reviewSessionId, int locationId);
        Task<IList<ResultDetail>> GetPrincipalResultDetailByReviewSessionIdAsync(int reviewSessionId);
        Task<IList<ResultDetail>> GetPrincipalResultDetailByUnitCodeAndReviewSessionIdAsync(int reviewSessionId, int unitId);
        Task<List<ReviewScore>> GetScoresByReviewHeaderIdAndAppraiserIdAsync(int reviewHeaderId, string appraiserId);
        Task<IList<ResultSummary>> GetSummaryByAppraiserIdAndReviewHeaderId(int reviewHeaderId, string appraiserId);
        Task<IList<ResultSummary>> GetSummaryByReportToId(string reportToId, int reviewSessionId);
        Task<IList<ResultSummary>> GetSummaryByReportToIdAndAppraiseeId(string reportToId, int reviewSessionId, string appraiseeId);
        Task<IList<ResultSummary>> GetSummaryByReviewSessionId(int reviewSessionId);
        Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndAppraiseeName(int reviewSessionId, string appraiseeName);
        Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndDepartmentCode(int reviewSessionId, int departmentId);
        Task<IList<ResultSummary>> GetSummaryByReviewSessionIdAndUnitCode(int reviewSessionId, int unitId);
        Task<bool> UpdateAsync(ReviewResult reviewResult);
        Task<bool> UpdateSummaryAsync(ResultSummary resultSummary);
    }
}