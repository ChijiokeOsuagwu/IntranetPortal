using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewMessageRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewMessage reviewMessage);
        Task<bool> DeleteAsync(int reviewMessageId);
        Task<ReviewMessage> GetByIdAsync(int reviewMessageId);
        Task<List<ReviewMessage>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<bool> UpdateAsync(ReviewMessage reviewMessage);
    }
}