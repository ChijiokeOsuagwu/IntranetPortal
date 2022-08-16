using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.UserAdministration.Models
{
    public class EmployeeUserDetailsViewModel : BaseViewModel
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string EmployeeNo1 { get; set; }
        public string EmployeeNo2 { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public string UnitName { get; set; }
        public string LoginID { get; set; }
        public bool EnableLockOut { get; set; }
        public string UserType { get; set; }
        public string OfficialEmail { get; set; }
        public string PersonalEmail { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string Sex { get; set; }
    }
}
