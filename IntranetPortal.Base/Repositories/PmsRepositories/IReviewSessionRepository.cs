using IntranetPortal.Base.Models.EmployeeRecordModels;
using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewSessionRepository
    {
        IConfiguration _config { get; }

        #region Review Session Action Interfaces
        Task<bool> AddAsync(ReviewSession reviewSession);
        Task<bool> DeleteAsync(int reviewSessionId);
        Task<IList<ReviewSession>> GetAllAsync();
        Task<IList<ReviewSession>> GetByIdAsync(int reviewSessionId);
        Task<IList<ReviewSession>> GetByNameAsync(string reviewSessionName);
        Task<IList<ReviewSession>> GetByYearIdAsync(int performanceYearId);
        Task<bool> UpdateAsync(ReviewSession reviewSession);
        #endregion

        #region Appraisal Non Participants Action Interfaces
        Task<IList<Employee>> GetNonParticipantsByReviewSessionIdAsync(int reviewSessionId);
        Task<IList<Employee>> GetNonParticipantsByReviewSessionIdnLocationIdAsync(int reviewSessionId, int locationId);
        Task<IList<Employee>> GetNonParticipantsByReviewSessionIdnLocationIdnDepartmentIdnUnitIdAsync(int reviewSessionId, int locationId, int deptId, int unitId);
        Task<IList<Employee>> GetNonParticipantsByReviewSessionIdnLocationIdnDepartmentIdAsync(int reviewSessionId, int locationId, int deptId);
        Task<IList<Employee>> GetNonParticipantsByReviewSessionIdnLocationIdnUnitIdAsync(int reviewSessionId, int locationId, int unitId);
        Task<IList<Employee>> GetNonParticipantsByReviewSessionIdnDepartmentIdnUnitIdAsync(int reviewSessionId, int deptId, int unitId);
        Task<IList<Employee>> GetNonParticipantsByReviewSessionIdnDepartmentIdAsync(int reviewSessionId, int deptId);
        Task<IList<Employee>> GetNonParticipantsByReviewSessionIdnUnitIdAsync(int reviewSessionId, int unitId);
        #endregion
    }
}