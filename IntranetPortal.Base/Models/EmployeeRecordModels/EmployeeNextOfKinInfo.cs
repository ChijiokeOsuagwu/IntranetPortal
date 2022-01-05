using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeNextOfKinInfo
    {
        public string EmployeeID { get; set; }
        public string NextOfKinName { get; set; }
        public string NextOfKinAddress { get; set; }
        public string NextOfKinEmail { get; set; }
        public string NextOfKinPhone { get; set; }
        public string NextOfKinRelationship { get; set; }
        public string EmployeeModifiedBy { get; set; }
        public string EmployeeModifiedDate { get; set; }
    }
}
