using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.ContentManagerModels
{
    public class Post
    {
        public long PostId { get; set; }
        public string CodedPostId { get; set; }
        public string PostTitle { get; set; }
        public string PostSummary { get; set; }
        public string PostDetails { get; set; }
        public string PostDetailsRaw { get; set; }
        public string ImagePath { get; set; }
        public string ImageFullPath { get; set; }
        public int PostTypeId { get; set; }
        public string PostTypeName { get; set; }
        public bool EnableComment { get; set; }
        public bool IsHidden { get; set; }
        public bool HasComments { get; set; }
        public bool HasMedia { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
