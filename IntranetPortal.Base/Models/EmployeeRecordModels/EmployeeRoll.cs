using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeRoll
    {
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string EmployeeNo1 { get; set; }
        public string Sex { get; set; }
        public string OfficialEmail { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public int? UnitID { get; set; }
        public string UnitName { get; set; }
        public int? LocationID { get; set; }
        public string LocationName { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }


    }
}
