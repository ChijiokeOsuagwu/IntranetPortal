using IntranetPortal.Base.Models.ClmModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public interface IClmService
    {
        #region Course Type Service Methods
        Task<bool> AddCourseTypeAsync(CourseType courseType);
        Task<bool> DeleteCourseTypeAsync(int courseTypeId);
        Task<bool> EditCourseTypeAsync(CourseType courseType);
        Task<CourseType> GetCourseTypeAsync(int CourseTypeId);
        Task<List<CourseType>> GetCourseTypesAsync();
        #endregion

        #region Subject Area Service Methods
        Task<SubjectArea> GetSubjectAreaAsync(int SubjectAreaId);
        Task<List<SubjectArea>> GetSubjectAreasAsync();
        Task<bool> AddSubjectAreaAsync(SubjectArea subjectArea);
        Task<bool> EditSubjectAreaAsync(SubjectArea subjectArea);
        Task<bool> DeleteSubjectAreaAsync(int subjectAreaId);
        #endregion

        #region Course Service Methods
        Task<List<Course>> FindCoursesAsync(int CourseTypeId, int SubjectAreaId, int CourseLevelId, string CourseTitle = null);
        Task<Course> GetCourseAsync(int CourseId);
        Task<bool> CreateCourseAsync(Course course);
        Task<bool> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int courseId);
        #endregion

        #region Course Content Service Methods
        Task<List<CourseContent>> FindCourseContentsAsync(int CourseId, int? CourseFormatId = null);
        Task<CourseContent> GetCourseContentAsync(long CourseContentId);
        Task<bool> CreateCourseContentAsync(CourseContent courseContent);
        Task<bool> UpdateCourseContentAsync(CourseContent courseContent);
        Task<bool> DeleteCourseContentAsync(long courseContentId);
        Task<bool> UpdateContentTextAsync(long courseContentId, string contentText);
        Task<bool> UpdateContentLinkAsync(long courseContentId, string contentLink, string contentFullPath);
        #endregion
    }
}