using IntranetPortal.Base.Models.ClmModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ClmRepositories
{
    public interface ICourseTypeRepository
    {
        IConfiguration _config { get; }

        Task<IList<CourseType>> GetAllAsync();
        Task<IList<CourseType>> GetByIdAsync(int courseTypeId);
        Task<IList<CourseType>> GetByDescriptionAsync(string courseTypeDescription);
        Task<bool> AddAsync(CourseType courseType);
        Task<bool> UpdateAsync(CourseType courseType);
        Task<bool> DeleteAsync(int courseTypeId);
    }
}
