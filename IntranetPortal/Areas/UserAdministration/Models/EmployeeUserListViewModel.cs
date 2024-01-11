using IntranetPortal.Base.Models.SecurityModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class EmployeeUserListViewModel:BaseListViewModel
    {
        public string ss { get; set; }
        public List<EmployeeUser> EmployeeUsers { get; set; }
    }
}
