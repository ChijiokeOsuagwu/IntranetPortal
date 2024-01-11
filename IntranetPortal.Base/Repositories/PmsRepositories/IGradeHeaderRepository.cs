using IntranetPortal.Base.Models.PmsModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.PmsRepositories
{
    public interface IGradeHeaderRepository
    {
        IConfiguration _config { get; }

        Task<bool> AddAsync(GradeHeader gradeHeader);
        Task<bool> DeleteAsync(int gradeHeaderId);
        Task<IList<GradeHeader>> GetAllAsync();
        Task<IList<GradeHeader>> GetByIdAsync(int gradeHeaderId);
        Task<IList<GradeHeader>> GetByNameAsync(string gradeHeaderName);
        Task<bool> UpdateAsync(GradeHeader gradeHeader);
    }
}