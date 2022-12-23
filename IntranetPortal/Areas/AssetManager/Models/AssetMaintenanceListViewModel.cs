using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetMaintenanceListViewModel
    {
        public int? yr { get; set; }
        public int? mn { get; set; }
        public string AssetID { get; set; }
        public List<AssetMaintenance> AssetMaintenanceList { get; set; }

    }
}
