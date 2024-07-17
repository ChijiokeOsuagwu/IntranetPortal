using IntranetPortal.Base.Models.ClmModels;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.CLM.Models
{
    public class UploadContentViewModel:BaseViewModel
    {
        [Required]
        public long CourseContentID { get; set; }
        [Required]
        public int CourseID { get; set; }
        public string ContentHeading { get; set; }
        public string ContentTitle { get; set; }
        [Required]
        public int FormatID { get; set; }
        public ContentFormat Format { get; set; }
        public string OldContentUrl { get; set; }

        public string NewContentUrl { get; set; }

        [Required(ErrorMessage ="Please select a file to upload.")]
        [Display(Name ="Upload File (Max file size = 250MB)")]
        public IFormFile ContentFile { get; set; }
    }
}
