using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetPermission : EntityPermission
    {
        public int AssetDivisionID { get; set; }
        public string AssetDivisionName { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
    }
}
