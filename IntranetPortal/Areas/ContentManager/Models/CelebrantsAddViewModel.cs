using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class CelebrantsAddViewModel:BaseViewModel
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title cannot be more than 100 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Summary cannot be more than 250 characters.")]
        [Display(Name = "Message")]
        public string Summary { get; set; }

        [Required]
        [Display(Name = "Upload Celebrant's Image")]
        public IFormFile CelebrantImage { get; set; }
        public bool EnableComments { get; set; }
        public bool IsHidden { get; set; }

    }
}
