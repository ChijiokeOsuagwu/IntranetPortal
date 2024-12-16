using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewCDGRepository
    {
        IConfiguration _config { get; }

        #region Review CDG Action Methods
        Task<bool> AddAsync(ReviewCDG reviewCDG);
        Task<bool> DeleteAsync(int reviewCdgId);
        Task<bool> UpdateAsync(ReviewCDG reviewCdg);
        #endregion

        #region Review CDG Read Action Methods
        Task<List<ReviewCDG>> GetByIdAsync(int reviewCdgId);
        Task<List<ReviewCDG>> GetByReviewSessionIdnEmployeeNameAsync(int reviewSessionId, string employeeName);
        Task<List<ReviewCDG>> GetByReviewHeaderIdAsync(int reviewHeaderId);
        Task<List<ReviewCDG>> GetByReviewSessionIdAsync(int reviewSessionId);
        Task<List<ReviewCDG>> GetByReviewSessionIdnLocationIdAsync(int reviewSessionId, int locationId);
        Task<List<ReviewCDG>> GetByReviewSessionIdnLocationIdnDepartmentIdAsync(int reviewSessionId, int locationId, int departmentId);
        Task<List<ReviewCDG>> GetByReviewSessionIdnLocationIdnDepartmentIdnUnitIdAsync(int reviewSessionId, int locationId, int departmentId, int unitId);
        Task<List<ReviewCDG>> GetByReviewSessionIdnDepartmentIdnUnitIdAsync(int reviewSessionId, int departmentId, int unitId);
        Task<List<ReviewCDG>> GetByReviewSessionIdnUnitIdAsync(int reviewSessionId, int unitId);
        Task<List<ReviewCDG>> GetByReviewSessionIdnLocationIdnUnitIdAsync(int reviewSessionId, int locationId, int unitId);


        #endregion
    }
}