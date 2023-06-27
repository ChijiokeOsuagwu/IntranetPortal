using IntranetPortal.Base.Models.SecurityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.AssetManagerModels
{
    public class AssetPermission : IEntityPermission
    {
        public string UserId { get; set; }
        public int? LocationId { get; set; }
    }
}
