using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SecurityModels
{
    public class LocationPermission
    {
        public int LocationPermissionId { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public int PermissionTypeId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }
}
