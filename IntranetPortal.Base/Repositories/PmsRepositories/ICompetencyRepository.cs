using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface ICompetencyRepository
    {
        IConfiguration _config { get; }

        #region Competency Service Methods
        Task<bool> AddAsync(Competency competency);
        Task<bool> DeleteAsync(int competencyId);
        Task<List<Competency>> GetByAllAsync();
        Task<List<Competency>> GetByCategoryIdAndLevelIdAsync(int categoryId, int levelId);
        Task<List<Competency>> GetByCategoryIdAsync(int categoryId);
        Task<List<Competency>> GetByIdAsync(int competencyId);
        Task<List<Competency>> GetByTitleAsync(string competencyTitle);
        Task<List<Competency>> GetByLevelIdAsync(int levelId);
        Task<bool> UpdateAsync(Competency competency);

        #endregion

        #region Competency Category Service Methods
        Task<List<CompetencyCategory>> GetCompetencyCategoriesAsync();
        Task<CompetencyCategory> GetCompetencyCategoryByIdAsync(int competencyCategoryId);
        Task<CompetencyCategory> GetCompetencyCategoryByDescriptionAsync(string competencyCategoryDescription);
        Task<bool> AddCompetencyCategoryAsync(string competencyCategoryDescription);
        Task<bool> UpdateCompetencyCategoryAsync(int competencyCategoryId, string competencyCategoryDescription);
        Task<bool> DeleteCompetencyCategoryAsync(int competencyCategoryId);
        #endregion

        #region Competency Level Service Methods
        Task<List<CompetencyLevel>> GetCompetencyLevelsAsync();
        Task<CompetencyLevel> GetCompetencyLevelByIdAsync(int competencyLevelId);
        Task<CompetencyLevel> GetCompetencyLevelByDescriptionAsync(string competencyLevelDescription);
        Task<bool> AddCompetencyLevelAsync(string competencyLevelDescription);
        Task<bool> UpdateCompetencyLevelAsync(int competencyLevelId, string competencyLevelDescription);
        Task<bool> DeleteCompetencyLevelAsync(int competencyLevelId);
        #endregion
    }
}