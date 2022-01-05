using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.EmployeeRecordModels
{
    public class EmployeeHistoryInfo
    {
        public string EmployeeID { get; set; }
        public string EmployeeNo1 { get; set; }
        public string EmployeeName { get; set; }
        public int? YearsOfExperience { get; set; }
        public int? LengthOfService { get; set; }
        public string PlaceOfEngagement { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string CurrentDesignation { get; set; }
        public DateTime? DateOfLastPromotion { get; set; }
        public string EmployeeModifiedBy { get; set; }
        public string EmployeeModifiedDate { get; set; }
    }
}
