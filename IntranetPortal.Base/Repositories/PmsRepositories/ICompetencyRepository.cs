using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface ICompetencyRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(Competency competency);
        Task<bool> DeleteAsync(int competencyId);
        Task<List<Competency>> GetByAllAsync();
        Task<List<Competency>> GetByCategoryIdAndLevelIdAsync(int categoryId, int levelId);
        Task<List<Competency>> GetByCategoryIdAsync(int categoryId);
        Task<List<Competency>> GetByIdAsync(int competencyId);
        Task<List<Competency>> GetByLevelIdAsync(int levelId);
        Task<bool> UpdateAsync(Competency competency);
    }
}