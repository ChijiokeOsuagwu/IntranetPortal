using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetClassListViewModel:BaseListViewModel
    {
        public List<AssetClass> AssetClassList { get; set; }
    }
}
