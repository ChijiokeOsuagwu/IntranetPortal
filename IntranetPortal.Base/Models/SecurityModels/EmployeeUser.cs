using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SecurityModels
{
    public class EmployeeUser
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string OfficialEmail { get; set; }
        public string PersonalEmail { get; set; }

        public string UserName { get; set; }
        public string UserType { get; set; }
        public int AccessFailCount { get; set; }
        public string ConcurrencyString { get; set; }
        public bool EmailIsConfirmed { get; set; }
        public bool LockEnabled { get; set; }
        public DateTime? LockEndDate { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string CompanyCode { get; set; }
        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedTime { get; set; }
        public string ModifiedBy { get; set; }

        public string EmployeeNo1 { get; set; }
        public string EmployeeNo2 { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; }
    }
}
