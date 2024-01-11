using IntranetPortal.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SecurityModels
{
    public abstract class EntityPermission
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string UserFullName { get; set; }
        public EntityPermissionType PermissionType { get; set; }
    }
}
