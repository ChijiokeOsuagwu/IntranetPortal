using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.GlobalSettingsModels
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentHeadID1 { get; set; }
        public string DepartmentHeadName1 { get; set; }
        public string DepartmentHeadID2 { get; set; }
        public string DepartmentHeadName2 { get; set; }
    }
}
