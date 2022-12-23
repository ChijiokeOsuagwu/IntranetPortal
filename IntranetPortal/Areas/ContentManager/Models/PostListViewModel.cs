using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class PostListViewModel:BaseListViewModel
    {
        public string ss { get; set; }
        public int? pt { get; set; }
        public List<Post> PostList { get; set; }
    }
}
