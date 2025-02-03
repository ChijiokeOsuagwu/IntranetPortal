using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeaveAttachmentViewModel:BaseViewModel
    {
        public long DocumentId { get; set; }

        [Required]
        public long LeaveId { get; set; }

        [Required]
        [Display(Name ="Title")]
        public string DocumentTitle { get; set; }

        [Display(Name = "Description")]
        public string DocumentDescription { get; set; }

        [Required]
        [Display(Name = "Format")]
        public string DocumentFormat { get; set; }

        [Required]
        [Display(Name = "Upload File")]
        public IFormFile UploadedFile { get; set; }

        public string DocumentLink { get; set; }
        public DateTime? UploadedTime { get; set; }
        public string UploadedBy { get; set; }

    }
}
