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
        public List<Asset> AssetList { get; set; }
    }
}
