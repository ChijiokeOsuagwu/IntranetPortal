using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class CelebrantEditViewModel:BaseViewModel
    {
        public long Id { get; set; }
        public string CodedId { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Title cannot be more than 50 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Caption cannot be more than 100 characters.")]
        [Display(Name = "Message")]
        public string Summary { get; set; }

        [Required]
        [Display(Name = "Upload Image")]
        public IFormFile CelebrantImage { get; set; }
        public bool EnableComments { get; set; }
        public bool IsHidden { get; set; }
        public string ImagePath { get; set; }

    }
}
