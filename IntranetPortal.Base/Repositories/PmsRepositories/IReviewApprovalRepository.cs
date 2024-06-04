using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewApprovalRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewApproval reviewApproval);
        Task<bool> DeleteAsync(int reviewApprovalId);
        Task<bool> DeleteAsync(ReviewApproval reviewApproval);
        Task<IList<ReviewApproval>> GetAllAsync();
        Task<IList<ReviewApproval>> GetByIdAsync(int reviewApprovalId);
        Task<IList<ReviewApproval>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<IList<ReviewApproval>> GetByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId);
        Task<IList<ReviewApproval>> GetByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId, int approverRoleId);


        Task<IList<ReviewApproval>> GetApprovedByReviewHeaderIdAsync(int reviewHeaderId);
        Task<IList<ReviewApproval>> GetApprovedByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId);

        Task<IList<ReviewApproval>> GetApprovedByReviewHeaderIdAsync(int reviewHeaderId, int approvalTypeId, int approverRoleId);


    }
}