using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class GrantUserLocationPermissionViewModel:BaseListViewModel
    {
        public string id { get; set; }
        public List<Location> LocationsList { get; set; }
    }
}
