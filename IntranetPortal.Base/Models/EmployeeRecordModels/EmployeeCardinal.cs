using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeCardinal
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeUnitId { get; set; }
        public int EmployeeDepartmentId { get; set; }
        public int EmployeeLocationId { get; set; }
    }
}
