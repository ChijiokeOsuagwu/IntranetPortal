using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewSubmissionRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewSubmission reviewSubmission);
        Task<bool> DeleteAsync(int reviewSubmissionId);
        Task<List<ReviewSubmission>> GetByIdAsync(int reviewSubmissionId);
        Task<List<ReviewSubmission>> GetByReviewerIdAndReviewSessionIdAsync(string reviewerId, int reviewSessionId);
        Task<List<ReviewSubmission>> GetByReviewerIdAsync(string reviewerId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAndSubmissionPurposeIdAsync(int reviewHeaderId, int submissionPurposeId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAndSubmissionPurposeIdAsync(int reviewHeaderId, int submissionPurposeId, string appraiserId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<bool> UpdateAsync(int reviewSubmissionId);
        Task<bool> UpdateAsync(int reviewHeaderId, string toEmployeeId, int submissionPurposeId);
    }
}