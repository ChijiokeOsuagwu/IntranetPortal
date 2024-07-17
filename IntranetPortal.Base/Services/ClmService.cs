using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Base.Repositories.ClmRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntranetPortal.Base.Services
{
    public class ClmService : IClmService
    {

        private readonly ICourseTypeRepository _courseTypeRepository;
        private readonly ISubjectAreaRepository _subjectAreaRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseContentRepository _courseContentRepository;

        public ClmService(ICourseTypeRepository CourseTypeRepository, ISubjectAreaRepository subjectAreaRepository,
            ICourseRepository courseRepository, ICourseContentRepository courseContentRepository)
        {
            _courseTypeRepository = CourseTypeRepository;
            _subjectAreaRepository = subjectAreaRepository;
            _courseRepository = courseRepository;
            _courseContentRepository = courseContentRepository;
        }

        #region Course Type Service Methods
        public async Task<List<CourseType>> GetCourseTypesAsync()
        {
            List<CourseType> courseTypeList = new List<CourseType>();
            var entities = await _courseTypeRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                courseTypeList = entities.ToList();
            }
            return courseTypeList;
        }

        public async Task<CourseType> GetCourseTypeAsync(int CourseTypeId)
        {
            CourseType courseType = new CourseType();
            var entities = await _courseTypeRepository.GetByIdAsync(CourseTypeId);
            if (entities != null && entities.Count > 0)
            {
                courseType = entities.ToList().FirstOrDefault();
            }
            return courseType;
        }

        public async Task<bool> AddCourseTypeAsync(CourseType courseType)
        {
            if (courseType == null) { throw new ArgumentNullException(nameof(courseType)); }
            var entities = await _courseTypeRepository.GetByDescriptionAsync(courseType.CourseTypeDescription);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Course Type with the same description already exists in the system.");
            }
            return await _courseTypeRepository.AddAsync(courseType);
        }

        public async Task<bool> EditCourseTypeAsync(CourseType courseType)
        {
            if (courseType == null) { throw new ArgumentNullException(nameof(courseType)); }
            var entities = await _courseTypeRepository.GetByDescriptionAsync(courseType.CourseTypeDescription);
            if (entities != null && entities.Count > 0)
            {
                List<CourseType> courseTypes = entities.ToList();
                foreach (CourseType c in courseTypes)
                {
                    if (c.CourseTypeId != courseType.CourseTypeId)
                    {
                        throw new Exception("A Course Type with the same description already exists in the system.");
                    }
                }
            }
            return await _courseTypeRepository.UpdateAsync(courseType);
        }

        public async Task<bool> DeleteCourseTypeAsync(int courseTypeId)
        {
            if (courseTypeId < 1) { throw new ArgumentNullException(nameof(courseTypeId)); }
            return await _courseTypeRepository.DeleteAsync(courseTypeId);
        }
        #endregion

        #region Course Subject Area Service Methods
        public async Task<List<SubjectArea>> GetSubjectAreasAsync()
        {
            List<SubjectArea> subjectAreaList = new List<SubjectArea>();
            var entities = await _subjectAreaRepository.GetAllAsync();
            if (entities != null && entities.Count > 0)
            {
                subjectAreaList = entities.ToList();
            }
            return subjectAreaList;
        }

        public async Task<SubjectArea> GetSubjectAreaAsync(int SubjectAreaId)
        {
            SubjectArea subjectArea = new SubjectArea();
            var entities = await _subjectAreaRepository.GetByIdAsync(SubjectAreaId);
            if (entities != null && entities.Count > 0)
            {
                subjectArea = entities.ToList().FirstOrDefault();
            }
            return subjectArea;
        }

        public async Task<bool> AddSubjectAreaAsync(SubjectArea subjectArea)
        {
            if (subjectArea == null) { throw new ArgumentNullException(nameof(subjectArea)); }
            var entities = await _subjectAreaRepository.GetByDescriptionAsync(subjectArea.SubjectAreaDescription);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Subject Area with the same name already exists in the system.");
            }


            return await _subjectAreaRepository.AddAsync(subjectArea);
        }

        public async Task<bool> EditSubjectAreaAsync(SubjectArea subjectArea)
        {
            if (subjectArea == null) { throw new ArgumentNullException(nameof(subjectArea)); }
            var entities = await _subjectAreaRepository.GetByDescriptionAsync(subjectArea.SubjectAreaDescription);
            if (entities != null && entities.Count > 0)
            {
                List<SubjectArea> subjectAreas = entities.ToList();
                foreach (SubjectArea s in subjectAreas)
                {
                    if (s.SubjectAreaId != subjectArea.SubjectAreaId)
                    {
                        throw new Exception("A Subject Area with the same name already exists in the system.");
                    }
                }
            }
            return await _subjectAreaRepository.UpdateAsync(subjectArea);
        }

        public async Task<bool> DeleteSubjectAreaAsync(int subjectAreaId)
        {
            if (subjectAreaId < 1) { throw new ArgumentNullException(nameof(subjectAreaId)); }
            return await _subjectAreaRepository.DeleteAsync(subjectAreaId);
        }
        #endregion

        #region Course Service Methods
        public async Task<List<Course>> FindCoursesAsync(int CourseTypeId, int SubjectAreaId, int CourseLevelId, string CourseTitle = null)
        {
            List<Course> courseList = new List<Course>();
            if (!string.IsNullOrWhiteSpace(CourseTitle))
            {
                var ct_entities = await _courseRepository.SearchByCourseTitleAsync(CourseTitle);
                if (ct_entities != null && ct_entities.Count > 0)
                {
                    courseList = ct_entities.ToList();
                }
            }
            else
            {
                if (CourseTypeId > 0)
                {
                    if (SubjectAreaId > 0)
                    {
                        if (CourseLevelId > 0)
                        {
                            var csc_entities = await _courseRepository.GetByCourseTypeIdnSubjectAreaIdnCourseLevelIdAsync(CourseTypeId, SubjectAreaId, CourseLevelId);
                            if (csc_entities != null && csc_entities.Count > 0)
                            {
                                courseList = csc_entities.ToList();
                            }
                        }
                        else
                        {
                            var csc_entities = await _courseRepository.GetByCourseTypeIdnSubjectAreaIdAsync(CourseTypeId, SubjectAreaId);
                            if (csc_entities != null && csc_entities.Count > 0)
                            {
                                courseList = csc_entities.ToList();
                            }
                        }
                    }
                    else
                    {
                        if (CourseLevelId > 0)
                        {
                            var cl_entities = await _courseRepository.GetByCourseTypeIdnCourseLevelIdAsync(CourseTypeId, CourseLevelId);
                            if (cl_entities != null && cl_entities.Count > 0)
                            {
                                courseList = cl_entities.ToList();
                            }
                        }
                        else
                        {
                            var cl_entities = await _courseRepository.GetByCourseTypeIdAsync(CourseTypeId);
                            if (cl_entities != null && cl_entities.Count > 0)
                            {
                                courseList = cl_entities.ToList();
                            }
                        }
                    }
                }
                else
                {
                    if (SubjectAreaId > 0)
                    {
                        if (CourseLevelId > 0)
                        {
                            var sl_entities = await _courseRepository.GetBySubjectAreaIdnCourseLevelIdAsync(SubjectAreaId, CourseLevelId);
                            if (sl_entities != null && sl_entities.Count > 0)
                            {
                                courseList = sl_entities.ToList();
                            }
                        }
                        else
                        {
                            var sl_entities = await _courseRepository.GetBySubjectAreaIdAsync(SubjectAreaId);
                            if (sl_entities != null && sl_entities.Count > 0)
                            {
                                courseList = sl_entities.ToList();
                            }
                        }
                    }
                    else
                    {
                        if (CourseLevelId > 0)
                        {
                            var cl_entities = await _courseRepository.GetByCourseLevelIdAsync(CourseLevelId);
                            if (cl_entities != null && cl_entities.Count > 0)
                            {
                                courseList = cl_entities.ToList();
                            }
                        }
                        else
                        {
                            var cl_entities = await _courseRepository.GetAllAsync();
                            if (cl_entities != null && cl_entities.Count > 0)
                            {
                                courseList = cl_entities.ToList();
                            }
                        }
                    }
                }
            }
            return courseList;
        }

        public async Task<Course> GetCourseAsync(int CourseId)
        {
            Course course = new Course();
            var entities = await _courseRepository.GetByCourseIdAsync(CourseId);
            if (entities != null && entities.Count > 0)
            {
                course = entities.ToList().FirstOrDefault();
            }
            return course;
        }

        public async Task<bool> CreateCourseAsync(Course course)
        {
            if (course == null) { throw new ArgumentNullException(nameof(course)); }
            var entities = await _courseRepository.GetByCourseTitleAsync(course.CourseTitle);
            if (entities != null && entities.Count > 0)
            {
                throw new Exception("A Course with the same title already exists in the system.");
            }
            return await _courseRepository.AddAsync(course);
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            if (course == null) { throw new ArgumentNullException(nameof(course)); }
            var entities = await _courseRepository.GetByCourseTitleAsync(course.CourseTitle);
            if (entities != null && entities.Count > 0)
            {
                List<Course> courses = entities.ToList();
                foreach (Course c in courses)
                {
                    if (c.CourseId != course.CourseId)
                    {
                        throw new Exception("A Course with the same description already exists in the system.");
                    }
                }
            }
            return await _courseRepository.UpdateAsync(course);
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            if (courseId < 1) { throw new ArgumentNullException(nameof(courseId)); }
            return await _courseRepository.DeleteAsync(courseId);
        }

        #endregion

        #region Course Contents Service Methods
        public async Task<List<CourseContent>> FindCourseContentsAsync(int CourseId, int? CourseFormatId = null)
        {
            List<CourseContent> courseContentList = new List<CourseContent>();
            if (CourseFormatId == null)
            {
                var ct_entities = await _courseContentRepository.GetByCourseIdAsync(CourseId);
                if (ct_entities != null && ct_entities.Count > 0)
                {
                    courseContentList = ct_entities.ToList();
                }
            }
            else
            {
                var ct_entities = await _courseContentRepository.GetByCourseIdnFormatIdAsync(CourseId, CourseFormatId.Value);
                if (ct_entities != null && ct_entities.Count > 0)
                {
                    courseContentList = ct_entities.ToList();
                }
            }
            return courseContentList;
        }

        public async Task<CourseContent> GetCourseContentAsync(long CourseContentId)
        {
            CourseContent courseContent = new CourseContent();
            var entities = await _courseContentRepository.GetByIdAsync(CourseContentId);
            if (entities != null && entities.Count > 0)
            {
                courseContent = entities.ToList().FirstOrDefault();
            }
            return courseContent;
        }

        public async Task<bool> CreateCourseContentAsync(CourseContent courseContent)
        {
            if (courseContent == null) { throw new ArgumentNullException(nameof(courseContent)); }
            var t_entities = await _courseContentRepository.GetByContentTitleAsync(courseContent.CourseId, courseContent.ContentTitle);
            if (t_entities != null && t_entities.Count > 0)
            {
                throw new Exception("A Course Content with the same title already exists in the system.");
            }

            var h_entities = await _courseContentRepository.GetByHeadingAsync(courseContent.CourseId, courseContent.ContentHeading);
            if (h_entities != null && h_entities.Count > 0)
            {
                throw new Exception("A Course Content with the same heading already exists in the system.");
            }
            return await _courseContentRepository.AddAsync(courseContent);
        }

        public async Task<bool> UpdateCourseContentAsync(CourseContent courseContent)
        {
            CourseContent content = new CourseContent();
            if (courseContent == null) { throw new ArgumentNullException(nameof(courseContent)); }
            var t_entities = await _courseContentRepository.GetByContentTitleAsync(courseContent.CourseId, courseContent.ContentTitle);
            if (t_entities != null && t_entities.Count > 0)
            {
                content = t_entities.FirstOrDefault();
                if (content.CourseContentId != courseContent.CourseContentId)
                {
                    throw new Exception("A Course Content with the same title already exists in the system.");
                }
            }

            var h_entities = await _courseContentRepository.GetByHeadingAsync(courseContent.CourseId, courseContent.ContentHeading);
            if (h_entities != null && h_entities.Count > 0)
            {
                content = h_entities.FirstOrDefault();
                if (content.CourseContentId != courseContent.CourseContentId)
                {
                    throw new Exception("A Course Content with the same heading already exists in the system.");
                }
            }

            return await _courseContentRepository.UpdateAsync(courseContent);
        }

        public async Task<bool> DeleteCourseContentAsync(long courseContentId)
        {
            CourseContent courseContent = new CourseContent();
            if (courseContentId < 1) { throw new ArgumentNullException(nameof(courseContentId)); }
            return  await _courseContentRepository.DeleteAsync(courseContentId);
        }

        public async Task<bool> UpdateContentTextAsync(long courseContentId, string contentText)
        {
            return await _courseContentRepository.UpdateTextAsync(courseContentId, contentText);
        }

        public async Task<bool> UpdateContentLinkAsync(long courseContentId, string contentLink, string contentFullPath)
        {
            return await _courseContentRepository.UpdateLinkAsync(courseContentId, contentLink, contentFullPath);
        }

        #endregion
    }
}
