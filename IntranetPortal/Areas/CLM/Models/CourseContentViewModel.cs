using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.CLM.Models
{
    public class CourseContentViewModel:BaseViewModel
    {
        public long CourseContentId { get; set; }
    
        [Required]
        [Display(Name = "Topic")]
        [MaxLength(100, ErrorMessage ="Topic must not exceed 100 characters.")]
        public string ContentTitle { get; set; }

        [Display(Name = "Description")]
        [MaxLength(250, ErrorMessage = "Description must not exceed 250 characters.")]
        public string ContentDescription { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Display(Name = "Course Title")]
        public string CourseTitle { get; set; }

        [Required(ErrorMessage ="Please select Content Format.")]
        [Display(Name = "Content Format")]
        public int? ContentFormatId { get; set; }

        [Display(Name = "Format")]
        public string ContentFormatDescription { get; set; }

        [Required]
        [Display(Name = "Heading (Egs. Chapter One, Session 3, Module IV etc.)")]
        [MaxLength(50, ErrorMessage = "Heading must not exceed 50 characters.")]
        public string ContentHeading { get; set; }

        [Required]
        [Display(Name = "Seq.No.")]
        public int SequenceNo { get; set; }

        [Display(Name = "Credit To")]
        [MaxLength(250, ErrorMessage = "Credit To must not exceed 250 characters.")]
        public string ContentCreditTo { get; set; }

        [Display(Name = "Source")]
        [MaxLength(250, ErrorMessage = "Source must not exceed 250 characters.")]
        public string ContentSource { get; set; }

        [Display(Name = "Duration (Mins)")]
        public int DurationInMinutes { get; set; }

        [Display(Name = "Recommended Audience")]
        [MaxLength(100, ErrorMessage = "Recommended Audience must not exceed 100 characters.")]
        public string ContentAudience { get; set; }

        public string ContentBody { get; set; }
        public bool HasAssessment { get; set; }
        public string ContentUploadedBy { get; set; }
        public DateTime? ContentUploadTime { get; set; }

        public CourseContent ConvertToCourseContent()
        {
            return new CourseContent
            {
                CourseContentId = CourseContentId,
                ContentAudience = ContentAudience,
                ContentCreditTo = ContentCreditTo,
                ContentDescription = ContentDescription,
                ContentFormatDescription = ContentFormatDescription,
                ContentFormatId = ContentFormatId ?? 0,
                ContentHeading = ContentHeading,
                ContentBody = ContentBody,
                ContentSource = ContentSource,
                ContentTitle = ContentTitle,
                CourseId = CourseId,
                CourseTitle = CourseTitle,
                DurationInMinutes = DurationInMinutes,
                SequenceNo = SequenceNo,
            };
        }

        public CourseContentViewModel ExtractFromCourseContent(CourseContent content)
        {
            return new CourseContentViewModel
            {
                CourseContentId = content.CourseContentId,
                ContentAudience = content.ContentAudience,
                ContentCreditTo = content.ContentCreditTo,
                ContentDescription = content.ContentDescription,
                ContentFormatDescription = content.ContentFormatDescription,
                ContentFormatId = content.ContentFormatId,
                ContentHeading = content.ContentHeading,
                ContentBody = content.ContentBody,
                ContentSource = content.ContentSource,
                ContentTitle = content.ContentTitle,
                CourseId = content.CourseId,
                CourseTitle = content.CourseTitle,
                DurationInMinutes = content.DurationInMinutes,
                SequenceNo = content.SequenceNo,
            };
        }
    }
}
