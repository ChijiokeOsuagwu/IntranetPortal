using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IReviewGradeRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(ReviewGrade reviewGrade);
        Task<bool> DeleteAsync(int reviewGradeId);
        Task<IList<ReviewGrade>> GetAllAsync();
        Task<IList<ReviewGrade>> GetByGradeHeaderIdAsync(int gradeHeaderId);
        Task<IList<ReviewGrade>> GetByGradeHeaderIdAsync(int gradeHeaderId, ReviewGradeType gradeType);
        Task<IList<ReviewGrade>> GetByIdAsync(int reviewGradeId);
        Task<IList<ReviewGrade>> GetByNameAsync(string reviewGradeName);
        Task<bool> UpdateAsync(ReviewGrade reviewGrade);
    }
}