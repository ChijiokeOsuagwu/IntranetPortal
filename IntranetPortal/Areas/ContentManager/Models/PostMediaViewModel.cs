using IntranetPortal.Base.Enums;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class PostMediaViewModel:BaseViewModel
    {
        public long PostMediaId { get; set; }

        [Display(Name = "Select File Type")]
        [Required]
        public int MediaTypeId { get; set; }

        [Display(Name = "File Type")]
        public string MediaTypeDescription { get; set; }

        [Display(Name = "Upload File")]
        [Required]
        public IFormFile MediaFile { get; set; }
        public string MediaLocationPath { get; set; }

        [Required]
        public long MasterPostId { get; set; }

        [Display(Name = "Caption")]
        public string Caption { get; set; }
    }
}
