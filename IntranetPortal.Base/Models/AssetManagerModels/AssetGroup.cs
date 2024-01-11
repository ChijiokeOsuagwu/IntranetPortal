using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetGroup
    {
        public int? GroupID { get; set; }
        public string GroupName { get; set; }
        public int? ClassID { get; set; }
        public string ClassName { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
