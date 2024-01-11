using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class SelectTypeListViewModel
    {
        public int? cl { get; set; }
        public int? gp { get; set; }
        public string tn { get; set; }
        public List<AssetType> AssetTypeList { get; set; }
    }
}
