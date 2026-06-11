using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveDocumentViewModel:BaseViewModel
    {
        public long LeaveDocumentId { get; set; }

        [Required]
        public long LeaveRequestId { get; set; }

        [Display(Name = "Upload File")]
        [Required]
        public IFormFile MediaFile { get; set; }
        
        public string DocumentReferencePath { get; set; }

        [Required]
        [Display(Name = "Title")]
        [MaxLength(100)]
        public string DocumentTitle { get; set; }

        [Display(Name = "Description")]
        [MaxLength(500)]
        public string DocumentDescription { get; set; }
    }
}
