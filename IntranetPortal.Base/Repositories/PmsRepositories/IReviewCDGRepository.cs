using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewCDGRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewCDG reviewCDG);
        Task<bool> DeleteAsync(int reviewCdgId);
        Task<List<ReviewCDG>> GetByIdAsync(int reviewCdgId);
        Task<List<ReviewCDG>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<bool> UpdateAsync(ReviewCDG reviewCdg);
    }
}