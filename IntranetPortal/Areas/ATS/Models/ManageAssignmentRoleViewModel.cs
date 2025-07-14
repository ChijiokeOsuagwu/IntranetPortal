using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.ATS.Models
{
    public class ManageAssignmentRoleViewModel:BaseViewModel
    {
        public int? Id { get; set; }
        public string Description { get; set; }
    }
}
