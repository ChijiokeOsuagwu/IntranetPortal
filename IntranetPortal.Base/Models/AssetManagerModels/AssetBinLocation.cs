using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetBinLocation
    {
        public int AssetBinLocationID { get; set; }
        public string AssetBinLocationName { get; set; }
        public string AssetBinLocationDescription { get; set; }
        public int AssetLocationID { get; set; }
        public string AssetLocationName { get; set; }
    }
}
