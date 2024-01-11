using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IPmsSystemRepository
    {
        IConfiguration _config { get; }

        Task<List<CompetencyCategory>> GetAllCompetencyCategoriesAsync();
        Task<List<CompetencyLevel>> GetAllCompetencyLevelsAsync();
    }
}