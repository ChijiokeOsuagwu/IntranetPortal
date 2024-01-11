using IntranetPortal.Base.Enums;
using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IAppraisalGradeRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(AppraisalGrade appraisalGrade);
        Task<bool> CopyAsync(string copiedBy, int reviewSessionId, int gradeTemplateId);
        Task<bool> CopyAsync(string copiedBy, int reviewSessionId, int gradeTemplateId, ReviewGradeType gradeType);
        Task<bool> DeleteAsync(int appraisalGradeId);
        Task<IList<AppraisalGrade>> GetAllAsync();
        Task<IList<AppraisalGrade>> GetByIdAsync(int appraisalGradeId);
        Task<IList<AppraisalGrade>> GetByNameAsync(string appraisalGradeName);
        Task<IList<AppraisalGrade>> GetByReviewSessionIdAndGradeScoreAsync(int reviewSessionId, ReviewGradeType gradeType, decimal gradeScore);
        Task<IList<AppraisalGrade>> GetByReviewSessionIdAsync(int reviewSessionId);
        Task<IList<AppraisalGrade>> GetByReviewSessionIdAsync(int reviewSessionId, ReviewGradeType gradeType);
        Task<bool> UpdateAsync(AppraisalGrade appraisalGrade);
    }
}