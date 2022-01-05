using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.GlobalSettingsModels
{
    public class Unit
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public string UnitHeadID1 { get; set; }
        public string UnitHeadName1 { get; set; }
        public string UnitHeadID2 { get; set; }
        public string UnitHeadName2 { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}
