using IntranetPortal.Base.Models.ContentManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ContentManager.Models
{
    public class BannerListViewModel : BaseViewModel
    {
        public List<Post> Banners { get; set; }
    }
}
