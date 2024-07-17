using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.ClmModels
{
    public class CourseContent
    {
        public long CourseContentId { get; set; }
        public string ContentTitle { get; set; }
        public string ContentDescription { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int ContentFormatId { get; set; }
        public string ContentFormatDescription { get; set; }
        public string ContentHeading { get; set; }
        public int SequenceNo { get; set; }
        public string ContentBody { get; set; }
        public string ContentLink { get; set; }
        public string ContentFullPath { get; set; }
        public string ContentCreditTo { get; set; }
        public string ContentSource { get; set; }
        public int DurationInMinutes { get; set; }
        public string ContentAudience { get; set; }
        public bool HasAssessment { get; set; }
        public string ContentUploadedBy { get; set; }
        public DateTime? ContentUploadTime { get; set; }
    }
}
