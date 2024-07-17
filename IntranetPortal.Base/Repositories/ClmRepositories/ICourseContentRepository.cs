using IntranetPortal.Base.Models.ClmModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Repositories.ClmRepositories
{
    public interface ICourseContentRepository
    {
        IConfiguration _config { get; }

        //===== Course Content Read Action Methods =====//
        Task<IList<CourseContent>> GetByCourseIdAsync(int courseId);
        Task<IList<CourseContent>> GetByCourseIdnFormatIdAsync(int courseId, int formatId);
        Task<IList<CourseContent>> GetByIdAsync(long courseContentId);
        Task<IList<CourseContent>> GetByContentTitleAsync(int courseId, string courseContentTitle);
        Task<IList<CourseContent>> GetByHeadingAsync(int courseId, string contentHeading);

        //===== Course Content Write Action Methods =====//
        Task<bool> UpdateAsync(CourseContent courseContent);
        Task<bool> AddAsync(CourseContent courseContent);
        Task<bool> DeleteAsync(long courseContentId);
        Task<bool> UpdateTextAsync(long courseContentId, string textContent);
        Task<bool> UpdateLinkAsync(long courseContentId, string contentLink, string contentFullPath);
    }
}