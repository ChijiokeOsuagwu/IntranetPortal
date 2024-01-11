using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class ApprovalRolesListViewModel:BaseViewModel
    {
        public List<ApprovalRole> ApprovalRoleList { get; set; }
    }
}
