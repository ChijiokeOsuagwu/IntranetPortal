using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewSessionRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewSession reviewSession);
        Task<bool> DeleteAsync(int reviewSessionId);
        Task<IList<ReviewSession>> GetAllAsync();
        Task<IList<ReviewSession>> GetByIdAsync(int reviewSessionId);
        Task<IList<ReviewSession>> GetByNameAsync(string reviewSessionName);
        Task<IList<ReviewSession>> GetByYearIdAsync(int performanceYearId);
        Task<bool> UpdateAsync(ReviewSession reviewSession);
    }
}