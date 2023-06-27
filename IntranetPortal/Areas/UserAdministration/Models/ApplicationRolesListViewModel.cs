using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class ApplicationRolesListViewModel:BaseListViewModel
    {
        public List<ApplicationRole> ApplicationRoleList { get; set; }
    }
}
