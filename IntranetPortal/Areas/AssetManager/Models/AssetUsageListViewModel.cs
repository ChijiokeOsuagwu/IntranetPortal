using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetUsageListViewModel:BaseListViewModel
    {
        public string AssetID { get; set; }
        public string AssetName { get; set; }
        public int? yr { get; set; }
        public int? mn { get; set; }
        public List<AssetUsage> AssetUsageList { get; set; }
    }
}
