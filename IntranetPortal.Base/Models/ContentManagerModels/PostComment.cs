using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.ContentManagerModels
{
    public class PostComment
    {
        public int PostCommentId { get; set; }
        public string PostCommentText { get; set; }
        public string PostCommentImage { get; set; }
        public int PostId { get; set; }
        public string CommentType { get; set; }
        public DateTime CommentDate { get; set; }
        public string CommentBy { get; set; }
    }
}
