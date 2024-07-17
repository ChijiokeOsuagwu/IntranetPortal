using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.ClmModels
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseOverview { get; set; }
        public int CourseTypeId { get; set; }
        public string CourseTypeDescription { get; set; }
        public int SubjectAreaId { get; set; }
        public string SubjectAreaDescription { get; set; }
        public int CourseLevelId { get; set; }
        public string CourseLevelDescription { get; set; }
        public string CreditTo { get; set; }
        public string CourseSource { get; set; }
        public string RecommendedAudience { get; set; }
        public int? DurationInHours { get; set; }
        public bool RequiresEnrollment { get; set; }
        public string UploadedBy { get; set; }
        public DateTime? UploadedTime { get; set; }



    }
}
