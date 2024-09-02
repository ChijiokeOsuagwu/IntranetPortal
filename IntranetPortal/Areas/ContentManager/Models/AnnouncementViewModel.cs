using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class AnnouncementViewModel:BaseViewModel
    {
        public int? PostId { get; set; }

        [Required]
        public int PostTypeId { get; set; }

        [Required]
        [MaxLength(155, ErrorMessage = "Announcements must not exceed 155 characters.")]
        [Display(Name = "Announcement Text (Less than 150 characters)")]
        public string PostDetails { get; set; }
    }
}
