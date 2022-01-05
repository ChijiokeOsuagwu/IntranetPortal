using IntranetPortal.Base.Models.ContentManagerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Models
{
    public class GeneralIndexViewModel : BaseViewModel
    {
        public int NewMessageCount { get; set; }
        public List<Post> Banners { get; set; }
        public List<Post> Announcements { get; set; }
        public List<Post> Celebrants { get; set; }
        public List<Post> Articles { get; set; }
        public List<Post> Events { get; set; }
    }
}
