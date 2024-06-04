using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewHeaderRepository
    {
        IConfiguration _config { get; }

        #region ReviewHeader Read Methods
        Task<IList<ReviewHeader>> GetByAppraiseeIdAndReviewSessionIdAsync(string appraiseeId, int reviewSessionId);
        Task<IList<ReviewHeader>> GetByIdAsync(int reviewHeaderId);
        Task<IList<ReviewHeader>> GetByAppraiseeNameAsync(string appraiseeName);
                Task<IList<ReviewHeader>> GetByReviewSessionIdAsync(int reviewSessionId);
        Task<IList<ReviewHeader>> GetByReviewSessionIdnAppraiseeNameAsync(int reviewSessionId, string appraiseeName);
        Task<IList<ReviewHeader>> GetByReviewSessionIdnStageIdAsync(int reviewSessionId, int reviewStageId);
        Task<IList<ReviewHeader>> GetByReviewSessionIdnStageIdnLocationIdAsync(int reviewSessionId, int reviewStageId, int locationId);
        Task<IList<ReviewHeader>> GetByReviewSessionIdnStageIdnLocationIdnUnitIdAsync(int reviewSessionId, int reviewStageId, int locationId, int unitId);
        Task<IList<ReviewHeader>> GetByReviewSessionIdnStageIdnLocationIdnUnitIdnAppraiseeNameAsync(int reviewSessionId, int reviewStageId, int locationId, int unitId, string appraiseeName);
        #endregion

        #region ReviewHeader Write Methods
        Task<bool> AddAsync(ReviewHeader reviewHeader);
        Task<bool> UpdateAppraiseeFlagAsync(int reviewHeaderId, bool isFlagged, string flaggedReason);
        Task<bool> UpdateContractAcceptanceAsync(int reviewHeaderId, bool isAccepted);
        Task<bool> UpdateDepartmentHeadRecommendationAsync(int reviewHeaderId, string deptHeadName, string recommendedAction, string remarks);
        Task<bool> UpdateEvaluationAcceptanceAsync(int reviewHeaderId, bool isAccepted);
        Task<bool> UpdateFeedbackAsync(int reviewHeaderId, string feedbackProblems, string feedbackSolutions);
        Task<bool> UpdateGoalAsync(int reviewHeaderId, string performanceGoal, string appraiserId);
        Task<bool> UpdateHrRecommendationAsync(int reviewHeaderId, string hrRepName, string recommendedAction, string remarks);
        Task<bool> UpdateLineManagerRecommendationAsync(int reviewHeaderId, string lineManagerName, string recommendedAction, string remarks);
        Task<bool> UpdateManagementDecisionAsync(int reviewHeaderId, string mgtRepName, string recommendedAction, string remarks);
        Task<bool> UpdateStageIdAsync(int reviewHeaderId, int nextStageId);
        Task<bool> UpdateUnitHeadRecommendationAsync(int reviewHeaderId, string unitHeadName, string recommendedAction, string remarks);
        #endregion
    }
}