using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class AddPostDetailsViewModel:BaseViewModel
    {
        public int PostId { get; set; }
        public string CodedPostId { get; set; }
        public string PostTitle { get; set; }
        public string PostDetailsHtml { get; set; }
        public string PostDetailsRaw { get; set; }
        public int PostTypeId { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
