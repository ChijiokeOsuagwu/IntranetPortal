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
        Task<List<ReviewSubmission>> GetByReviewerIdAndReviewSessionIdAsync(int reviewerId, int reviewSessionId);
        Task<List<ReviewSubmission>> GetByReviewerIdAsync(int reviewerId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAndSubmissionPurposeIdAsync(int reviewHeaderId, int submissionPurposeId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAndSubmissionPurposeIdAsync(int reviewHeaderId, int submissionPurposeId, int appraiserId);
        Task<List<ReviewSubmission>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<bool> UpdateAsync(int reviewSubmissionId);
        Task<bool> UpdateAsync(int reviewHeaderId, int toEmployeeId, int submissionPurposeId);
    }
}