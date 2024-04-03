using IntranetPortal.Base.Models.AssetManagerModels;
using System;
using System.Collections.Generic;
using IntranetPortal.Models;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.AssetManager.Models
{
    public class AssetStatusReportViewModel:BaseViewModel
    {
        public int? bsl { get; set; }
        public int? bnl { get; set; }
        public int? grp { get; set; }
        public int? typ { get; set; }
        public int? cnd { get; set; }

        public int RecordCount { get; set; }
        public List<Asset> AssetMasterList { get; set; }

    }
}
