using IntranetPortal.Base.Models.AssetManagerModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class AssetPermissionsListViewModel:BaseViewModel
    {
        public string id { get; set; }
        public List<AssetPermission> AssetPermissionList { get; set; }
    }
}
