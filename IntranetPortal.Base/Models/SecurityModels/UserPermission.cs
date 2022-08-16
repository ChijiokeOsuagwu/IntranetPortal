using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SecurityModels
{
    public class UserPermission
    {
        public string PermissionID { get; set; }
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsGranted { get; set; }
        public string ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }

    }
}
