using IntranetPortal.Base.Models.ClmModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ClmRepositories
{
    public interface ICourseRepository
    {
        IConfiguration _config { get; }

        #region Course Read Service Methods
        Task<IList<Course>> GetAllAsync();
        Task<IList<Course>> GetByCourseIdAsync(int courseId);
        Task<IList<Course>> GetByCourseTitleAsync(string courseTitle);
        Task<IList<Course>> SearchByCourseTitleAsync(string courseTitle);
        Task<IList<Course>> GetByCourseTypeIdAsync(int courseTypeId);
        Task<IList<Course>> GetByCourseTypeIdnCourseLevelIdAsync(int courseTypeId, int courseLevelId);
        Task<IList<Course>> GetByCourseTypeIdnSubjectAreaIdAsync(int courseTypeId, int subjectAreaId);
        Task<IList<Course>> GetByCourseTypeIdnSubjectAreaIdnCourseLevelIdAsync(int courseTypeId, int subjectAreaId, int courseLevelId);
        Task<IList<Course>> GetBySubjectAreaIdAsync(int subjectAreaId);
        Task<IList<Course>> GetByCourseLevelIdAsync(int courseLevelId);
        Task<IList<Course>> GetBySubjectAreaIdnCourseLevelIdAsync(int subjectAreaId, int courseLevelId);

        #endregion

        #region
        Task<bool> UpdateAsync(Course course);
        Task<bool> AddAsync(Course course);
        Task<bool> DeleteAsync(int courseId);
        #endregion
    }
}