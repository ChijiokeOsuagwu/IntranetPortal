using IntranetPortal.Base.Models.ClmModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ClmRepositories
{
    public interface ISubjectAreaRepository
    {
        IConfiguration _config { get; }

        Task<IList<SubjectArea>> GetAllAsync();
        Task<IList<SubjectArea>> GetByIdAsync(int subjectAreaId);
        Task<IList<SubjectArea>> GetByDescriptionAsync(string subjectAreaDescription);
        Task<bool> AddAsync(SubjectArea subjectArea);
        Task<bool> UpdateAsync(SubjectArea subjectArea);
        Task<bool> DeleteAsync(int subjectAreaId);
    }
}
