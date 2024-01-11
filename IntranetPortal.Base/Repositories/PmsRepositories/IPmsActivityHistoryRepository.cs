using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IPmsActivityHistoryRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(PmsActivityHistory activityHistory);
        Task<IList<PmsActivityHistory>> GetByReviewHeaderIdAsync(int reviewHeaderId);
    }
}