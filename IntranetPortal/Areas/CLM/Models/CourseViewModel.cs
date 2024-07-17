using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.CLM.Models
{
    public class CourseViewModel:BaseViewModel
    {
        public int CourseId { get; set; }
        [Required]
        [Display(Name = "Course Title")]
        [MaxLength(100,ErrorMessage ="Course Title must not exceed 100 characters.")]
        public string CourseTitle { get; set; }

        [Display(Name = "Course Overview")]
        [MaxLength(500, ErrorMessage = "Course Overview must not exceed 500 characters.")]
        public string CourseOverview { get; set; }

        [Required]
        [Display(Name = "Course Type")]
        public int CourseTypeId { get; set; }

        [Display(Name = "Course Type")]
        public string CourseTypeDescription { get; set; }

        [Required]
        [Display(Name = "Subject Area")]
        public int SubjectAreaId { get; set; }

        [Display(Name = "Subject Area")]
        public string SubjectAreaDescription { get; set; }

        [Required]
        [Display(Name = "Course Level")]
        public int CourseLevelId { get; set; }

        [Display(Name = "Course Level")]
        public string CourseLevelDescription { get; set; }

        [Display(Name = "Credit To")]
        public string CreditTo { get; set; }

        [Display(Name = "Source")]
        public string CourseSource { get; set; }

        [Display(Name = "Recommended Audience")]
        public string RecommendedAudience { get; set; }

        [Display(Name = "Duration (Hrs)")]
        public int DurationInHours { get; set; }

        [Display(Name = "Requires Enrollment")]
        public bool RequiresEnrollment { get; set; }
        public string UploadedBy { get; set; }
        public DateTime? UploadedTime { get; set; }

        public Course ConvertToCourse()
        {
            return new Course
            {
                CourseId = CourseId,
                CourseTitle = CourseTitle,
                CourseOverview = CourseOverview,
                CourseTypeId = CourseTypeId,
                CourseTypeDescription = CourseTypeDescription,
                CourseLevelId = CourseLevelId,
                CourseLevelDescription = CourseLevelDescription,
                SubjectAreaId = SubjectAreaId,
                SubjectAreaDescription = SubjectAreaDescription,
                CreditTo = CreditTo,
                CourseSource = CourseSource,
                RecommendedAudience = RecommendedAudience,
                DurationInHours = DurationInHours,
                RequiresEnrollment = RequiresEnrollment,
                UploadedBy = UploadedBy,
                UploadedTime = UploadedTime
            };
        }

        public CourseViewModel ExtractFromCourse(Course course)
        {
            return new CourseViewModel
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseTitle,
                CourseOverview = course.CourseOverview,
                CourseTypeId = course.CourseTypeId,
                CourseTypeDescription = course.CourseTypeDescription,
                CourseLevelId = course.CourseLevelId,
                CourseLevelDescription = course.CourseLevelDescription,
                SubjectAreaId = course.SubjectAreaId,
                SubjectAreaDescription = course.SubjectAreaDescription,
                CreditTo = course.CreditTo,
                CourseSource = course.CourseSource,
                RecommendedAudience = course.RecommendedAudience,
                DurationInHours = course.DurationInHours ?? 0,
                RequiresEnrollment = course.RequiresEnrollment,
                UploadedBy = course.UploadedBy,
                UploadedTime = course.UploadedTime
            };
        }
    }
}
