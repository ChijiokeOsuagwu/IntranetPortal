using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetListViewModel:BaseListViewModel
    {
        public int? tp { get; set; }
        public int? ct { get; set; }
        public int? cl { get; set; }
        public int? gp { get; set; }
        public string an { get; set; }
        public List<Asset> AssetList { get; set; }
    }
}
