using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class PostEditViewModel : BaseViewModel
    {
        public long? PostId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
        [Display(Name = "Title")]
        public string PostTitle { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "Summary must not exceed 500 characters.")]
        [Display(Name = "Summary")]
        public string PostSummary { get; set; }

        public string OldImagePath { get; set; }

        public string ImagePath { get; set; }

        [Display(Name = "Upload Image (Less than 1MB)")]
        public IFormFile ImageFile { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int PostTypeId { get; set; }
        public bool EnableComments { get; set; }
        public bool IsHidden { get; set; }
        public string PostDetails { get; set; }

        [Display(Name = "Enter Full Article Below:")]
        public string PostDetailsRaw { get; set; }

        public Post ConvertToPost()
        {
            return new Post
            {
                EnableComment = EnableComments,
                ImagePath = ImagePath,
                IsHidden = IsHidden,
                PostId = PostId == null ? 0 : PostId.Value,
                PostTitle = PostTitle,
                PostSummary = PostSummary,
                PostTypeId = PostTypeId,
                PostDetails = PostDetails,
                PostDetailsRaw = PostDetailsRaw
            };
        }

        public PostEditViewModel ExtraFromPost(Post p)
        {
            return new PostEditViewModel
            {
                EnableComments = p.EnableComment,
                ImagePath = p.ImagePath,
                IsHidden = p.IsHidden,
                PostId = PostId == null ? 0 : PostId.Value,
                PostTitle = p.PostTitle,
                PostSummary = p.PostSummary,
                PostTypeId = p.PostTypeId,
                PostDetails = p.PostDetails,
                PostDetailsRaw = p.PostDetailsRaw,
            };
        }
    }
}
